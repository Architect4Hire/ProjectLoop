using Microsoft.EntityFrameworkCore;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentDownloadFacadeTests
{
    private sealed class FakeTenantContextAccessor : ICurrentTenantContextAccessor
    {
        public ITenantContext? Current { get; set; }
    }

    private sealed class FakeBlobDocumentStore : IBlobDocumentStore
    {
        public Task<string> PutAsync(Stream content, string contentType, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken = default) =>
            Task.FromResult<Stream>(new MemoryStream("document-content"u8.ToArray()));

        public Task DeleteIfOrphanAsync(string objectKey, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private static DocumentsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task<(Document Document, DocumentVersion Version)> SeedAsync(
        DocumentsDbContext dbContext,
        Guid tenantId,
        DocumentVisibility visibility,
        bool isPublished)
    {
        var now = DateTimeOffset.UtcNow;
        var document = new Document
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = Guid.NewGuid(),
            Title = "Statement of Work",
            Category = "Contract",
            Status = isPublished ? DocumentStatus.Published : DocumentStatus.Draft,
            Visibility = visibility,
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
            IsPublished = isPublished,
            PublishedAtUtc = isPublished ? now : null,
        };
        document.CurrentVersionId = version.Id;

        dbContext.Documents.Add(document);
        dbContext.DocumentVersions.Add(version);
        await dbContext.SaveChangesAsync();

        return (document, version);
    }

    private static DocumentDownloadFacade CreateFacade(DocumentsDbContext dbContext, ICurrentTenantContextAccessor tenantContextAccessor) =>
        new(tenantContextAccessor, new DocumentRepository(dbContext), new DocumentVersionRepository(dbContext), new FakeBlobDocumentStore());

    [Fact]
    public async Task DownloadAsync_Denies_Client_User_Downloading_An_Unpublished_Version()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var (document, version) = await SeedAsync(dbContext, tenantId, DocumentVisibility.Client, isPublished: false);

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            // Authorized member of the same project/tenant — still not
            // enough, because the version has never been published.
            Current = new TenantContext { TenantId = tenantId, UserId = "client-user", IsClientUser = true },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var result = await facade.DownloadAsync(document.Id, version.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentDownloadFacadeError.Forbidden, result.Error);
    }

    [Fact]
    public async Task DownloadAsync_Allows_Internal_User_Downloading_An_Unpublished_Version()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var (document, version) = await SeedAsync(dbContext, tenantId, DocumentVisibility.Client, isPublished: false);

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "internal-user", IsClientUser = false },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        await using var result = await facade.DownloadAsync(document.Id, version.Id);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DownloadAsync_Allows_Client_User_Downloading_A_Published_Client_Visible_Version()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var (document, version) = await SeedAsync(dbContext, tenantId, DocumentVisibility.Client, isPublished: true);

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "client-user", IsClientUser = true },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        await using var result = await facade.DownloadAsync(document.Id, version.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(document.Title, result.Descriptor!.FileName);
    }

    [Fact]
    public async Task DownloadAsync_Returns_NotFound_When_The_Exact_Version_Belongs_To_Another_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var ownerTenantId = Guid.NewGuid();
        var callerTenantId = Guid.NewGuid();

        // The version is published and client-visible for its own tenant —
        // it must still be unreachable by exact Id from a different tenant.
        var (document, version) = await SeedAsync(dbContext, ownerTenantId, DocumentVisibility.Client, isPublished: true);

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = callerTenantId, UserId = "caller-user", IsClientUser = false },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var result = await facade.DownloadAsync(document.Id, version.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentDownloadFacadeError.NotFound, result.Error);
    }
}
