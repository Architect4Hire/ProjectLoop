using Microsoft.EntityFrameworkCore;
using ProjectLoop.Audit.Core;
using ProjectLoop.Contracts;
using Xunit;

namespace ProjectLoop.Audit.Core.Tests;

public class ApprovalRejectedAuditConsumerTransactionTests
{
    private static AuditDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AuditDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static IntegrationEventEnvelope<ApprovalRejectedV1> CreateEnvelope(Guid? eventId = null) => new(
        EventId: eventId ?? Guid.NewGuid(),
        EventType: "ApprovalRejected",
        EventVersion: 1,
        OccurredAtUtc: DateTimeOffset.UtcNow,
        TenantId: Guid.NewGuid(),
        CorrelationId: "correlation-1",
        CausationId: null,
        TraceParent: null,
        Data: new ApprovalRejectedV1(
            ApprovalDecisionId: Guid.NewGuid(),
            ApprovalRequestId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid(),
            TargetType: "Document",
            TargetId: Guid.NewGuid(),
            TargetVersionId: null,
            ApproverUserId: "approver-1",
            Comments: null,
            DecidedAtUtc: DateTimeOffset.UtcNow));

    [Fact]
    public async Task ExecuteAsync_First_Delivery_Appends_AuditRecord_And_Records_Inbox()
    {
        await using var dbContext = CreateDbContext();
        var transaction = new ApprovalRejectedAuditConsumerTransaction(dbContext, new AuditRecordRepository(dbContext));
        var envelope = CreateEnvelope();

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);

        Assert.Equal(1, await dbContext.AuditRecords.CountAsync());
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_Duplicate_Delivery_Does_Not_Append_A_Second_AuditRecord()
    {
        await using var dbContext = CreateDbContext();
        var transaction = new ApprovalRejectedAuditConsumerTransaction(dbContext, new AuditRecordRepository(dbContext));
        var envelope = CreateEnvelope();

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);
        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Equal(1, await dbContext.AuditRecords.CountAsync());
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }
}
