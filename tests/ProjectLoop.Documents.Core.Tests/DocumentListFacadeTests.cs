using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentListFacadeTests
{
    private sealed class FakeTenantContextAccessor : ICurrentTenantContextAccessor
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
    public async Task ListAsync_Never_Returns_Another_Tenants_Documents_For_The_Same_Project_Id()
    {
        await using var dbContext = CreateDbContext();
        var projectId = Guid.NewGuid();
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();

        // Two tenants happen to use the same ProjectId value; only the
        // caller's own tenant's rows must ever be visible.
        dbContext.Documents.Add(CreateDocument(tenantAId, projectId));
        dbContext.Documents.Add(CreateDocument(tenantBId, projectId));
        await dbContext.SaveChangesAsync();

        var repository = new DocumentRepository(dbContext);
        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantAId, UserId = "user-1" },
        };
        var facade = new DocumentListFacade(tenantContextAccessor, repository);

        var result = await facade.ListAsync(new DocumentListQuery { ProjectId = projectId });

        Assert.True(result.IsSuccess);
        Assert.Single(result.Response!.Items);
        Assert.Equal(1, result.Response.TotalCount);
    }

    [Fact]
    public async Task ListAsync_Ignores_Any_Tenant_Scope_Implied_By_The_Query_And_Uses_Only_The_Authenticated_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var projectId = Guid.NewGuid();
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();

        dbContext.Documents.Add(CreateDocument(tenantBId, projectId));
        await dbContext.SaveChangesAsync();

        var repository = new DocumentRepository(dbContext);
        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            // The caller is authenticated as tenant A; tenant B's document
            // must remain invisible even though it is the only document for
            // this ProjectId in the database.
            Current = new TenantContext { TenantId = tenantAId, UserId = "user-1" },
        };
        var facade = new DocumentListFacade(tenantContextAccessor, repository);

        var result = await facade.ListAsync(new DocumentListQuery { ProjectId = projectId });

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Response!.Items);
        Assert.Equal(0, result.Response.TotalCount);
    }
}
