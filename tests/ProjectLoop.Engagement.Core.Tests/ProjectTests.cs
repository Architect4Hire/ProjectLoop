using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class ProjectTests
{
    [Fact]
    public void Project_Can_Be_Created_With_Required_Members()
    {
        var now = DateTimeOffset.UtcNow;

        var project = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = "Portal Modernization",
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Green,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        Assert.Equal("Portal Modernization", project.Name);
        Assert.Equal(ProjectStatus.Active, project.Status);
        Assert.Equal(ProjectHealth.Green, project.Health);
    }

    [Fact]
    public void Project_Health_Can_Transition_To_Red()
    {
        var now = DateTimeOffset.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = "Portal Modernization",
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Green,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        project.Health = ProjectHealth.Red;

        Assert.Equal(ProjectHealth.Red, project.Health);
    }
}
