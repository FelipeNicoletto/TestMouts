using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface IDiscountCalculator
{
    void ApplyDiscounts(Sale sale);
}
