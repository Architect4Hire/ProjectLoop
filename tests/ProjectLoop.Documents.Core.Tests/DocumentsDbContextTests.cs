using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentsDbContextTests
{
    private static DocumentsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopDocuments;Trusted_Connection=True;")
            .Options;

        return new DocumentsDbContext(options);
    }

    [Fact]
    public void Model_Builds_Without_Error()
    {
        using var context = CreateDbContext();

        var model = context.Model;

        Assert.NotNull(model);
        Assert.Equal("documents", model.GetDefaultSchema());
    }

    [Fact]
    public void Model_Includes_Document()
    {
        using var context = CreateDbContext();

        var entity = context.Model.FindEntityType(typeof(Document));

        Assert.NotNull(entity);
        Assert.Equal("Documents", entity.GetTableName());
    }

    [Fact]
    public void Documents_DbSet_Is_Queryable()
    {
        using var context = CreateDbContext();

        Assert.NotNull(context.Documents);
    }

    [Fact]
    public void Model_Includes_DocumentVersion()
    {
        using var context = CreateDbContext();

        var entity = context.Model.FindEntityType(typeof(DocumentVersion));

        Assert.NotNull(entity);
        Assert.Equal("DocumentVersions", entity.GetTableName());
    }

    [Fact]
    public void DocumentVersions_DbSet_Is_Queryable()
    {
        using var context = CreateDbContext();

        Assert.NotNull(context.DocumentVersions);
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
}
