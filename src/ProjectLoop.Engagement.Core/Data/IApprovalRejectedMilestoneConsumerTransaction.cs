using ProjectLoop.Contracts;

namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Owns the transaction boundary for idempotently applying one delivery of
/// the ApprovalRejected integration event: checking the inbox for the
/// message's identity, applying the eligible milestone transition when the
/// approval targets a milestone, and marking inbox completion — all in a
/// single atomic unit of work so an at-least-once redelivery can never
/// apply the transition twice.
/// </summary>
public interface IApprovalRejectedMilestoneConsumerTransaction
{
    Task ExecuteAsync(
        IntegrationEventEnvelope<ApprovalRejectedV1> envelope,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default);
}
