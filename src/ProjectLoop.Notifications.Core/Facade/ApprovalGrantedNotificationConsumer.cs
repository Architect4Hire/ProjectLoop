using ProjectLoop.Contracts;

namespace ProjectLoop.Notifications.Core;

/// <summary>
/// Thin use-case boundary: delegates directly to the idempotent consumer
/// transaction. No notification content is composed or sent here.
/// </summary>
public sealed class ApprovalGrantedNotificationConsumer : IApprovalGrantedNotificationConsumer
{
    private readonly IApprovalGrantedNotificationConsumerTransaction _transaction;
    private readonly TimeProvider _timeProvider;

    public ApprovalGrantedNotificationConsumer(
        IApprovalGrantedNotificationConsumerTransaction transaction,
        TimeProvider? timeProvider = null)
    {
        _transaction = transaction;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public Task ConsumeAsync(IntegrationEventEnvelope<ApprovalGrantedV1> envelope, CancellationToken cancellationToken = default) =>
        _transaction.ExecuteAsync(envelope, _timeProvider.GetUtcNow(), cancellationToken);
}
