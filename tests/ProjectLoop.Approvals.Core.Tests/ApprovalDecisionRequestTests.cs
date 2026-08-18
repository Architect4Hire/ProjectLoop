using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalDecisionRequestTests
{
    [Fact]
    public void ApprovalDecisionRequest_Can_Be_Created_With_Required_Members()
    {
        var request = new ApprovalDecisionRequest
        {
            ApprovalRequestId = Guid.NewGuid(),
            Comments = "Looks good.",
        };

        Assert.NotNull(request.Comments);
    }

    [Fact]
    public void ApprovalDecisionRequest_Comments_Are_Optional()
    {
        var request = new ApprovalDecisionRequest
        {
            ApprovalRequestId = Guid.NewGuid(),
        };

        Assert.Null(request.Comments);
    }
}
