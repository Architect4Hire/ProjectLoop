using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using ProjectLoop.Contracts;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class DocumentPublishedConsumerTransactionTests
{
    private static ApprovalsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApprovalsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static IntegrationEventEnvelope<DocumentPublishedV1> CreateEnvelope(
        DocumentPublishedVisibility visibility,
        Guid? eventId = null) => new(
        EventId: eventId ?? Guid.NewGuid(),
        EventType: "DocumentPublished",
        EventVersion: 1,
        OccurredAtUtc: DateTimeOffset.UtcNow,
        TenantId: Guid.NewGuid(),
        CorrelationId: "correlation-1",
        CausationId: null,
        TraceParent: null,
        Data: new DocumentPublishedV1(
            DocumentId: Guid.NewGuid(),
            DocumentVersionId: Guid.NewGuid(),
            VersionNumber: 1,
            ProjectId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            Category: "Contract",
            Visibility: visibility,
            PublishedAtUtc: DateTimeOffset.UtcNow));

    [Fact]
    public async Task ExecuteAsync_First_Delivery_Requiring_Approval_Creates_One_Pending_Request()
    {
        await using var dbContext = CreateDbContext();
        var transaction = new DocumentPublishedConsumerTransaction(dbContext, new ApprovalRequestRepository(dbContext));
        var envelope = CreateEnvelope(DocumentPublishedVisibility.Client);

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);

        var request = await dbContext.ApprovalRequests.SingleAsync();
        Assert.Equal(ApprovalRequestStatus.Pending, request.Status);
        Assert.Equal(envelope.Data.DocumentId, request.TargetId);
        Assert.Equal(envelope.Data.DocumentVersionId, request.TargetVersionId);
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_Duplicate_Delivery_Does_Not_Create_A_Second_Approval_Request()
    {
        await using var dbContext = CreateDbContext();
        var transaction = new DocumentPublishedConsumerTransaction(dbContext, new ApprovalRequestRepository(dbContext));
        var envelope = CreateEnvelope(DocumentPublishedVisibility.Client);

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);
        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);

        Assert.Equal(1, await dbContext.ApprovalRequests.CountAsync());
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_Internal_Visibility_Records_Inbox_Completion_Without_Creating_A_Request()
    {
        await using var dbContext = CreateDbContext();
        var transaction = new DocumentPublishedConsumerTransaction(dbContext, new ApprovalRequestRepository(dbContext));
        var envelope = CreateEnvelope(DocumentPublishedVisibility.Internal);

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);

        Assert.False(await dbContext.ApprovalRequests.AnyAsync());
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }
}
