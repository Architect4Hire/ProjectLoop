using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Audit.Core;
using ProjectLoop.Contracts;

namespace ProjectLoop.Audit.Functions;

/// <summary>
/// Thin Service Bus entry point for the ApprovalRequested audit workflow:
/// deserializes the envelope and delegates immediately to the injected
/// consumer. No AuditDbContext access happens here.
/// </summary>
public sealed class ApprovalRequestedAuditTrigger
{
    private readonly IApprovalRequestedAuditConsumer _consumer;

    public ApprovalRequestedAuditTrigger(IApprovalRequestedAuditConsumer consumer)
    {
        _consumer = consumer;
    }

    [Function(nameof(ApprovalRequestedAuditTrigger))]
    public Task RunAsync(
        [ServiceBusTrigger(
            "%ApprovalRequestedServiceBus:TopicName%",
            "%ApprovalRequestedServiceBus:AuditSubscriptionName%",
            Connection = "servicebus")]
        string message,
        CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalRequestedV1>>(message)
            ?? throw new InvalidOperationException("ApprovalRequested message payload could not be deserialized.");

        return _consumer.ConsumeAsync(envelope, cancellationToken);
    }
}
