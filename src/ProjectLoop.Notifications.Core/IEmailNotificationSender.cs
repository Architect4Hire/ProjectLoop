namespace ProjectLoop.Notifications.Core;

/// <summary>
/// Provider-neutral contract for delivering one email notification. No
/// production provider is chosen here — implementations (development,
/// SMTP, a managed email API, etc.) are supplied independently via DI.
/// </summary>
public interface IEmailNotificationSender
{
    Task SendAsync(NotificationEnvelope notification, CancellationToken cancellationToken = default);
}
