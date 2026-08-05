using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using MediatR;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Application.Messaging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler(
    ISaleRepository saleRepository,
    IDiscountCalculator discountCalculator,
    IMapper mapper,
    IBus bus) : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator(saleRepository);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale is null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        var isCancelled =
            command.Status == Domain.Enums.SalesStatus.Cancelled &&
            sale.Status != Domain.Enums.SalesStatus.Cancelled;

        sale = mapper.Map(command, sale);

        discountCalculator.ApplyDiscounts(sale);

        await saleRepository.UpdateAsync(sale, cancellationToken);

        await bus.Send(new SaleModified
        {
            Id = sale.Id,
            Number = sale.Number
        });

        if (isCancelled)
            await bus.Send(new SaleCancelled
            {
                Id = sale.Id,
                Number = sale.Number
            });

        return mapper.Map<UpdateSaleResult>(sale);
    }
}
