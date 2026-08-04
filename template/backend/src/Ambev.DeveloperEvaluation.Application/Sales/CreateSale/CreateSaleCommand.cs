using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommand : IRequest<CreateSaleResult>
{
    public long Number { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Branch { get; set; } = string.Empty;
    public SalesStatus Status { get; set; } = SalesStatus.Pending;
    public ICollection<CreateSaleProductCommand> Products { get; set; } = [];
}

public class CreateSaleProductCommand
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
