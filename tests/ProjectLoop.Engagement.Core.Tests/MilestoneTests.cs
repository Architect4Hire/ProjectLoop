using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class MilestoneTests
{
    [Fact]
    public void Milestone_Can_Be_Created_With_Required_Members()
    {
        var now = DateTimeOffset.UtcNow;

        var milestone = new Milestone
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = "Discovery Complete",
            Status = MilestoneStatus.Planned,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        Assert.Equal("Discovery Complete", milestone.Name);
        Assert.Equal(MilestoneStatus.Planned, milestone.Status);
    }

    [Fact]
    public void Milestone_Status_Can_Transition_To_AtRisk()
    {
        var now = DateTimeOffset.UtcNow;
        var milestone = new Milestone
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = "Discovery Complete",
            Status = MilestoneStatus.InProgress,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        milestone.Status = MilestoneStatus.AtRisk;

        Assert.Equal(MilestoneStatus.AtRisk, milestone.Status);
    }
}
