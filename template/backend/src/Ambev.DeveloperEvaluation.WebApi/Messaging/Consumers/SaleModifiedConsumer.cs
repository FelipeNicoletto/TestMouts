using Ambev.DeveloperEvaluation.Application.Messaging;
using MassTransit;

namespace Ambev.DeveloperEvaluation.WebApi.Messaging.Consumers;

public class SaleModifiedConsumer : IConsumer<SaleModified>
{
    private readonly ILogger<SaleModifiedConsumer> _logger;

    public SaleModifiedConsumer(ILogger<SaleModifiedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<SaleModified> context)
    {
        _logger.LogInformation("SaleModified received: {Id}", context.Message.Id);
        return Task.CompletedTask;
    }
}
