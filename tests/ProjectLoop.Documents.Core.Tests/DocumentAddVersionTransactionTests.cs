using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentAddVersionTransactionTests
{
    private static DocumentsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DocumentsDbContext(options);
    }

    private static async Task<(Document Document, DocumentVersion FirstVersion)> SeedDocumentWithFirstVersionAsync(DocumentsDbContext dbContext)
    {
        var now = DateTimeOffset.UtcNow;
        var document = new Document
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
        var firstVersion = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            VersionNumber = 1,
            BlobObjectKey = "documents/v1-object-key",
            MimeType = "application/pdf",
            SizeBytes = 1024,
            ContentHash = "v1-hash",
            UploadedByUserId = "user-1",
            CreatedAtUtc = now,
        };
        document.CurrentVersionId = firstVersion.Id;

        dbContext.Documents.Add(document);
        dbContext.DocumentVersions.Add(firstVersion);
        await dbContext.SaveChangesAsync();

        return (document, firstVersion);
    }

    [Fact]
    public async Task ExecuteAsync_Adds_V2_Without_Modifying_V1()
    {
        await using var dbContext = CreateDbContext();
        var documentRepository = new DocumentRepository(dbContext);
        var versionRepository = new DocumentVersionRepository(dbContext);
        var transaction = new DocumentAddVersionTransaction(dbContext, documentRepository, versionRepository);

        var (document, firstVersion) = await SeedDocumentWithFirstVersionAsync(dbContext);

        var secondVersion = await transaction.ExecuteAsync(
            document,
            Guid.NewGuid(),
            "documents/v2-object-key",
            "application/pdf",
            2048,
            "v2-hash",
            "user-2",
            DateTimeOffset.UtcNow,
            CancellationToken.None);

        // v1's persisted row is untouched.
        var persistedFirstVersion = await dbContext.DocumentVersions.AsNoTracking()
            .SingleAsync(v => v.Id == firstVersion.Id);
        Assert.Equal(1, persistedFirstVersion.VersionNumber);
        Assert.Equal(firstVersion.BlobObjectKey, persistedFirstVersion.BlobObjectKey);
        Assert.Equal(firstVersion.ContentHash, persistedFirstVersion.ContentHash);
        Assert.Equal(firstVersion.CreatedAtUtc, persistedFirstVersion.CreatedAtUtc);

        // v2 was created with the next version number.
        Assert.Equal(2, secondVersion.VersionNumber);
        Assert.Equal(document.Id, secondVersion.DocumentId);

        // CurrentVersion now points at v2, not v1.
        var persistedDocument = await dbContext.Documents.AsNoTracking()
            .SingleAsync(d => d.Id == document.Id);
        Assert.Equal(secondVersion.Id, persistedDocument.CurrentVersionId);

        // Both versions remain independently queryable — v1 is never
        // overwritten or removed by adding v2.
        var versionCount = await dbContext.DocumentVersions.CountAsync(v => v.DocumentId == document.Id);
        Assert.Equal(2, versionCount);
    }
}
