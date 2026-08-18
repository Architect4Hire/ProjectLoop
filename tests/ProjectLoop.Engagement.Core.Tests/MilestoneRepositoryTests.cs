using Microsoft.EntityFrameworkCore;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class MilestoneRepositoryTests
{
    private static EngagementDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<EngagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new EngagementDbContext(options);
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
    public async Task ListByProjectAsync_Returns_Milestones_For_Owning_Tenant_And_Project()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        dbContext.Milestones.Add(CreateMilestone(tenantId, projectId, "Discovery Complete"));
        dbContext.Milestones.Add(CreateMilestone(tenantId, projectId, "UAT Sign-off"));
        await dbContext.SaveChangesAsync();

        var repository = new MilestoneRepository(dbContext);
        var result = await repository.ListByProjectAsync(tenantId, projectId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task ListByProjectAsync_Excludes_Milestones_From_Different_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var projectId = Guid.NewGuid();
        dbContext.Milestones.Add(CreateMilestone(Guid.NewGuid(), projectId, "Other Tenant Milestone"));
        await dbContext.SaveChangesAsync();

        var repository = new MilestoneRepository(dbContext);
        var result = await repository.ListByProjectAsync(Guid.NewGuid(), projectId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task ListByProjectAsync_Excludes_Milestones_From_Sibling_Project_In_Same_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        dbContext.Milestones.Add(CreateMilestone(tenantId, Guid.NewGuid(), "Other Project Milestone"));
        await dbContext.SaveChangesAsync();

        var repository = new MilestoneRepository(dbContext);
        var result = await repository.ListByProjectAsync(tenantId, Guid.NewGuid());

        Assert.Empty(result);
    }
}
