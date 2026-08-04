using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation.TestHelper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CreateSale;

public class CreateSaleCommandValidatorTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly CreateSaleCommandValidator _validator;

    public CreateSaleCommandValidatorTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _validator = new CreateSaleCommandValidator(_saleRepository);
    }

    [Fact(DisplayName = "Valid command should pass all validation rules")]
    public async Task Given_ValidCommand_When_Validated_Then_NoErrors()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            Number = 1,
            CustomerName = "John Doe",
            Date = DateTime.UtcNow,
            Branch = "Main",
            Products =
            [
                new CreateSaleProductCommand { ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 10m },
                new CreateSaleProductCommand { ProductId = Guid.NewGuid(), Quantity = 5, UnitPrice = 5m }
            ]
        };

        _saleRepository.SaleExistsAsync(Arg.Any<long>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Number must be greater than zero")]
    public async Task Given_NumberZero_When_Validated_Then_HasError()
    {
        var command = new CreateSaleCommand { Number = 0, CustomerName = "A", Branch = "B", Products = [new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 0 }] };
        _saleRepository.SaleExistsAsync(Arg.Any<long>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Number);
    }

    [Fact(DisplayName = "Number must be unique")]
    public async Task Given_NumberAlreadyExists_When_Validated_Then_HasError()
    {
        var command = new CreateSaleCommand { Number = 5, CustomerName = "A", Branch = "B", Products = [new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 0 }] };
        _saleRepository.SaleExistsAsync(5, Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await _validator.TestValidateAsync(command);

        var failures = result.ShouldHaveValidationErrorFor(c => c.Number);
        failures.WithErrorMessage("Sale number must be unique.");
    }

    [Fact(DisplayName = "Products cannot be empty")]
    public async Task Given_NoProducts_When_Validated_Then_HasError()
    {
        var command = new CreateSaleCommand { Number = 1, CustomerName = "A", Branch = "B", Products = [] };
        _saleRepository.SaleExistsAsync(Arg.Any<long>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _validator.TestValidateAsync(command);

        var failures = result.ShouldHaveValidationErrorFor(c => c.Products);
        failures.WithErrorMessage("At least one product is required.");
    }

    [Fact(DisplayName = "Duplicate products should fail validation")]
    public async Task Given_DuplicateProducts_When_Validated_Then_HasError()
    {
        var pid = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            Number = 1,
            CustomerName = "A",
            Branch = "B",
            Products =
            [
                new() { ProductId = pid, Quantity = 1, UnitPrice = 1m },
                new() { ProductId = pid, Quantity = 1, UnitPrice = 1m }
            ]
        };
        _saleRepository.SaleExistsAsync(Arg.Any<long>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _validator.TestValidateAsync(command);

        var failures = result.ShouldHaveValidationErrorFor(c => c.Products);
        failures.WithErrorMessage("Each product can only be added once.");
    }

    [Fact(DisplayName = "Product max quantity exceeded should fail validation")]
    public async Task Given_ProductQuantityExceeded_When_Validated_Then_HasError()
    {
        var command = new CreateSaleCommand
        {
            Number = 1,
            CustomerName = "A",
            Branch = "B",
            Products =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 21, UnitPrice = 1m }
            ]
        };
        _saleRepository.SaleExistsAsync(Arg.Any<long>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _validator.TestValidateAsync(command);

        var failures = result.ShouldHaveValidationErrorFor(c => c.Products);
        failures.WithErrorMessage("Each product can have a maximum of 20 units.");
    }

    [Fact(DisplayName = "Product item fields should be validated")]
    public async Task Given_ProductWithInvalidFields_When_Validated_Then_HasErrors()
    {
        var command = new CreateSaleCommand
        {
            Number = 1,
            CustomerName = "A",
            Branch = "B",
            Products =
            [
                new() { ProductId = Guid.Empty, Quantity = 0, UnitPrice = -1m }
            ]
        };
        _saleRepository.SaleExistsAsync(Arg.Any<long>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor("Products[0].ProductId");
        result.ShouldHaveValidationErrorFor("Products[0].Quantity");
        result.ShouldHaveValidationErrorFor("Products[0].UnitPrice");
    }
}
