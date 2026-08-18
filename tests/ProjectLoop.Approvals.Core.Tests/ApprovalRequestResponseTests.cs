using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalRequestResponseTests
{
    [Fact]
    public void ApprovalRequestResponse_Can_Be_Created_With_Required_Members()
    {
        var response = new ApprovalRequestResponse
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            TargetType = "DocumentVersion",
            TargetId = Guid.NewGuid(),
            TargetVersionId = Guid.NewGuid(),
            Status = ApprovalRequestStatus.Pending,
            RequestedByUserId = "user-1",
            RequestedAtUtc = DateTimeOffset.UtcNow,
        };

        Assert.Equal(ApprovalRequestStatus.Pending, response.Status);
        Assert.NotNull(response.TargetVersionId);
    }

    [Fact]
    public void ApprovalRequestResponse_Never_Exposes_TenantId()
    {
        var propertyNames = typeof(ApprovalRequestResponse)
            .GetProperties()
            .Select(p => p.Name);

        Assert.DoesNotContain(nameof(ApprovalRequest.TenantId), propertyNames);
    }
}
