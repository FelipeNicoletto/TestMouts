using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using MediatR;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Application.Messaging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler(
    ISaleRepository saleRepository,
    IDiscountCalculator discountCalculator,
    IMapper mapper,
    IBus bus) : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator(saleRepository);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = mapper.Map<Sale>(command);

        discountCalculator.ApplyDiscounts(sale);

        await saleRepository.CreateAsync(sale, cancellationToken);

        await bus.Send(new SaleCreated
        {
            Id = sale.Id,
            Number = sale.Number
        });

        return mapper.Map<CreateSaleResult>(sale);
    }
}
