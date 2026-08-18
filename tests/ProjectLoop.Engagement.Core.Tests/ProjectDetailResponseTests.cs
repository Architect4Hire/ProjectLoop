using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class ProjectDetailResponseTests
{
    [Fact]
    public void ProjectDetailResponse_Can_Be_Created_With_Required_Members()
    {
        var updatedAtUtc = DateTimeOffset.UtcNow;
        var id = Guid.NewGuid();

        var response = new ProjectDetailResponse
        {
            Id = id,
            Name = "Portal Modernization",
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Green,
            UpdatedAtUtc = updatedAtUtc,
        };

        Assert.Equal(id, response.Id);
        Assert.Equal("Portal Modernization", response.Name);
        Assert.Equal(ProjectStatus.Active, response.Status);
        Assert.Equal(ProjectHealth.Green, response.Health);
        Assert.Equal(updatedAtUtc, response.UpdatedAtUtc);
    }
}
