using ProjectLoop.Contracts;

namespace ProjectLoop.Notifications.Core;

/// <summary>
/// Owns the transaction boundary for idempotently applying one delivery of
/// the ApprovalGranted integration event: checking the inbox for the
/// message's identity, invoking the notification sender, recording the
/// delivery outcome, and marking inbox completion — all in a single atomic
/// unit of work so an at-least-once redelivery can never produce a
/// duplicate logical notification.
/// </summary>
public interface IApprovalGrantedNotificationConsumerTransaction
{
    Task ExecuteAsync(
        IntegrationEventEnvelope<ApprovalGrantedV1> envelope,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default);
}
