using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Messaging.Consumers;

public class SaleModifiedConsumer(ILogger<SaleModifiedConsumer> logger) : IHandleMessages<SaleModified>
{
    public Task Handle(SaleModified message)
    {
        logger.LogInformation("SaleModified received: {Id}", message.Id);
        return Task.CompletedTask;
    }
}
