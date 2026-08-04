using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    private readonly ISaleRepository _saleRepository;

    public CreateSaleCommandValidator(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;

        RuleFor(x => x.Number)
            .GreaterThan(0)
            .MustAsync(NumberUniqueAsync)
            .WithMessage("Sale number must be unique.");
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.Branch).NotEmpty();
        RuleForEach(x => x.Products).SetValidator(new CreateSaleProductCommandValidator());
        RuleFor(x => x.Products)
            .NotEmpty()
            .WithMessage("At least one product is required.")
            .Must(x => x.GroupBy(p => p.ProductId).Any(g => g.Count() == 1))
            .WithMessage("Each product can only be added once.")
            .Must(x => x.Any(p => p.Quantity <= 20))
            .WithMessage("Each product can have a maximum of 20 units.");
    }

    private async Task<bool> NumberUniqueAsync(long number, CancellationToken cancellationToken)
    {
        var exists = await _saleRepository.SaleExistsAsync(number, cancellationToken: cancellationToken);
        return !exists;
    }
}

public class CreateSaleProductCommandValidator : AbstractValidator<CreateSaleProductCommand>
{
    public CreateSaleProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
