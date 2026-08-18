using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentListFacadeTests
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

    private static DocumentsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Document CreateDocument(Guid tenantId, Guid projectId)
    {
        var now = DateTimeOffset.UtcNow;

        return new Document
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = projectId,
            Title = "Statement of Work",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
    }

    [Fact]
    public async Task ListAsync_Returns_NoTenantContext_When_Unauthenticated()
    {
        await using var dbContext = CreateDbContext();
        var facade = new DocumentListFacade(new TestCurrentTenantContextAccessor(), new DocumentRepository(dbContext));

        var result = await facade.ListAsync(new DocumentListQuery { ProjectId = Guid.NewGuid() });

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentListFacadeError.NoTenantContext, result.Error);
    }

    [Fact]
    public async Task ListAsync_Returns_Mapped_Documents_For_The_Owning_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        dbContext.Documents.Add(CreateDocument(tenantId, projectId));
        dbContext.Documents.Add(CreateDocument(Guid.NewGuid(), projectId));
        await dbContext.SaveChangesAsync();

        var accessor = new TestCurrentTenantContextAccessor
        {
            Current = new TestTenantContext { TenantId = tenantId, UserId = "user-1" },
        };
        var facade = new DocumentListFacade(accessor, new DocumentRepository(dbContext));

        var result = await facade.ListAsync(new DocumentListQuery { ProjectId = projectId });

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Response!.TotalCount);
        Assert.Single(result.Response.Items);
    }
}
