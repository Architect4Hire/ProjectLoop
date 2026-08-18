namespace ProjectLoop.Documents.Core;

public interface IOutboxMessageRepository
{
    /// <summary>
    /// Returns up to <paramref name="batchSize"/> Pending outbox rows,
    /// oldest first. Rows may already be mid-flight in another concurrent
    /// relay pass — the outbox-inbox invariant that every consumer is
    /// idempotent is what makes that safe, not a pessimistic lock here.
    /// </summary>
    Task<IReadOnlyList<OutboxMessage>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages the row's transition to Processed, but only when it is still
    /// Pending — a row already marked Processed (by this or a concurrent
    /// relay pass) is left untouched, so a late/duplicate mark can never
    /// turn a failed publish into a false success. Does not commit.
    /// </summary>
    void MarkProcessed(OutboxMessage message, DateTimeOffset processedAtUtc);
}
