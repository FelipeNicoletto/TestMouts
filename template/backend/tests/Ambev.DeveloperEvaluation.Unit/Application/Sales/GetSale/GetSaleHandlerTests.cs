using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.GetSale;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given existing sale When handling Then returns mapped result")]
    public async Task Handle_ExistingSale_ReturnsResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new GetSaleCommand(id);
        var sale = new Sale { Id = id, Number = 1, CustomerName = "C", Branch = "B" };
        var expected = new GetSaleResult { Id = id };

        _saleRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(expected);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        await _saleRepository.Received(1).GetByIdAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non existing sale When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new GetSaleCommand(id);
        _saleRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _saleRepository.Received(1).GetByIdAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new GetSaleCommand(Guid.Empty);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
