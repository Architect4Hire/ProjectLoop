using ProjectLoop.Notifications.Core;
using Xunit;

namespace ProjectLoop.Notifications.Core.Tests;

public class ApprovalRejectedNotificationMapperTests
{
    [Fact]
    public void Map_Produces_A_Notification_Addressed_To_The_Approver()
    {
        var data = new ApprovalRejectedV1(
            ApprovalDecisionId: Guid.NewGuid(),
            ApprovalRequestId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid(),
            TargetType: "DocumentVersion",
            TargetId: Guid.NewGuid(),
            TargetVersionId: Guid.NewGuid(),
            ApproverUserId: "approver-1",
            Comments: "Needs revision",
            DecidedAtUtc: DateTimeOffset.UtcNow);

        var notification = ApprovalRejectedNotificationMapper.Map(data);

        Assert.Equal(data.TenantId, notification.TenantId);
        Assert.Equal(data.ApproverUserId, notification.RecipientUserId);
        Assert.Contains(data.TargetId.ToString(), notification.Subject);
    }
}
