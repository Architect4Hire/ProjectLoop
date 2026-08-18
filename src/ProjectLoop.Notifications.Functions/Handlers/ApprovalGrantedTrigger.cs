using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Contracts;
using ProjectLoop.Notifications.Core;

namespace ProjectLoop.Notifications.Functions;

/// <summary>
/// Thin Service Bus entry point for the ApprovalGranted notification
/// workflow: deserializes the envelope and delegates immediately to the
/// injected consumer. No email is sent and no NotificationsDbContext access
/// happens here.
/// </summary>
public sealed class ApprovalGrantedTrigger
{
    private readonly IApprovalGrantedNotificationConsumer _consumer;

    public ApprovalGrantedTrigger(IApprovalGrantedNotificationConsumer consumer)
    {
        _consumer = consumer;
    }

    [Function(nameof(ApprovalGrantedTrigger))]
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
