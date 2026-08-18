using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class MilestoneMapperTests
{
    private static Milestone CreateMilestone(string name, DateTimeOffset createdAtUtc, MilestoneStatus status = MilestoneStatus.Planned) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = name,
            Status = status,
            CreatedAtUtc = createdAtUtc,
            UpdatedAtUtc = createdAtUtc,
        };

    [Fact]
    public void ToSummaryResponses_Maps_All_Fields()
    {
        var now = DateTimeOffset.UtcNow;
        var milestone = CreateMilestone("Discovery Complete", now, MilestoneStatus.Completed);

        var responses = MilestoneMapper.ToSummaryResponses(new[] { milestone });

        var response = Assert.Single(responses);
        Assert.Equal(milestone.Id, response.Id);
        Assert.Equal(milestone.Name, response.Name);
        Assert.Equal(milestone.Status, response.Status);
        Assert.Equal(milestone.UpdatedAtUtc, response.UpdatedAtUtc);
    }

    [Fact]
    public void ToSummaryResponses_Orders_By_CreatedAtUtc_Ascending()
    {
        var now = DateTimeOffset.UtcNow;
        var third = CreateMilestone("UAT Sign-off", now.AddDays(20));
        var first = CreateMilestone("Discovery Complete", now);
        var second = CreateMilestone("Design Review", now.AddDays(10));

        var responses = MilestoneMapper.ToSummaryResponses(new[] { third, first, second });

        Assert.Equal(
            new[] { "Discovery Complete", "Design Review", "UAT Sign-off" },
            responses.Select(r => r.Name));
    }
}
