using Microsoft.EntityFrameworkCore;
using ProjectLoop.Contracts;

namespace ProjectLoop.Notifications.Core;

public sealed class ApprovalRequestedNotificationConsumerTransaction : IApprovalRequestedNotificationConsumerTransaction
{
    private const string NotificationType = "ApprovalRequested";

    private readonly NotificationsDbContext _dbContext;
    private readonly IEmailNotificationSender _sender;

    public ApprovalRequestedNotificationConsumerTransaction(
        NotificationsDbContext dbContext,
        IEmailNotificationSender sender)
    {
        _dbContext = dbContext;
        _sender = sender;
    }

    public async Task ExecuteAsync(
        IntegrationEventEnvelope<ApprovalRequestedV1> envelope,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _dbContext.InboxMessages
            .AnyAsync(m => m.MessageId == envelope.EventId, cancellationToken);
        if (alreadyProcessed)
        {
            // At-least-once redelivery of a message this consumer already
            // applied. Returning without touching NotificationDeliveries is
            // what keeps a duplicate ApprovalRequested delivery from ever
            // producing a duplicate logical notification.
            return;
        }

        var notification = ApprovalRequestedNotificationMapper.Map(envelope.Data);

        var delivery = new NotificationDelivery
        {
            Id = Guid.NewGuid(),
            TenantId = envelope.Data.TenantId,
            SourceEventId = envelope.EventId,
            NotificationType = NotificationType,
            RecipientUserId = notification.RecipientUserId,
            CorrelationId = envelope.CorrelationId,
            Status = NotificationDeliveryStatus.Pending,
            AttemptCount = 1,
            CreatedAtUtc = processedAtUtc,
            LastAttemptedAtUtc = processedAtUtc,
        };

        try
        {
            await _sender.SendAsync(notification, cancellationToken);
            delivery.Status = NotificationDeliveryStatus.Sent;
            delivery.SentAtUtc = processedAtUtc;
        }
        catch (Exception ex)
        {
            // Notification failure is recorded, not propagated: it must
            // never roll back the approval transaction that produced this
            // event, and it already happened asynchronously and
            // independently of it.
            delivery.Status = NotificationDeliveryStatus.Failed;
            delivery.LastError = ex.Message;
        }

        _dbContext.NotificationDeliveries.Add(delivery);

        _dbContext.InboxMessages.Add(new InboxMessage
        {
            Id = Guid.NewGuid(),
            MessageId = envelope.EventId,
            EventType = envelope.EventType,
            CorrelationId = envelope.CorrelationId,
            ProcessedAtUtc = processedAtUtc,
        });

        // A single SaveChangesAsync call commits the inbox completion
        // record together with the NotificationDelivery outcome as one
        // atomic unit of work.
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
