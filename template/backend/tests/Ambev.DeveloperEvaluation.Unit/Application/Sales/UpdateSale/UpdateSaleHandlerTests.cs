using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Rebus.Bus;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Messaging;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IDiscountCalculator _discountCalculator;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _discountCalculator = Substitute.For<IDiscountCalculator>();
        _mapper = Substitute.For<IMapper>();
        _bus = Substitute.For<IBus>();
        _handler = new UpdateSaleHandler(_saleRepository, _discountCalculator, _mapper, _bus);
    }

    [Fact(DisplayName = "Given valid command When handling Then updates sale and returns result")]
    public async Task Handle_ValidCommand_ReturnsResult()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand
        {
            Id = saleId,
            Number = 2,
            CustomerName = "Customer",
            Branch = "Branch",
            Date = DateTime.UtcNow,
            Products =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 5m }
            ]
        };

        var existingSale = new Sale { Id = saleId, Number = 1, CustomerName = "Old", Branch = "Old" };

        var expectedResult = new UpdateSaleResult { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map(command, existingSale).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(existingSale).Returns(expectedResult);

        // Act
        var handled = await _handler.Handle(command, CancellationToken.None);

        // Assert
        handled.Should().NotBeNull();
        handled.Id.Should().Be(saleId);
        _discountCalculator.Received(1).ApplyDiscounts(Arg.Is<Sale>(s => s == existingSale));
        await _saleRepository.Received(1).UpdateAsync(Arg.Is<Sale>(s => s == existingSale), Arg.Any<CancellationToken>());
        await _bus.Received(1).Publish(Arg.Is<SaleModified>(e => e.Id == existingSale.Id));
    }

    [Fact(DisplayName = "Given status changed to Cancelled When handling Then publishes SaleCancelled event")]
    public async Task Handle_StatusChangedToCancelled_PublishesSaleCancelled()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var existingSale = new Sale { Id = saleId, Number = 1, CustomerName = "Old", Branch = "Old", Status = SalesStatus.Pending };

        var command = new UpdateSaleCommand
        {
            Id = saleId,
            Number = 2,
            CustomerName = "Customer",
            Branch = "Branch",
            Date = DateTime.UtcNow,
            Status = SalesStatus.Cancelled,
            Products = [ new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 5m } ]
        };

        var expectedResult = new UpdateSaleResult { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map(command, existingSale).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(existingSale).Returns(expectedResult);

        // Act
        var handled = await _handler.Handle(command, CancellationToken.None);

        // Assert
        handled.Should().NotBeNull();
        handled.Id.Should().Be(saleId);
        await _bus.Received(1).Publish(Arg.Is<SaleCancelled>(e => e.Id == existingSale.Id));
    }

    [Fact(DisplayName = "Given sale not found When handling Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand
        {
            Id = saleId,
            Number = 2,
            CustomerName = "Customer",
            Branch = "Branch",
            Date = DateTime.UtcNow,
            Products =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 5m }
            ]
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new UpdateSaleCommand();
        _saleRepository.SaleExistsAsync(Arg.Any<long>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
