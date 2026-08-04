using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
