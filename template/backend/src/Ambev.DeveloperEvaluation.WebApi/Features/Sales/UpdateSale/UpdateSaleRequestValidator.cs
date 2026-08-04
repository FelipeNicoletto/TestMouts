using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Number).GreaterThan(0);
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.Branch).NotEmpty();
        RuleFor(x => x.Products)
            .NotEmpty()
            .WithMessage("At least one product is required.");
        RuleForEach(x => x.Products).SetValidator(new UpdateSaleProductRequestValidator());
    }
}

public class UpdateSaleProductRequestValidator : AbstractValidator<UpdateSaleProductRequest>
{
    public UpdateSaleProductRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
