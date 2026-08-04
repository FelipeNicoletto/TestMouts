using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.Number).GreaterThan(0);
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.Branch).NotEmpty();
        RuleFor(x => x.Products)
            .NotEmpty()
            .WithMessage("At least one product is required.");
        RuleForEach(x => x.Products).SetValidator(new CreateSaleProductRequestValidator());
    }
}

public class CreateSaleProductRequestValidator : AbstractValidator<CreateSaleProductRequest>
{
    public CreateSaleProductRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
