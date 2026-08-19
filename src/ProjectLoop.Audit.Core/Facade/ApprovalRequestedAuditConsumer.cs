using ProjectLoop.Contracts;

namespace ProjectLoop.Audit.Core;

/// <summary>
/// Thin use-case boundary: delegates directly to the idempotent consumer
/// transaction. No AuditDb access happens here.
/// </summary>
public sealed class ApprovalRequestedAuditConsumer : IApprovalRequestedAuditConsumer
{
    private readonly IApprovalRequestedAuditConsumerTransaction _transaction;
    private readonly TimeProvider _timeProvider;

    public ApprovalRequestedAuditConsumer(
        IApprovalRequestedAuditConsumerTransaction transaction,
        TimeProvider? timeProvider = null)
    {
        _transaction = transaction;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public Task ConsumeAsync(IntegrationEventEnvelope<ApprovalRequestedV1> envelope, CancellationToken cancellationToken = default) =>
        _transaction.ExecuteAsync(envelope, _timeProvider.GetUtcNow(), cancellationToken);
}
