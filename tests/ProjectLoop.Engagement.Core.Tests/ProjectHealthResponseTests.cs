using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class ProjectHealthResponseTests
{
    [Fact]
    public void ProjectHealthResponse_Can_Be_Created_With_Required_Members()
    {
        var updatedAtUtc = DateTimeOffset.UtcNow;
        var projectId = Guid.NewGuid();

        var response = new ProjectHealthResponse
        {
            ProjectId = projectId,
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Amber,
            UpdatedAtUtc = updatedAtUtc,
        };

        Assert.Equal(projectId, response.ProjectId);
        Assert.Equal(ProjectStatus.Active, response.Status);
        Assert.Equal(ProjectHealth.Amber, response.Health);
        Assert.Equal(updatedAtUtc, response.UpdatedAtUtc);
    }
}
