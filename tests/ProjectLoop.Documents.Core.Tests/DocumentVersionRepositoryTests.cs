using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentVersionRepositoryTests
{
    private static DocumentsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DocumentsDbContext(options);
    }

    private static Document CreateDocument()
    {
        var now = DateTimeOffset.UtcNow;

        return new Document
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
    }

    private static DocumentVersion CreateVersion(Guid documentId) => new()
    {
        Id = Guid.NewGuid(),
        DocumentId = documentId,
        VersionNumber = 1,
        BlobObjectKey = Guid.NewGuid().ToString("n"),
        MimeType = "application/pdf",
        SizeBytes = 2048,
        ContentHash = new string('a', 64),
        UploadedByUserId = "user-1",
        CreatedAtUtc = DateTimeOffset.UtcNow,
    };

    [Fact]
    public async Task AddAsync_Stages_DocumentVersion_For_The_Next_SaveChanges()
    {
        await using var dbContext = CreateDbContext();
        var document = CreateDocument();
        dbContext.Documents.Add(document);
        await dbContext.SaveChangesAsync();

        var repository = new DocumentVersionRepository(dbContext);
        var version = CreateVersion(document.Id);

        await repository.AddAsync(version);
        await dbContext.SaveChangesAsync();

        var persisted = await dbContext.DocumentVersions.SingleAsync(v => v.Id == version.Id);
        Assert.Equal(1, persisted.VersionNumber);
        Assert.Equal(document.Id, persisted.DocumentId);
    }

    [Fact]
    public async Task AddAsync_Does_Not_Commit_By_Itself()
    {
        await using var dbContext = CreateDbContext();
        var document = CreateDocument();
        dbContext.Documents.Add(document);
        await dbContext.SaveChangesAsync();

        var repository = new DocumentVersionRepository(dbContext);
        var version = CreateVersion(document.Id);

        await repository.AddAsync(version);

        Assert.False(await dbContext.DocumentVersions.AnyAsync(v => v.Id == version.Id));
    }
}
