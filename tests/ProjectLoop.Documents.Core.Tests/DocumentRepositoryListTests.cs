using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentRepositoryListTests
{
    private static DocumentsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Document CreateDocument(
        Guid tenantId,
        Guid projectId,
        string category = "Contract",
        DocumentStatus status = DocumentStatus.Draft,
        DocumentVisibility visibility = DocumentVisibility.Internal,
        DateTimeOffset? updatedAtUtc = null)
    {
        var now = updatedAtUtc ?? DateTimeOffset.UtcNow;

        return new Document
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = projectId,
            Title = "Document",
            Category = category,
            Status = status,
            Visibility = visibility,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
    }

    [Fact]
    public async Task ListAsync_Only_Returns_Documents_For_The_Owning_Tenant_And_Project()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        dbContext.Documents.Add(CreateDocument(tenantId, projectId));
        dbContext.Documents.Add(CreateDocument(Guid.NewGuid(), projectId)); // different tenant
        dbContext.Documents.Add(CreateDocument(tenantId, Guid.NewGuid())); // different project
        await dbContext.SaveChangesAsync();

        var repository = new DocumentRepository(dbContext);
        var (items, totalCount) = await repository.ListAsync(tenantId, new DocumentListQuery { ProjectId = projectId });

        Assert.Equal(1, totalCount);
        Assert.Single(items);
    }

    [Fact]
    public async Task ListAsync_Applies_Category_Status_And_Visibility_Filters()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        dbContext.Documents.Add(CreateDocument(tenantId, projectId, category: "Contract", status: DocumentStatus.Published, visibility: DocumentVisibility.Client));
        dbContext.Documents.Add(CreateDocument(tenantId, projectId, category: "Deliverable", status: DocumentStatus.Draft, visibility: DocumentVisibility.Internal));
        await dbContext.SaveChangesAsync();

        var repository = new DocumentRepository(dbContext);
        var (items, totalCount) = await repository.ListAsync(tenantId, new DocumentListQuery
        {
            ProjectId = projectId,
            Category = "Contract",
            Status = DocumentStatus.Published,
            Visibility = DocumentVisibility.Client,
        });

        Assert.Equal(1, totalCount);
        Assert.Equal("Contract", items[0].Category);
    }

    [Fact]
    public async Task ListAsync_Paginates_Ordered_By_Most_Recently_Updated()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var baseTime = DateTimeOffset.UtcNow;

        for (var i = 0; i < 5; i++)
        {
            dbContext.Documents.Add(CreateDocument(tenantId, projectId, updatedAtUtc: baseTime.AddMinutes(i)));
        }

        await dbContext.SaveChangesAsync();

        var repository = new DocumentRepository(dbContext);
        var (items, totalCount) = await repository.ListAsync(tenantId, new DocumentListQuery { ProjectId = projectId, Page = 2, PageSize = 2 });

        Assert.Equal(5, totalCount);
        Assert.Equal(2, items.Count);
        // Page 1 (desc by UpdatedAtUtc) holds minutes 4,3; page 2 holds 2,1.
        Assert.Equal(baseTime.AddMinutes(2), items[0].UpdatedAtUtc);
        Assert.Equal(baseTime.AddMinutes(1), items[1].UpdatedAtUtc);
    }
}
