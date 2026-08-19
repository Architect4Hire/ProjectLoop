using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Audit.Core;
using ProjectLoop.Contracts;

namespace ProjectLoop.Audit.Functions;

/// <summary>
/// Thin Service Bus entry point for the ApprovalGranted audit workflow:
/// deserializes the envelope and delegates immediately to the injected
/// consumer. No AuditDbContext access happens here.
/// </summary>
public sealed class ApprovalGrantedAuditTrigger
{
    private readonly IApprovalGrantedAuditConsumer _consumer;

    public ApprovalGrantedAuditTrigger(IApprovalGrantedAuditConsumer consumer)
    {
        _consumer = consumer;
    }

    [Function(nameof(ApprovalGrantedAuditTrigger))]
    public Task RunAsync(
        [ServiceBusTrigger(
            "%ApprovalGrantedServiceBus:TopicName%",
            "%ApprovalGrantedServiceBus:AuditSubscriptionName%",
            Connection = "servicebus")]
        string message,
        CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalGrantedV1>>(message)
            ?? throw new InvalidOperationException("ApprovalGranted message payload could not be deserialized.");

        return _consumer.ConsumeAsync(envelope, cancellationToken);
    }
}
