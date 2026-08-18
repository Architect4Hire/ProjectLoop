using ProjectLoop.Notifications.Core;
using Xunit;

namespace ProjectLoop.Notifications.Core.Tests;

public class NotificationDeliveryTests
{
    [Fact]
    public void New_Delivery_Defaults_To_Pending_State_With_No_Attempts()
    {
        var delivery = new NotificationDelivery
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            SourceEventId = Guid.NewGuid(),
            NotificationType = "ApprovalRequested",
            RecipientUserId = "user-1",
            Status = NotificationDeliveryStatus.Pending,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        };

        Assert.Equal(NotificationDeliveryStatus.Pending, delivery.Status);
        Assert.Equal(0, delivery.AttemptCount);
        Assert.Null(delivery.SentAtUtc);
    }
}
