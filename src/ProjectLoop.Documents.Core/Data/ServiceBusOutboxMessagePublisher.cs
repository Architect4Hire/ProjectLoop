using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace ProjectLoop.Documents.Core;

public sealed class ServiceBusOutboxMessagePublisher : IOutboxMessagePublisher
{
    private readonly ServiceBusSender _sender;

    public ServiceBusOutboxMessagePublisher(ServiceBusClient client, IOptions<OutboxServiceBusOptions> options)
        : this(client.CreateSender(options.Value.EntityName))
    {
    }

    public ServiceBusOutboxMessagePublisher(ServiceBusSender sender)
    {
        _sender = sender;
    }

    public Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default) =>
        _sender.SendMessageAsync(BuildMessage(message), cancellationToken);

    public static ServiceBusMessage BuildMessage(OutboxMessage message) => new(message.Payload)
    {
        MessageId = message.EventId.ToString(),
        Subject = message.EventType,
        ContentType = "application/json",
        CorrelationId = message.CorrelationId,
    };
}
