using Microsoft.Extensions.Logging;

namespace ProjectLoop.Notifications.Core;

/// <summary>
/// Local/development sender for the Aspire baseline: no outbound network
/// call, just a structured log line, so the notification pipeline can be
/// exercised end to end without a configured production email provider.
/// </summary>
public sealed class DevelopmentEmailNotificationSender : IEmailNotificationSender
{
    private readonly ILogger<DevelopmentEmailNotificationSender> _logger;

    public DevelopmentEmailNotificationSender(ILogger<DevelopmentEmailNotificationSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(NotificationEnvelope notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Development notification: tenant {TenantId} recipient {RecipientUserId} subject {Subject}",
            notification.TenantId,
            notification.RecipientUserId,
            notification.Subject);

        return Task.CompletedTask;
    }
}
