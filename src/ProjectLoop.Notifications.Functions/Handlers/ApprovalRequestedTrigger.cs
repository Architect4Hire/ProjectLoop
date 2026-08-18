using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Contracts;
using ProjectLoop.Notifications.Core;

namespace ProjectLoop.Notifications.Functions;

/// <summary>
/// Thin Service Bus entry point for the ApprovalRequested notification
/// workflow: deserializes the envelope and delegates immediately to the
/// injected consumer. No email is sent and no NotificationsDbContext access
/// happens here.
/// </summary>
public sealed class ApprovalRequestedTrigger
{
    private readonly IApprovalRequestedNotificationConsumer _consumer;

    public ApprovalRequestedTrigger(IApprovalRequestedNotificationConsumer consumer)
    {
        _consumer = consumer;
    }

    [Function(nameof(ApprovalRequestedTrigger))]
    public Task RunAsync(
        [ServiceBusTrigger(
            "%ApprovalRequestedServiceBus:TopicName%",
            "%ApprovalRequestedServiceBus:SubscriptionName%",
            Connection = "servicebus")]
        string message,
        CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalRequestedV1>>(message)
            ?? throw new InvalidOperationException("ApprovalRequested message payload could not be deserialized.");

        return _consumer.ConsumeAsync(envelope, cancellationToken);
    }
}
