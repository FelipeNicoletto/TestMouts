using FluentValidation.TestHelper;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.DeleteSale;

public class DeleteSaleCommandValidatorTests
{
    private readonly DeleteSaleCommandValidator _validator;

    public DeleteSaleCommandValidatorTests()
    {
        _validator = new DeleteSaleCommandValidator();
    }

    [Fact(DisplayName = "Valid id should not produce validation error")]
    public void Given_ValidId_When_Validated_Then_NoErrors()
    {
        var command = new DeleteSaleCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty id should produce validation error")]
    public void Given_EmptyId_When_Validated_Then_HasError()
    {
        var command = new DeleteSaleCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
}
