using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class MilestoneApprovalTargetMatcherTests
{
    [Fact]
    public void TargetsMilestone_Returns_True_For_Matching_Milestone_Target()
    {
        var milestoneId = Guid.NewGuid();

        Assert.True(MilestoneApprovalTargetMatcher.TargetsMilestone("Milestone", milestoneId, milestoneId));
    }

    [Fact]
    public void TargetsMilestone_Returns_False_For_Non_Milestone_Target_Type()
    {
        var milestoneId = Guid.NewGuid();

        Assert.False(MilestoneApprovalTargetMatcher.TargetsMilestone("DocumentVersion", milestoneId, milestoneId));
    }

    [Fact]
    public void TargetsMilestone_Returns_False_For_Different_Milestone_Id()
    {
        Assert.False(MilestoneApprovalTargetMatcher.TargetsMilestone("Milestone", Guid.NewGuid(), Guid.NewGuid()));
    }

    [Theory]
    [InlineData(MilestoneStatus.Planned)]
    [InlineData(MilestoneStatus.InProgress)]
    [InlineData(MilestoneStatus.AtRisk)]
    public void GetEligibleStatusOnGranted_Returns_Completed_When_Not_Already_Completed(MilestoneStatus currentStatus)
    {
        Assert.Equal(MilestoneStatus.Completed, MilestoneApprovalTargetMatcher.GetEligibleStatusOnGranted(currentStatus));
    }

    [Fact]
    public void GetEligibleStatusOnGranted_Returns_Null_When_Already_Completed()
    {
        Assert.Null(MilestoneApprovalTargetMatcher.GetEligibleStatusOnGranted(MilestoneStatus.Completed));
    }

    [Theory]
    [InlineData(MilestoneStatus.Planned)]
    [InlineData(MilestoneStatus.InProgress)]
    [InlineData(MilestoneStatus.Completed)]
    public void GetEligibleStatusOnRejected_Returns_AtRisk_When_Not_Already_AtRisk(MilestoneStatus currentStatus)
    {
        Assert.Equal(MilestoneStatus.AtRisk, MilestoneApprovalTargetMatcher.GetEligibleStatusOnRejected(currentStatus));
    }

    [Fact]
    public void GetEligibleStatusOnRejected_Returns_Null_When_Already_AtRisk()
    {
        Assert.Null(MilestoneApprovalTargetMatcher.GetEligibleStatusOnRejected(MilestoneStatus.AtRisk));
    }
}
