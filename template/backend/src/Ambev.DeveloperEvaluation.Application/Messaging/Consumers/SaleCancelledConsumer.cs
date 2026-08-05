using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Messaging.Consumers;

public class SaleCancelledConsumer(ILogger<SaleCancelledConsumer> logger) : IHandleMessages<SaleCancelled>
{
    public Task Handle(SaleCancelled message)
    {
        logger.LogInformation("SaleCancelled received: {Id}", message.Id);
        return Task.CompletedTask;
    }
}
