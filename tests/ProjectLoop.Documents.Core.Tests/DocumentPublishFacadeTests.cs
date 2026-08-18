using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentPublishFacadeTests
{
    private sealed class FakeTenantContextAccessor : ICurrentTenantContextAccessor
    {
        public ITenantContext? Current { get; set; }
    }

    private static DocumentsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task<(Document Document, DocumentVersion Version)> SeedClientVisibleDraftAsync(
        DocumentsDbContext dbContext,
        Guid tenantId)
    {
        var now = DateTimeOffset.UtcNow;
        var document = new Document
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = Guid.NewGuid(),
            Title = "Architecture Decision Record",
            Category = "Architecture",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Client,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
        var version = new DocumentVersion
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
        document.CurrentVersionId = version.Id;

        dbContext.Documents.Add(document);
        dbContext.DocumentVersions.Add(version);
        await dbContext.SaveChangesAsync();

        return (document, version);
    }

    private static DocumentPublishFacade CreateFacade(DocumentsDbContext dbContext, ICurrentTenantContextAccessor tenantContextAccessor)
    {
        var documentRepository = new DocumentRepository(dbContext);
        var versionRepository = new DocumentVersionRepository(dbContext);
        var transaction = new DocumentPublishTransaction(dbContext, documentRepository, versionRepository);
        return new DocumentPublishFacade(tenantContextAccessor, documentRepository, versionRepository, transaction);
    }

    [Fact]
    public async Task PublishAsync_Forbids_Client_Users_From_Publishing()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var (document, version) = await SeedClientVisibleDraftAsync(dbContext, tenantId);

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "client-user", IsClientUser = true },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var result = await facade.PublishAsync(new PublishDocumentVersionRequest { DocumentId = document.Id, VersionId = version.Id });

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentPublishFacadeError.Forbidden, result.Error);

        var persistedVersion = await dbContext.DocumentVersions.AsNoTracking().SingleAsync(v => v.Id == version.Id);
        Assert.False(persistedVersion.IsPublished);
    }

    [Fact]
    public async Task PublishAsync_Marks_Current_Version_Published_And_Document_Status_Published_For_Internal_Users()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var (document, version) = await SeedClientVisibleDraftAsync(dbContext, tenantId);

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "internal-user", IsClientUser = false },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var result = await facade.PublishAsync(new PublishDocumentVersionRequest { DocumentId = document.Id, VersionId = version.Id });

        Assert.True(result.IsSuccess);
        Assert.Equal(DocumentStatus.Published, result.Result!.Status);

        var persistedVersion = await dbContext.DocumentVersions.AsNoTracking().SingleAsync(v => v.Id == version.Id);
        Assert.True(persistedVersion.IsPublished);
        Assert.NotNull(persistedVersion.PublishedAtUtc);

        var persistedDocument = await dbContext.Documents.AsNoTracking().SingleAsync(d => d.Id == document.Id);
        Assert.Equal(DocumentStatus.Published, persistedDocument.Status);

        // The version is now in the state a client-visibility download check
        // relies on: Client visibility plus an explicitly published version.
        Assert.Equal(DocumentVisibility.Client, persistedDocument.Visibility);
    }

    [Fact]
    public async Task PublishAsync_Rejects_Republishing_An_Already_Published_Version()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var (document, version) = await SeedClientVisibleDraftAsync(dbContext, tenantId);

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "internal-user", IsClientUser = false },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var first = await facade.PublishAsync(new PublishDocumentVersionRequest { DocumentId = document.Id, VersionId = version.Id });
        Assert.True(first.IsSuccess);

        var second = await facade.PublishAsync(new PublishDocumentVersionRequest { DocumentId = document.Id, VersionId = version.Id });

        Assert.False(second.IsSuccess);
        Assert.Equal(DocumentPublishFacadeError.NotEligible, second.Error);
    }

    [Fact]
    public async Task PublishAsync_Rejects_A_Version_That_Is_No_Longer_The_Current_Version()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var (document, firstVersion) = await SeedClientVisibleDraftAsync(dbContext, tenantId);

        // A newer version supersedes v1 as current, without publishing it.
        var secondVersion = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            VersionNumber = 2,
            BlobObjectKey = "documents/v2-object-key",
            MimeType = "application/pdf",
            SizeBytes = 2048,
            ContentHash = "v2-hash",
            UploadedByUserId = "user-1",
            CreatedAtUtc = DateTimeOffset.UtcNow,
        };
        dbContext.DocumentVersions.Add(secondVersion);
        document.CurrentVersionId = secondVersion.Id;
        await dbContext.SaveChangesAsync();

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "internal-user", IsClientUser = false },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var result = await facade.PublishAsync(new PublishDocumentVersionRequest { DocumentId = document.Id, VersionId = firstVersion.Id });

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentPublishFacadeError.NotEligible, result.Error);

        var persistedFirstVersion = await dbContext.DocumentVersions.AsNoTracking().SingleAsync(v => v.Id == firstVersion.Id);
        Assert.False(persistedFirstVersion.IsPublished);
    }
}
