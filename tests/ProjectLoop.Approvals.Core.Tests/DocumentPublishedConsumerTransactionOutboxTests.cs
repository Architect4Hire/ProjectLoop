using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using ProjectLoop.Contracts;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class DocumentPublishedConsumerTransactionOutboxTests
{
    private static ApprovalsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApprovalsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static IntegrationEventEnvelope<DocumentPublishedV1> CreateEnvelope() => new(
        EventId: Guid.NewGuid(),
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
            Visibility: DocumentPublishedVisibility.Client,
            PublishedAtUtc: DateTimeOffset.UtcNow));

    [Fact]
    public async Task ExecuteAsync_Commits_The_New_Request_And_The_ApprovalRequested_Outbox_Row_Together()
    {
        await using var dbContext = CreateDbContext();
        var transaction = new DocumentPublishedConsumerTransaction(dbContext, new ApprovalRequestRepository(dbContext));
        var envelope = CreateEnvelope();

        var processedAtUtc = DateTimeOffset.UtcNow;
        await transaction.ExecuteAsync(envelope, processedAtUtc);

        var request = await dbContext.ApprovalRequests.SingleAsync();
        var outboxRow = await dbContext.OutboxMessages.SingleAsync(m => m.EventType == "ApprovalRequested");

        Assert.Equal(OutboxMessageStatus.Pending, outboxRow.Status);
        Assert.Equal(1, outboxRow.EventVersion);

        var requestedEnvelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalRequestedV1>>(outboxRow.Payload);
        Assert.NotNull(requestedEnvelope);
        Assert.Equal(request.Id, requestedEnvelope!.Data.ApprovalRequestId);
        Assert.Equal(request.TenantId, requestedEnvelope.TenantId);
        Assert.Equal(envelope.EventId.ToString(), requestedEnvelope.CausationId);
    }

    [Fact]
    public async Task ExecuteAsync_Duplicate_Delivery_Does_Not_Enqueue_A_Second_ApprovalRequested_Row()
    {
        await using var dbContext = CreateDbContext();
        var transaction = new DocumentPublishedConsumerTransaction(dbContext, new ApprovalRequestRepository(dbContext));
        var envelope = CreateEnvelope();

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);
        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);

        Assert.Equal(1, await dbContext.OutboxMessages.CountAsync(m => m.EventType == "ApprovalRequested"));
    }
}
