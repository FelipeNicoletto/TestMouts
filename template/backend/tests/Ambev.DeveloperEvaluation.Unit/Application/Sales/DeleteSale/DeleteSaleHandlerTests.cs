using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.DeleteSale;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _handler = new DeleteSaleHandler(_saleRepository);
    }

    [Fact(DisplayName = "Given existing sale When handling Then deletes without error")]
    public async Task Handle_ExistingSale_Deletes()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteSaleCommand(id);
        var sale = new Ambev.DeveloperEvaluation.Domain.Entities.Sale { Id = id, Number = 1 };
        _saleRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        await _saleRepository.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non existing sale When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteSaleCommand(id);
        _saleRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Ambev.DeveloperEvaluation.Domain.Entities.Sale?)null);
        _saleRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _saleRepository.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new DeleteSaleCommand(Guid.Empty);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
