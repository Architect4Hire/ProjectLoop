using Microsoft.EntityFrameworkCore;
using ProjectLoop.Contracts;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class ApprovalGrantedMilestoneConsumerTransactionTests
{
    private static EngagementDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<EngagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Milestone CreateMilestone(Guid tenantId, Guid projectId, MilestoneStatus status) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        ProjectId = projectId,
        Name = "Kickoff",
        Status = status,
        CreatedAtUtc = DateTimeOffset.UtcNow,
        UpdatedAtUtc = DateTimeOffset.UtcNow,
    };

    private static IntegrationEventEnvelope<ApprovalGrantedV1> CreateEnvelope(Milestone milestone, Guid? eventId = null) => new(
        EventId: eventId ?? Guid.NewGuid(),
        EventType: "ApprovalGranted",
        EventVersion: 1,
        OccurredAtUtc: DateTimeOffset.UtcNow,
        TenantId: milestone.TenantId,
        CorrelationId: "correlation-1",
        CausationId: null,
        TraceParent: null,
        Data: new ApprovalGrantedV1(
            ApprovalDecisionId: Guid.NewGuid(),
            ApprovalRequestId: Guid.NewGuid(),
            TenantId: milestone.TenantId,
            ProjectId: milestone.ProjectId,
            TargetType: "Milestone",
            TargetId: milestone.Id,
            TargetVersionId: null,
            ApproverUserId: "approver-1",
            Comments: null,
            DecidedAtUtc: DateTimeOffset.UtcNow));

    [Fact]
    public async Task ExecuteAsync_First_Delivery_Applies_Eligible_Transition_And_Records_Inbox()
    {
        await using var dbContext = CreateDbContext();
        var milestone = CreateMilestone(Guid.NewGuid(), Guid.NewGuid(), MilestoneStatus.InProgress);
        dbContext.Milestones.Add(milestone);
        await dbContext.SaveChangesAsync();

        var transaction = new ApprovalGrantedMilestoneConsumerTransaction(dbContext);
        var envelope = CreateEnvelope(milestone);

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);

        var updated = await dbContext.Milestones.SingleAsync(m => m.Id == milestone.Id);
        Assert.Equal(MilestoneStatus.Completed, updated.Status);
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_Duplicate_Delivery_Does_Not_Reapply_The_Milestone_Transition()
    {
        await using var dbContext = CreateDbContext();
        var milestone = CreateMilestone(Guid.NewGuid(), Guid.NewGuid(), MilestoneStatus.InProgress);
        dbContext.Milestones.Add(milestone);
        await dbContext.SaveChangesAsync();

        var transaction = new ApprovalGrantedMilestoneConsumerTransaction(dbContext);
        var envelope = CreateEnvelope(milestone);

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);
        var firstUpdatedAtUtc = (await dbContext.Milestones.SingleAsync(m => m.Id == milestone.Id)).UpdatedAtUtc;

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow.AddMinutes(1));

        var updated = await dbContext.Milestones.SingleAsync(m => m.Id == milestone.Id);
        Assert.Equal(MilestoneStatus.Completed, updated.Status);
        Assert.Equal(firstUpdatedAtUtc, updated.UpdatedAtUtc);
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }
}
