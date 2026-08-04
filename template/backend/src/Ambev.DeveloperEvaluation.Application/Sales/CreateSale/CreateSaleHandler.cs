using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using MediatR;
using MassTransit;
using Ambev.DeveloperEvaluation.Application.Messaging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler(
    ISaleRepository saleRepository,
    IDiscountCalculator discountCalculator,
    IMapper mapper,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CreateSaleCommand, CreateSaleResult>
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

        await publishEndpoint.Publish(new SaleCreated
        {
            Id = sale.Id,
            Number = sale.Number
        }, cancellationToken);

        return mapper.Map<CreateSaleResult>(sale);
    }
}
