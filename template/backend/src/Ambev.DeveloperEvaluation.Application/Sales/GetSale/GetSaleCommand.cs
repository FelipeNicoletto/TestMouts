using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleCommand(Guid id) : IRequest<GetSaleResult>
{
    public Guid Id { get; } = id;
}
