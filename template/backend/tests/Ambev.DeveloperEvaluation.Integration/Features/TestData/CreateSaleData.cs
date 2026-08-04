using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

namespace Ambev.DeveloperEvaluation.Integration.Features.TestData;

public static class CreateSaleData
{
    public static CreateSaleRequest ValidCreateSaleRequest(long number)
    {
        return new CreateSaleRequest
        {
            Number = number,
            CustomerName = "John Doe",
            Date = DateTime.UtcNow,
            Branch = "Branch A",
            Status = SalesStatus.Pending,
            Products =
            [
                new CreateSaleProductRequest
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 10.5m
                },
                new CreateSaleProductRequest
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 15,
                    UnitPrice = 8.5m
                }
            ]
        };
    }
}
