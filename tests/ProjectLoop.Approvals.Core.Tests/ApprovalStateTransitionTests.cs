using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalStateTransitionTests
{
    [Theory]
    [InlineData(ApprovalRequestStatus.Pending, ApprovalRequestStatus.Approved, true)]
    [InlineData(ApprovalRequestStatus.Pending, ApprovalRequestStatus.Rejected, true)]
    [InlineData(ApprovalRequestStatus.Approved, ApprovalRequestStatus.Approved, false)]
    [InlineData(ApprovalRequestStatus.Approved, ApprovalRequestStatus.Rejected, false)]
    [InlineData(ApprovalRequestStatus.Rejected, ApprovalRequestStatus.Approved, false)]
    [InlineData(ApprovalRequestStatus.Rejected, ApprovalRequestStatus.Rejected, false)]
    public void CanTransition_Only_Allows_Pending_To_A_Terminal_State(
        ApprovalRequestStatus current,
        ApprovalRequestStatus target,
        bool expected)
    {
        Assert.Equal(expected, ApprovalStateTransition.CanTransition(current, target));
    }
}
