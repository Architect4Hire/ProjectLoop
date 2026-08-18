using Microsoft.EntityFrameworkCore;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class EngagementDbContextTests
{
    [Fact]
    public void Model_Builds_Without_Error()
    {
        var options = new DbContextOptionsBuilder<EngagementDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopEngagement;Trusted_Connection=True;")
            .Options;

        using var context = new EngagementDbContext(options);

        var model = context.Model;

        Assert.NotNull(model);
        Assert.Equal("engagement", model.GetDefaultSchema());
    }

    [Fact]
    public void Model_Includes_Project()
    {
        var options = new DbContextOptionsBuilder<EngagementDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopEngagement;Trusted_Connection=True;")
            .Options;

        using var context = new EngagementDbContext(options);

        var entity = context.Model.FindEntityType(typeof(Project));

        Assert.NotNull(entity);
        Assert.Equal("Projects", entity.GetTableName());
    }

    [Fact]
    public void Projects_DbSet_Is_Queryable()
    {
        var options = new DbContextOptionsBuilder<EngagementDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopEngagement;Trusted_Connection=True;")
            .Options;

        using var context = new EngagementDbContext(options);

        Assert.NotNull(context.Projects);
    }

    [Fact]
    public void Model_Includes_Milestone()
    {
        var options = new DbContextOptionsBuilder<EngagementDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopEngagement;Trusted_Connection=True;")
            .Options;

        using var context = new EngagementDbContext(options);

        var entity = context.Model.FindEntityType(typeof(Milestone));

        Assert.NotNull(entity);
        Assert.Equal("Milestones", entity.GetTableName());
    }

    [Fact]
    public void Milestones_DbSet_Is_Queryable()
    {
        var options = new DbContextOptionsBuilder<EngagementDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopEngagement;Trusted_Connection=True;")
            .Options;

        using var context = new EngagementDbContext(options);

        Assert.NotNull(context.Milestones);
    }
}
