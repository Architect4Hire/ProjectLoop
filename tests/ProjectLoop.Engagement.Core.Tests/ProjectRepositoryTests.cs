using Microsoft.EntityFrameworkCore;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class ProjectRepositoryTests
{
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

    [Fact]
    public async Task FindByIdAsync_Returns_Matching_Project_For_Owning_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        dbContext.Projects.Add(CreateProject(tenantId, projectId));
        await dbContext.SaveChangesAsync();

        var repository = new ProjectRepository(dbContext);
        var found = await repository.FindByIdAsync(tenantId, projectId);

        Assert.NotNull(found);
        Assert.Equal(projectId, found!.Id);
    }

    [Fact]
    public async Task FindByIdAsync_Returns_Null_When_Project_Belongs_To_Different_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var projectId = Guid.NewGuid();
        dbContext.Projects.Add(CreateProject(Guid.NewGuid(), projectId));
        await dbContext.SaveChangesAsync();

        var repository = new ProjectRepository(dbContext);
        var found = await repository.FindByIdAsync(Guid.NewGuid(), projectId);

        Assert.Null(found);
    }

    [Fact]
    public async Task FindByIdAsync_Returns_Null_When_No_Match()
    {
        await using var dbContext = CreateDbContext();
        var repository = new ProjectRepository(dbContext);

        var found = await repository.FindByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(found);
    }
}
