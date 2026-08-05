using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Messaging.Consumers;

public class SaleCreatedConsumer(ILogger<SaleCreatedConsumer> logger) : IHandleMessages<SaleCreated>
{
    public Task Handle(SaleCreated message)
    {
        logger.LogInformation("SaleCreated received: {Id} Number:{Number}", message.Id, message.Number);
        return Task.CompletedTask;
    }
}
