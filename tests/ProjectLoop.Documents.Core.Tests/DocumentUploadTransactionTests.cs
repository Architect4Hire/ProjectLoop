using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentUploadTransactionTests
{
    private static DocumentsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task ExecuteAsync_Persists_Document_Version_And_CurrentVersion_Pointer_Atomically()
    {
        await using var dbContext = CreateDbContext();
        var transaction = new DocumentUploadTransaction(
            dbContext,
            new DocumentRepository(dbContext),
            new DocumentVersionRepository(dbContext));

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
        var version = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            VersionNumber = 1,
            BlobObjectKey = Guid.NewGuid().ToString("n"),
            MimeType = "application/pdf",
            SizeBytes = 4096,
            ContentHash = new string('a', 64),
            UploadedByUserId = "user-1",
            CreatedAtUtc = now,
        };

        await transaction.ExecuteAsync(document, version);

        var persistedDocument = await dbContext.Documents.SingleAsync(d => d.Id == document.Id);
        var persistedVersion = await dbContext.DocumentVersions.SingleAsync(v => v.Id == version.Id);

        Assert.Equal(version.Id, persistedDocument.CurrentVersionId);
        Assert.Equal(document.Id, persistedVersion.DocumentId);
    }
}
