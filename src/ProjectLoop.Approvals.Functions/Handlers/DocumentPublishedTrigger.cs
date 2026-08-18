using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Approvals.Core;
using ProjectLoop.Contracts;

namespace ProjectLoop.Approvals.Functions;

/// <summary>
/// Thin Service Bus entry point for the DocumentPublished -> ApprovalRequested
/// workflow: deserializes the envelope and delegates immediately to the
/// injected consumer. No domain logic or direct ApprovalsDbContext access
/// happens here.
/// </summary>
public sealed class DocumentPublishedTrigger
{
    private readonly IDocumentPublishedConsumer _consumer;

    public DocumentPublishedTrigger(IDocumentPublishedConsumer consumer)
    {
        _consumer = consumer;
    }

    [Function(nameof(DocumentPublishedTrigger))]
    public Task RunAsync(
        [ServiceBusTrigger(
            "%DocumentPublishedServiceBus:TopicName%",
            "%DocumentPublishedServiceBus:SubscriptionName%",
            Connection = "servicebus")]
        string message,
        CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<DocumentPublishedV1>>(message)
            ?? throw new InvalidOperationException("DocumentPublished message payload could not be deserialized.");

        return _consumer.ConsumeAsync(envelope, cancellationToken);
    }
}
