using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectLoop.Contracts;
using ProjectLoop.Notifications.Core;
using Xunit;

namespace ProjectLoop.Notifications.Functions.Tests;

/// <summary>
/// End-to-end proof, through the real trigger, facade consumer and
/// transaction (not a fake), that an at-least-once Service Bus redelivery
/// of the same ApprovalRequested message never produces a second logical
/// notification delivery.
/// </summary>
public class ApprovalRequestedNotificationDuplicateDeliveryTests
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

    [Fact]
    public async Task RunAsync_Replayed_Message_Does_Not_Create_A_Second_Notification_Delivery()
    {
        await using var dbContext = new NotificationsDbContext(
            new DbContextOptionsBuilder<NotificationsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
        var sender = new RecordingEmailNotificationSender();
        var transaction = new ApprovalRequestedNotificationConsumerTransaction(dbContext, sender);
        var consumer = new ApprovalRequestedNotificationConsumer(transaction);
        var trigger = new ApprovalRequestedTrigger(consumer);

        var envelope = new IntegrationEventEnvelope<ApprovalRequestedV1>(
            EventId: Guid.NewGuid(),
            EventType: "ApprovalRequested",
            EventVersion: 1,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            TenantId: Guid.NewGuid(),
            CorrelationId: "correlation-1",
            CausationId: null,
            TraceParent: null,
            Data: new ApprovalRequestedV1(
                ApprovalRequestId: Guid.NewGuid(),
                TenantId: Guid.NewGuid(),
                ProjectId: Guid.NewGuid(),
                TargetType: "DocumentVersion",
                TargetId: Guid.NewGuid(),
                TargetVersionId: Guid.NewGuid(),
                RequestedByUserId: "user-1",
                RequestedAtUtc: DateTimeOffset.UtcNow));
        var message = JsonSerializer.Serialize(envelope);

        await trigger.RunAsync(message, cancellationToken: default);
        await trigger.RunAsync(message, cancellationToken: default);

        Assert.Equal(1, sender.SendCount);
        Assert.Equal(1, await dbContext.NotificationDeliveries.CountAsync());
        Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
    }
}
