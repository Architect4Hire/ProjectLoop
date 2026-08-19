using ProjectLoop.Contracts;

namespace ProjectLoop.Audit.Core;

/// <summary>
/// Thin use-case boundary: delegates directly to the idempotent consumer
/// transaction. No AuditDb access happens here. No business state outside
/// Audit's own database is read or written.
/// </summary>
public sealed class ApprovalRejectedAuditConsumer : IApprovalRejectedAuditConsumer
{
    private readonly IApprovalRejectedAuditConsumerTransaction _transaction;
    private readonly TimeProvider _timeProvider;

    public ApprovalRejectedAuditConsumer(
        IApprovalRejectedAuditConsumerTransaction transaction,
        TimeProvider? timeProvider = null)
    {
        _transaction = transaction;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public Task ConsumeAsync(IntegrationEventEnvelope<ApprovalRejectedV1> envelope, CancellationToken cancellationToken = default) =>
        _transaction.ExecuteAsync(envelope, _timeProvider.GetUtcNow(), cancellationToken);
}
