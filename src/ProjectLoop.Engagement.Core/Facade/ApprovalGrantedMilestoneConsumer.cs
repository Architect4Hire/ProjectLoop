using ProjectLoop.Contracts;

namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Thin use-case boundary: delegates directly to the idempotent consumer
/// transaction. No milestone state is inspected or mutated here.
/// </summary>
public sealed class ApprovalGrantedMilestoneConsumer : IApprovalGrantedMilestoneConsumer
{
    private readonly IApprovalGrantedMilestoneConsumerTransaction _transaction;
    private readonly TimeProvider _timeProvider;

    public ApprovalGrantedMilestoneConsumer(
        IApprovalGrantedMilestoneConsumerTransaction transaction,
        TimeProvider? timeProvider = null)
    {
        _transaction = transaction;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public Task ConsumeAsync(IntegrationEventEnvelope<ApprovalGrantedV1> envelope, CancellationToken cancellationToken = default) =>
        _transaction.ExecuteAsync(envelope, _timeProvider.GetUtcNow(), cancellationToken);
}
