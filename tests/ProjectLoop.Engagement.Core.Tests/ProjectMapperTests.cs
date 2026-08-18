using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class ProjectMapperTests
{
    [Fact]
    public void ToDetailResponse_Maps_All_ProjectFacing_Fields()
    {
        var now = DateTimeOffset.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = "Portal Modernization",
            Status = ProjectStatus.OnHold,
            Health = ProjectHealth.Amber,
            CreatedAtUtc = now.AddDays(-30),
            UpdatedAtUtc = now,
        };

        var response = ProjectMapper.ToDetailResponse(project);

        Assert.Equal(project.Id, response.Id);
        Assert.Equal(project.Name, response.Name);
        Assert.Equal(project.Status, response.Status);
        Assert.Equal(project.Health, response.Health);
        Assert.Equal(project.UpdatedAtUtc, response.UpdatedAtUtc);
    }
}
