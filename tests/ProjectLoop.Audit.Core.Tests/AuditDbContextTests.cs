using Microsoft.EntityFrameworkCore;
using ProjectLoop.Audit.Core;
using Xunit;

namespace ProjectLoop.Audit.Core.Tests;

public class AuditDbContextTests
{
    private static AuditDbContext CreateSqlServerModelDbContext() =>
        new(new DbContextOptionsBuilder<AuditDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopAudit;Trusted_Connection=True;")
            .Options);

    [Fact]
    public void Model_Builds_Without_Error()
    {
        using var context = CreateSqlServerModelDbContext();

        var model = context.Model;

        Assert.NotNull(model);
        Assert.Equal("audit", model.GetDefaultSchema());
    }

    [Fact]
    public void Model_Includes_AuditRecord()
    {
        using var context = CreateSqlServerModelDbContext();

        var entity = context.Model.FindEntityType(typeof(AuditRecord));

        Assert.NotNull(entity);
        Assert.Equal("AuditRecords", entity.GetTableName());
    }

    [Fact]
    public void AuditRecords_DbSet_Is_Queryable()
    {
        using var context = CreateSqlServerModelDbContext();

        Assert.NotNull(context.AuditRecords);
    }

    [Fact]
    public void Model_Includes_InboxMessage()
    {
        using var context = CreateSqlServerModelDbContext();

        var entity = context.Model.FindEntityType(typeof(InboxMessage));

        Assert.NotNull(entity);
        Assert.Equal("InboxMessages", entity.GetTableName());
    }

    [Fact]
    public void InboxMessages_DbSet_Is_Queryable()
    {
        using var context = CreateSqlServerModelDbContext();

        Assert.NotNull(context.InboxMessages);
    }
}
