using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentRepositorySetCurrentVersionTests
{
    private static Document CreateDocument(DateTimeOffset now) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        ProjectId = Guid.NewGuid(),
        Title = "Statement of Work",
        Category = "Contract",
        Status = DocumentStatus.Draft,
        Visibility = DocumentVisibility.Internal,
        CreatedAtUtc = now,
        UpdatedAtUtc = now,
    };

    [Fact]
    public async Task SetCurrentVersion_Updates_Pointer_When_No_Concurrent_Change()
    {
        var databaseName = Guid.NewGuid().ToString();
        var now = DateTimeOffset.UtcNow;
        var document = CreateDocument(now);

        await using (var seedContext = new DocumentsDbContext(BuildOptions(databaseName)))
        {
            seedContext.Documents.Add(document);
            await seedContext.SaveChangesAsync();
        }

        await using var dbContext = new DocumentsDbContext(BuildOptions(databaseName));
        var tracked = await dbContext.Documents.SingleAsync(d => d.Id == document.Id);
        var repository = new DocumentRepository(dbContext);
        var versionId = Guid.NewGuid();
        var updatedAt = now.AddMinutes(1);

        repository.SetCurrentVersion(tracked, versionId, updatedAt);
        await dbContext.SaveChangesAsync();

        var persisted = await dbContext.Documents.SingleAsync(d => d.Id == document.Id);
        Assert.Equal(versionId, persisted.CurrentVersionId);
        Assert.Equal(updatedAt, persisted.UpdatedAtUtc);
    }

    [Fact]
    public async Task SetCurrentVersion_Throws_Concurrency_Exception_When_Document_Changed_Since_Load()
    {
        var databaseName = Guid.NewGuid().ToString();
        var now = DateTimeOffset.UtcNow;
        var document = CreateDocument(now);

        await using (var seedContext = new DocumentsDbContext(BuildOptions(databaseName)))
        {
            seedContext.Documents.Add(document);
            await seedContext.SaveChangesAsync();
        }

        // Load the document into two independent contexts, simulating two
        // concurrent requests, and let the first one win.
        await using var firstRequest = new DocumentsDbContext(BuildOptions(databaseName));
        await using var secondRequest = new DocumentsDbContext(BuildOptions(databaseName));

        var firstTracked = await firstRequest.Documents.SingleAsync(d => d.Id == document.Id);
        var secondTracked = await secondRequest.Documents.SingleAsync(d => d.Id == document.Id);

        new DocumentRepository(firstRequest).SetCurrentVersion(firstTracked, Guid.NewGuid(), now.AddMinutes(1));
        await firstRequest.SaveChangesAsync();

        new DocumentRepository(secondRequest).SetCurrentVersion(secondTracked, Guid.NewGuid(), now.AddMinutes(2));

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => secondRequest.SaveChangesAsync());
    }

    private static DbContextOptions<DocumentsDbContext> BuildOptions(string databaseName) =>
        new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
}
