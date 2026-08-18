using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Contracts;
using ProjectLoop.Notifications.Core;

namespace ProjectLoop.Notifications.Functions;

/// <summary>
/// Thin Service Bus entry point for the ApprovalRejected notification
/// workflow: deserializes the envelope and delegates immediately to the
/// injected consumer. No email is sent and no NotificationsDbContext access
/// happens here.
/// </summary>
public sealed class ApprovalRejectedTrigger
{
    private readonly IApprovalRejectedNotificationConsumer _consumer;

    public ApprovalRejectedTrigger(IApprovalRejectedNotificationConsumer consumer)
    {
        _consumer = consumer;
    }

    [Function(nameof(ApprovalRejectedTrigger))]
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
