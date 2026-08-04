using FluentValidation.TestHelper;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.GetSale;

public class GetSaleCommandValidatorTests
{
    private readonly GetSaleCommandValidator _validator;

    public GetSaleCommandValidatorTests()
    {
        _validator = new GetSaleCommandValidator();
    }

    [Fact(DisplayName = "Valid id should not produce validation error")]
    public void Given_ValidId_When_Validated_Then_NoErrors()
    {
        var command = new GetSaleCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty id should produce validation error")]
    public void Given_EmptyId_When_Validated_Then_HasError()
    {
        var command = new GetSaleCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
}
