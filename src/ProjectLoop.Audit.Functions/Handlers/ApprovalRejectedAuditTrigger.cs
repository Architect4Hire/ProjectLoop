using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Audit.Core;
using ProjectLoop.Contracts;

namespace ProjectLoop.Audit.Functions;

/// <summary>
/// Thin Service Bus entry point for the ApprovalRejected audit workflow:
/// deserializes the envelope and delegates immediately to the injected
/// consumer. No AuditDbContext access happens here.
/// </summary>
public sealed class ApprovalRejectedAuditTrigger
{
    private readonly IApprovalRejectedAuditConsumer _consumer;

    public ApprovalRejectedAuditTrigger(IApprovalRejectedAuditConsumer consumer)
    {
        _consumer = consumer;
    }

    [Function(nameof(ApprovalRejectedAuditTrigger))]
    public Task RunAsync(
        [ServiceBusTrigger(
            "%ApprovalRejectedServiceBus:TopicName%",
            "%ApprovalRejectedServiceBus:AuditSubscriptionName%",
            Connection = "servicebus")]
        string message,
        CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalRejectedV1>>(message)
            ?? throw new InvalidOperationException("ApprovalRejected message payload could not be deserialized.");

        return _consumer.ConsumeAsync(envelope, cancellationToken);
    }
}
