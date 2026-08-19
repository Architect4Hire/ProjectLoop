using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Contracts;
using ProjectLoop.Engagement.Core;

namespace ProjectLoop.Engagement.Functions;

/// <summary>
/// Thin Service Bus entry point for the ApprovalGranted milestone
/// propagation workflow: deserializes the envelope and delegates
/// immediately to the injected consumer. No milestone state is read or
/// mutated here.
/// </summary>
public sealed class ApprovalGrantedMilestoneTrigger
{
    private readonly IApprovalGrantedMilestoneConsumer _consumer;

    public ApprovalGrantedMilestoneTrigger(IApprovalGrantedMilestoneConsumer consumer)
    {
        _consumer = consumer;
    }

    [Function(nameof(ApprovalGrantedMilestoneTrigger))]
    public Task RunAsync(
        [ServiceBusTrigger(
            "%ApprovalGrantedServiceBus:TopicName%",
            "%ApprovalGrantedServiceBus:SubscriptionName%",
            Connection = "servicebus")]
        string message,
        CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalGrantedV1>>(message)
            ?? throw new InvalidOperationException("ApprovalGranted message payload could not be deserialized.");

        return _consumer.ConsumeAsync(envelope, cancellationToken);
    }
}
