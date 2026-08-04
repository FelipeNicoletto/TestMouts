using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using MassTransit;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Messaging;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CreateSale;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IDiscountCalculator _discountCalculator;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _discountCalculator = Substitute.For<IDiscountCalculator>();
        _mapper = Substitute.For<IMapper>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _handler = new CreateSaleHandler(_saleRepository, _discountCalculator, _mapper, _publishEndpoint);
    }

    [Fact(DisplayName = "Given valid command When handling Then creates sale and returns result")]
    public async Task Handle_ValidCommand_ReturnsResult()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            Number = 10,
            CustomerName = "Customer",
            Branch = "Branch A",
            Date = DateTime.UtcNow,
            Products =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 5m }
            ]
        };

        var sale = new Sale { Id = Guid.NewGuid(), Number = command.Number, CustomerName = command.CustomerName, Branch = command.Branch };
        var result = new CreateSaleResult { Id = sale.Id };

        _saleRepository.SaleExistsAsync(Arg.Any<long>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);

        // Act
        var handled = await _handler.Handle(command, CancellationToken.None);

        // Assert
        handled.Should().NotBeNull();
        handled.Id.Should().Be(sale.Id);
        _discountCalculator.Received(1).ApplyDiscounts(Arg.Is<Sale>(s => s == sale));
        await _saleRepository.Received(1).CreateAsync(Arg.Is<Sale>(s => s == sale), Arg.Any<CancellationToken>());
        await _publishEndpoint.Received(1).Publish(Arg.Is<SaleCreated>(e => e.Id == sale.Id), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new CreateSaleCommand();

        _saleRepository.SaleExistsAsync(Arg.Any<long>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
