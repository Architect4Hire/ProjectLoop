using Microsoft.EntityFrameworkCore;
using ProjectLoop.Contracts;

namespace ProjectLoop.Engagement.Core;

public sealed class ApprovalRejectedMilestoneConsumerTransaction : IApprovalRejectedMilestoneConsumerTransaction
{
    private readonly EngagementDbContext _dbContext;

    public ApprovalRejectedMilestoneConsumerTransaction(EngagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(
        IntegrationEventEnvelope<ApprovalRejectedV1> envelope,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _dbContext.InboxMessages
            .AnyAsync(m => m.MessageId == envelope.EventId, cancellationToken);
        if (alreadyProcessed)
        {
            // At-least-once redelivery of a message this consumer already
            // applied. Returning without touching Milestones or Approvals
            // history is what keeps a duplicate ApprovalRejected delivery
            // from ever applying the same transition twice.
            return;
        }

        var data = envelope.Data;

        var milestone = await _dbContext.Milestones
            .FirstOrDefaultAsync(
                m => m.TenantId == data.TenantId && m.Id == data.TargetId,
                cancellationToken);

        if (milestone is not null && MilestoneApprovalTargetMatcher.TargetsMilestone(data.TargetType, data.TargetId, milestone.Id))
        {
            var eligibleStatus = MilestoneApprovalTargetMatcher.GetEligibleStatusOnRejected(milestone.Status);
            if (eligibleStatus is not null)
            {
                milestone.Status = eligibleStatus.Value;
                milestone.UpdatedAtUtc = processedAtUtc;
            }
        }

        _dbContext.InboxMessages.Add(new InboxMessage
        {
            Id = Guid.NewGuid(),
            MessageId = envelope.EventId,
            EventType = envelope.EventType,
            CorrelationId = envelope.CorrelationId,
            ProcessedAtUtc = processedAtUtc,
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
