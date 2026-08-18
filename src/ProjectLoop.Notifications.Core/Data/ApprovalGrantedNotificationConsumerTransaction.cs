using Microsoft.EntityFrameworkCore;
using ProjectLoop.Contracts;

namespace ProjectLoop.Notifications.Core;

public sealed class ApprovalGrantedNotificationConsumerTransaction : IApprovalGrantedNotificationConsumerTransaction
{
    private const string NotificationType = "ApprovalGranted";

    private readonly NotificationsDbContext _dbContext;
    private readonly IEmailNotificationSender _sender;

    public ApprovalGrantedNotificationConsumerTransaction(
        NotificationsDbContext dbContext,
        IEmailNotificationSender sender)
    {
        _dbContext = dbContext;
        _sender = sender;
    }

    public async Task ExecuteAsync(
        IntegrationEventEnvelope<ApprovalGrantedV1> envelope,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _dbContext.InboxMessages
            .AnyAsync(m => m.MessageId == envelope.EventId, cancellationToken);
        if (alreadyProcessed)
        {
            // At-least-once redelivery of a message this consumer already
            // applied. Returning without touching NotificationDeliveries or
            // Engagement/milestone state is what keeps a duplicate
            // ApprovalGranted delivery from ever producing a duplicate
            // logical notification.
            return;
        }

        var notification = ApprovalGrantedNotificationMapper.Map(envelope.Data);

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
            // never roll back the approval decision that produced this
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

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
