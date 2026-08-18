using Microsoft.EntityFrameworkCore;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class ProjectMilestonesFacadeTests
{
    private sealed class TestTenantContext : ITenantContext
    {
        public required Guid TenantId { get; init; }

        public required string UserId { get; init; }
    }

    private sealed class TestCurrentTenantContextAccessor : ICurrentTenantContextAccessor
    {
        public ITenantContext? Current { get; set; }
    }

    private static EngagementDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<EngagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new EngagementDbContext(options);
    }

    private static Project CreateProject(Guid tenantId, Guid id)
    {
        var now = DateTimeOffset.UtcNow;

        return new Project
        {
            Id = id,
            TenantId = tenantId,
            Name = "Portal Modernization",
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Green,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
    }

    private static Milestone CreateMilestone(Guid tenantId, Guid projectId, string name)
    {
        var now = DateTimeOffset.UtcNow;

        return new Milestone
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = projectId,
            Name = name,
            Status = MilestoneStatus.Planned,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
    }

    [Fact]
    public async Task GetProjectMilestonesAsync_Returns_NoTenantContext_When_Unauthenticated()
    {
        await using var dbContext = CreateDbContext();
        var facade = new ProjectMilestonesFacade(
            new TestCurrentTenantContextAccessor(),
            new ProjectRepository(dbContext),
            new MilestoneRepository(dbContext));

        var result = await facade.GetProjectMilestonesAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ProjectMilestonesFacadeError.NoTenantContext, result.Error);
    }

    [Fact]
    public async Task GetProjectMilestonesAsync_Returns_NotFound_When_Project_Belongs_To_Different_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var projectId = Guid.NewGuid();
        dbContext.Projects.Add(CreateProject(Guid.NewGuid(), projectId));
        await dbContext.SaveChangesAsync();

        var accessor = new TestCurrentTenantContextAccessor
        {
            Current = new TestTenantContext { TenantId = Guid.NewGuid(), UserId = "user-1" },
        };
        var facade = new ProjectMilestonesFacade(accessor, new ProjectRepository(dbContext), new MilestoneRepository(dbContext));

        var result = await facade.GetProjectMilestonesAsync(projectId);

        Assert.False(result.IsSuccess);
        Assert.Equal(ProjectMilestonesFacadeError.NotFound, result.Error);
    }

    [Fact]
    public async Task GetProjectMilestonesAsync_Returns_Mapped_Milestones_For_Owning_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        dbContext.Projects.Add(CreateProject(tenantId, projectId));
        dbContext.Milestones.Add(CreateMilestone(tenantId, projectId, "Discovery Complete"));
        dbContext.Milestones.Add(CreateMilestone(tenantId, projectId, "UAT Sign-off"));
        await dbContext.SaveChangesAsync();

        var accessor = new TestCurrentTenantContextAccessor
        {
            Current = new TestTenantContext { TenantId = tenantId, UserId = "user-1" },
        };
        var facade = new ProjectMilestonesFacade(accessor, new ProjectRepository(dbContext), new MilestoneRepository(dbContext));

        var result = await facade.GetProjectMilestonesAsync(projectId);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Milestones!.Count);
    }
}
