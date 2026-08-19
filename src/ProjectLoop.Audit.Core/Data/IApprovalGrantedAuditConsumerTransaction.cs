using ProjectLoop.Contracts;

namespace ProjectLoop.Audit.Core;

/// <summary>
/// Owns the transaction boundary for idempotently applying one delivery of
/// the ApprovalGranted integration event: checking the inbox for the
/// message's identity, appending the mapped AuditRecord, and marking inbox
/// completion — all in a single atomic unit of work so an at-least-once
/// redelivery can never produce a duplicate audit record.
/// </summary>
public interface IApprovalGrantedAuditConsumerTransaction
{
    Task ExecuteAsync(
        IntegrationEventEnvelope<ApprovalGrantedV1> envelope,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default);
}
