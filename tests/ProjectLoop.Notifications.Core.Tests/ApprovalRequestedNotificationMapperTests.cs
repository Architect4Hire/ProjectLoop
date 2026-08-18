using ProjectLoop.Notifications.Core;
using Xunit;

namespace ProjectLoop.Notifications.Core.Tests;

public class ApprovalRequestedNotificationMapperTests
{
    [Fact]
    public void Map_Produces_A_Notification_Addressed_To_The_Requester()
    {
        var data = new ApprovalRequestedV1(
            ApprovalRequestId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid(),
            TargetType: "DocumentVersion",
            TargetId: Guid.NewGuid(),
            TargetVersionId: Guid.NewGuid(),
            RequestedByUserId: "user-1",
            RequestedAtUtc: DateTimeOffset.UtcNow);

        var notification = ApprovalRequestedNotificationMapper.Map(data);

        Assert.Equal(data.TenantId, notification.TenantId);
        Assert.Equal(data.RequestedByUserId, notification.RecipientUserId);
        Assert.Contains(data.TargetId.ToString(), notification.Subject);
    }
}
