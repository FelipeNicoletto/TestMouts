using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    public Guid Id { get; set; }
    public long Number { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Branch { get; set; } = string.Empty;
    public SalesStatus Status { get; set; } = SalesStatus.Pending;
    public ICollection<UpdateSaleProductCommand> Products { get; set; } = [];
}

public class UpdateSaleProductCommand
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
