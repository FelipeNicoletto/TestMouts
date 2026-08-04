using Ambev.DeveloperEvaluation.Application.Messaging;
using MassTransit;

namespace Ambev.DeveloperEvaluation.WebApi.Messaging.Consumers;

public class SaleCreatedConsumer : IConsumer<SaleCreated>
{
    private readonly ILogger<SaleCreatedConsumer> _logger;

    public SaleCreatedConsumer(ILogger<SaleCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<SaleCreated> context)
    {
        _logger.LogInformation("SaleCreated received: {Id} Number:{Number}", context.Message.Id, context.Message.Number);
        return Task.CompletedTask;
    }
}
