using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalsDbContextTests
{
    private static ApprovalsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApprovalsDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopApprovals;Trusted_Connection=True;")
            .Options;

        return new ApprovalsDbContext(options);
    }

    [Fact]
    public void Model_Builds_Without_Error()
    {
        using var context = CreateDbContext();

        var model = context.Model;

        Assert.NotNull(model);
        Assert.Equal("approvals", model.GetDefaultSchema());
    }

    [Fact]
    public void Model_Includes_OutboxMessage()
    {
        using var context = CreateDbContext();

        var entity = context.Model.FindEntityType(typeof(OutboxMessage));

        Assert.NotNull(entity);
        Assert.Equal("OutboxMessages", entity.GetTableName());
    }

    [Fact]
    public void OutboxMessages_DbSet_Is_Queryable()
    {
        using var context = CreateDbContext();

        Assert.NotNull(context.OutboxMessages);
    }

    [Fact]
    public void Model_Includes_ApprovalRequest()
    {
        using var context = CreateDbContext();

        var entity = context.Model.FindEntityType(typeof(ApprovalRequest));

        Assert.NotNull(entity);
        Assert.Equal("ApprovalRequests", entity.GetTableName());
    }

    [Fact]
    public void ApprovalRequests_DbSet_Is_Queryable()
    {
        using var context = CreateDbContext();

        Assert.NotNull(context.ApprovalRequests);
    }

    [Fact]
    public void Model_Includes_ApprovalDecision()
    {
        using var context = CreateDbContext();

        var entity = context.Model.FindEntityType(typeof(ApprovalDecision));

        Assert.NotNull(entity);
        Assert.Equal("ApprovalDecisions", entity.GetTableName());
    }

    [Fact]
    public void ApprovalDecisions_DbSet_Is_Queryable()
    {
        using var context = CreateDbContext();

        Assert.NotNull(context.ApprovalDecisions);
    }
}
