using Microsoft.Extensions.Logging.Abstractions;
using ProjectLoop.Notifications.Core;
using Xunit;

namespace ProjectLoop.Notifications.Core.Tests;

public class DevelopmentEmailNotificationSenderTests
{
    [Fact]
    public async Task SendAsync_Completes_Without_An_Outbound_Call()
    {
        var sender = new DevelopmentEmailNotificationSender(NullLogger<DevelopmentEmailNotificationSender>.Instance);
        var notification = new NotificationEnvelope(Guid.NewGuid(), "user-1", "Subject", "Body");

        await sender.SendAsync(notification);
    }
}
