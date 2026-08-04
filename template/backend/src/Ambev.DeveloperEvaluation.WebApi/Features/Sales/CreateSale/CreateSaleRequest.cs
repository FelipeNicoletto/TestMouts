using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequest
{
    public long Number { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Branch { get; set; } = string.Empty;
    public SalesStatus Status { get; set; } = SalesStatus.Pending;
    public ICollection<CreateSaleProductRequest> Products { get; set; } = [];
}

public class CreateSaleProductRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
