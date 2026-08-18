using Microsoft.EntityFrameworkCore;
using ProjectLoop.Contracts;
using ProjectLoop.Notifications.Core;
using Xunit;

namespace ProjectLoop.Notifications.Core.Tests;

public class ApprovalGrantedNotificationConsumerTransactionTests
{
    private sealed class RecordingEmailNotificationSender : IEmailNotificationSender
    {
        public int SendCount { get; private set; }

        public Task SendAsync(NotificationEnvelope notification, CancellationToken cancellationToken = default)
        {
            SendCount++;
            return Task.CompletedTask;
        }
    }

    private static NotificationsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static IntegrationEventEnvelope<ApprovalGrantedV1> CreateEnvelope(Guid? eventId = null) => new(
        EventId: eventId ?? Guid.NewGuid(),
        EventType: "ApprovalGranted",
        EventVersion: 1,
        OccurredAtUtc: DateTimeOffset.UtcNow,
        TenantId: Guid.NewGuid(),
        CorrelationId: "correlation-1",
        CausationId: null,
        TraceParent: null,
        Data: new ApprovalGrantedV1(
            ApprovalDecisionId: Guid.NewGuid(),
            ApprovalRequestId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid(),
            TargetType: "DocumentVersion",
            TargetId: Guid.NewGuid(),
            TargetVersionId: Guid.NewGuid(),
            ApproverUserId: "approver-1",
            Comments: null,
            DecidedAtUtc: DateTimeOffset.UtcNow));

    [Fact]
    public async Task ExecuteAsync_First_Delivery_Sends_And_Records_One_Notification_Delivery()
    {
        await using var dbContext = CreateDbContext();
        var sender = new RecordingEmailNotificationSender();
        var transaction = new ApprovalGrantedNotificationConsumerTransaction(dbContext, sender);
        var envelope = CreateEnvelope();

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);

        Assert.Equal(1, sender.SendCount);
        var delivery = await dbContext.NotificationDeliveries.SingleAsync();
        Assert.Equal(NotificationDeliveryStatus.Sent, delivery.Status);
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_Duplicate_Delivery_Does_Not_Send_Or_Record_A_Second_Notification()
    {
        await using var dbContext = CreateDbContext();
        var sender = new RecordingEmailNotificationSender();
        var transaction = new ApprovalGrantedNotificationConsumerTransaction(dbContext, sender);
        var envelope = CreateEnvelope();

        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);
        await transaction.ExecuteAsync(envelope, DateTimeOffset.UtcNow);

        Assert.Equal(1, sender.SendCount);
        Assert.Equal(1, await dbContext.NotificationDeliveries.CountAsync());
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }
}
