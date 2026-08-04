using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleProductTests
{
    [Fact(DisplayName = "New sale product should have zero total amount by default")]
    public void NewSaleProduct_Defaults_TotalAmountZero()
    {
        var sp = new SaleProduct();

        sp.TotalAmount.Should().Be(0);
        sp.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        sp.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact(DisplayName = "TotalAmount is calculated when Quantity and UnitPrice are set")]
    public void When_SetQuantityAndUnitPrice_TotalAmountCalculated()
    {
        var sp = new SaleProduct();

        sp.Quantity = 3;
        sp.UnitPrice = 10m;

        sp.TotalAmount.Should().Be(30m);
    }

    [Fact(DisplayName = "TotalAmount updates when Discounts change")]
    public void When_DiscountsChange_TotalAmountRecalculated()
    {
        var sp = new SaleProduct { Quantity = 2, UnitPrice = 7.5m };

        sp.TotalAmount.Should().Be(15m);

        sp.Discounts = 5m;

        sp.TotalAmount.Should().Be(10m);
    }

    [Fact(DisplayName = "TotalAmount updates when UnitPrice changes after Quantity set")]
    public void When_UnitPriceChanged_AfterQuantity_TotalAmountRecalculated()
    {
        var sp = new SaleProduct { Quantity = 4 };

        sp.TotalAmount.Should().Be(0m);

        sp.UnitPrice = 2.5m;

        sp.TotalAmount.Should().Be(10m);
    }
}
