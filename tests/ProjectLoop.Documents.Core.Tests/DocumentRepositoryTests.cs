using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentRepositoryTests
{
    private static DocumentsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DocumentsDbContext(options);
    }

    private static Document CreateDocument(Guid tenantId, Guid id)
    {
        var now = DateTimeOffset.UtcNow;

        return new Document
        {
            Id = id,
            TenantId = tenantId,
            ProjectId = Guid.NewGuid(),
            Title = "Statement of Work",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
    }

    [Fact]
    public async Task AddAsync_Stages_Document_For_The_Next_SaveChanges()
    {
        await using var dbContext = CreateDbContext();
        var repository = new DocumentRepository(dbContext);
        var document = CreateDocument(Guid.NewGuid(), Guid.NewGuid());

        await repository.AddAsync(document);
        await dbContext.SaveChangesAsync();

        var persisted = await dbContext.Documents.SingleAsync(d => d.Id == document.Id);
        Assert.Equal(document.Title, persisted.Title);
    }

    [Fact]
    public async Task AddAsync_Does_Not_Commit_By_Itself()
    {
        await using var dbContext = CreateDbContext();
        var repository = new DocumentRepository(dbContext);
        var document = CreateDocument(Guid.NewGuid(), Guid.NewGuid());

        await repository.AddAsync(document);

        Assert.False(await dbContext.Documents.AnyAsync(d => d.Id == document.Id));
    }
}
