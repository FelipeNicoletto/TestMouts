using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Application.Services;

public class DiscountCalculator : IDiscountCalculator
{
    private static decimal GetDiscountPercent(int quantity)
    {
        if (quantity > 20) throw new ArgumentOutOfRangeException(nameof(quantity), "Maximum 20 items per product is allowed.");
        if (quantity >= 10) return 0.20m;
        if (quantity >= 4) return 0.10m;
        return 0m;
    }

    private static decimal CalculateLine(decimal unitPrice, int quantity)
    {
        var pct = GetDiscountPercent(quantity);
        var gross = unitPrice * quantity;
        var discountAmount = Math.Round(gross * pct, 2, MidpointRounding.AwayFromZero);
        return discountAmount;
    }

    public void ApplyDiscounts(Sale sale)
    {
        decimal totalAmount = 0;
        foreach (var product in sale.Products)
        {
            var discountAmount = DiscountCalculator.CalculateLine(product.UnitPrice, product.Quantity);
            product.Discounts = discountAmount;
            totalAmount += product.TotalAmount;
        }

        sale.TotalAmount = totalAmount;
    }
}