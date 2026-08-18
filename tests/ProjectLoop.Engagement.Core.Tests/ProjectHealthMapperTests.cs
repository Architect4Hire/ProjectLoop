using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class ProjectHealthMapperTests
{
    [Fact]
    public void ToHealthResponse_Maps_Health_Relevant_Fields()
    {
        var now = DateTimeOffset.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = "Portal Modernization",
            Status = ProjectStatus.OnHold,
            Health = ProjectHealth.Red,
            CreatedAtUtc = now.AddDays(-30),
            UpdatedAtUtc = now,
        };

        var response = ProjectHealthMapper.ToHealthResponse(project);

        Assert.Equal(project.Id, response.ProjectId);
        Assert.Equal(project.Status, response.Status);
        Assert.Equal(project.Health, response.Health);
        Assert.Equal(project.UpdatedAtUtc, response.UpdatedAtUtc);
    }
}
