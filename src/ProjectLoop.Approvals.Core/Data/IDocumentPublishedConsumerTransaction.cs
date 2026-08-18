using ProjectLoop.Contracts;

namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Owns the transaction boundary for idempotently applying one delivery of
/// the DocumentPublished integration event: checking the inbox for the
/// message's identity, conditionally creating an ApprovalRequest when policy
/// requires one, and recording inbox completion — all in a single atomic
/// unit of work so an at-least-once redelivery can never produce a duplicate
/// approval.
/// </summary>
public interface IDocumentPublishedConsumerTransaction
{
    Task ExecuteAsync(
        IntegrationEventEnvelope<DocumentPublishedV1> envelope,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default);
}
