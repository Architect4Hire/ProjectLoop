using ProjectLoop.Contracts;

namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Use-case boundary for the DocumentPublished consumer: delegates directly
/// to the idempotent consumer transaction. There is no tenant-context
/// derivation here — the caller is the Documents service via Service Bus,
/// not an authenticated end user, so tenant scoping instead comes from the
/// event's own TenantId.
/// </summary>
public sealed class DocumentPublishedConsumer : IDocumentPublishedConsumer
{
    private readonly IDocumentPublishedConsumerTransaction _transaction;
    private readonly TimeProvider _timeProvider;

    public DocumentPublishedConsumer(
        IDocumentPublishedConsumerTransaction transaction,
        TimeProvider? timeProvider = null)
    {
        _transaction = transaction;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public Task ConsumeAsync(IntegrationEventEnvelope<DocumentPublishedV1> envelope, CancellationToken cancellationToken = default) =>
        _transaction.ExecuteAsync(envelope, _timeProvider.GetUtcNow(), cancellationToken);
}
