using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler(
    ISaleRepository saleRepository) : IRequestHandler<DeleteSaleCommand>
{
    public async Task Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (!await saleRepository.DeleteAsync(command.Id, cancellationToken))
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");
    }
}
