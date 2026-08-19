using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Contracts;
using ProjectLoop.Engagement.Core;

namespace ProjectLoop.Engagement.Functions;

/// <summary>
/// Thin Service Bus entry point for the ApprovalRejected milestone
/// propagation workflow: deserializes the envelope and delegates
/// immediately to the injected consumer. No milestone state is read or
/// mutated here.
/// </summary>
public sealed class ApprovalRejectedMilestoneTrigger
{
    private readonly IApprovalRejectedMilestoneConsumer _consumer;

    public ApprovalRejectedMilestoneTrigger(IApprovalRejectedMilestoneConsumer consumer)
    {
        _consumer = consumer;
    }

    [Function(nameof(ApprovalRejectedMilestoneTrigger))]
    public Task RunAsync(
        [ServiceBusTrigger(
            "%ApprovalRejectedServiceBus:TopicName%",
            "%ApprovalRejectedServiceBus:SubscriptionName%",
            Connection = "servicebus")]
        string message,
        CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalRejectedV1>>(message)
            ?? throw new InvalidOperationException("ApprovalRejected message payload could not be deserialized.");

        return _consumer.ConsumeAsync(envelope, cancellationToken);
    }
}
