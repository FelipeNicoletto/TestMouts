using Ambev.DeveloperEvaluation.Application.Messaging;
using MassTransit;

namespace Ambev.DeveloperEvaluation.WebApi.Messaging.Consumers;

public class SaleCancelledConsumer : IConsumer<SaleCancelled>
{
    private readonly ILogger<SaleCancelledConsumer> _logger;

    public SaleCancelledConsumer(ILogger<SaleCancelledConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<SaleCancelled> context)
    {
        _logger.LogInformation("SaleCancelled received: {Id}", context.Message.Id);
        return Task.CompletedTask;
    }
}
