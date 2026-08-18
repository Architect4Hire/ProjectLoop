using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentUploadFacadeTests
{
    private sealed class TestTenantContext : ITenantContext
    {
        public required Guid TenantId { get; init; }

        public required string UserId { get; init; }

        public bool IsClientUser { get; init; }
    }

    private sealed class TestCurrentTenantContextAccessor : ICurrentTenantContextAccessor
    {
        public ITenantContext? Current { get; set; }
    }

    private sealed class FakeBlobDocumentStore : IBlobDocumentStore
    {
        public string? LastPutObjectKey { get; private set; }

        public string? DeletedObjectKey { get; private set; }

        public Task<string> PutAsync(Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            LastPutObjectKey = Guid.NewGuid().ToString("n");
            return Task.FromResult(LastPutObjectKey);
        }

        public Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task DeleteIfOrphanAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            DeletedObjectKey = objectKey;
            return Task.CompletedTask;
        }
    }

    private static DocumentsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static DocumentUploadFacade CreateFacade(
        DocumentsDbContext dbContext,
        ICurrentTenantContextAccessor accessor,
        IBlobDocumentStore blobStore,
        long maxSizeBytes = 1_000_000) =>
        new(
            accessor,
            new DocumentUploadSizeValidator(Options.Create(new DocumentUploadLimitsOptions { MaxSizeBytes = maxSizeBytes })),
            blobStore,
            new DocumentUploadTransaction(dbContext, new DocumentRepository(dbContext), new DocumentVersionRepository(dbContext)));

    private static UploadDocumentMetadata CreateMetadata() => new()
    {
        ProjectId = Guid.NewGuid(),
        Title = "Statement of Work",
        Category = "Contract",
        Visibility = DocumentVisibility.Internal,
    };

    [Fact]
    public async Task UploadAsync_Returns_NoTenantContext_When_Unauthenticated()
    {
        await using var dbContext = CreateDbContext();
        var facade = CreateFacade(dbContext, new TestCurrentTenantContextAccessor(), new FakeBlobDocumentStore());

        using var content = new MemoryStream("content"u8.ToArray());
        var result = await facade.UploadAsync(CreateMetadata(), content, "file.pdf", "application/pdf");

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentUploadFacadeError.NoTenantContext, result.Error);
    }

    [Fact]
    public async Task UploadAsync_Returns_InvalidMetadata_When_Title_Is_Blank()
    {
        await using var dbContext = CreateDbContext();
        var accessor = new TestCurrentTenantContextAccessor { Current = new TestTenantContext { TenantId = Guid.NewGuid(), UserId = "user-1" } };
        var facade = CreateFacade(dbContext, accessor, new FakeBlobDocumentStore());
        var metadata = new UploadDocumentMetadata { ProjectId = Guid.NewGuid(), Title = "   ", Category = "Contract", Visibility = DocumentVisibility.Internal };

        using var content = new MemoryStream("content"u8.ToArray());
        var result = await facade.UploadAsync(metadata, content, "file.pdf", "application/pdf");

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentUploadFacadeError.InvalidMetadata, result.Error);
    }

    [Fact]
    public async Task UploadAsync_Returns_InvalidFileName_For_Traversal_Only_Name()
    {
        await using var dbContext = CreateDbContext();
        var accessor = new TestCurrentTenantContextAccessor { Current = new TestTenantContext { TenantId = Guid.NewGuid(), UserId = "user-1" } };
        var facade = CreateFacade(dbContext, accessor, new FakeBlobDocumentStore());

        using var content = new MemoryStream("content"u8.ToArray());
        var result = await facade.UploadAsync(CreateMetadata(), content, "..", "application/pdf");

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentUploadFacadeError.InvalidFileName, result.Error);
    }

    [Fact]
    public async Task UploadAsync_Returns_UnsupportedMediaType_For_Disallowed_Mime()
    {
        await using var dbContext = CreateDbContext();
        var accessor = new TestCurrentTenantContextAccessor { Current = new TestTenantContext { TenantId = Guid.NewGuid(), UserId = "user-1" } };
        var facade = CreateFacade(dbContext, accessor, new FakeBlobDocumentStore());

        using var content = new MemoryStream("content"u8.ToArray());
        var result = await facade.UploadAsync(CreateMetadata(), content, "file.exe", "application/x-msdownload");

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentUploadFacadeError.UnsupportedMediaType, result.Error);
    }

    [Fact]
    public async Task UploadAsync_Returns_PayloadTooLarge_When_Content_Exceeds_Configured_Limit()
    {
        await using var dbContext = CreateDbContext();
        var accessor = new TestCurrentTenantContextAccessor { Current = new TestTenantContext { TenantId = Guid.NewGuid(), UserId = "user-1" } };
        var facade = CreateFacade(dbContext, accessor, new FakeBlobDocumentStore(), maxSizeBytes: 4);

        using var content = new MemoryStream("too-large-content"u8.ToArray());
        var result = await facade.UploadAsync(CreateMetadata(), content, "file.pdf", "application/pdf");

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentUploadFacadeError.PayloadTooLarge, result.Error);
    }

    [Fact]
    public async Task UploadAsync_Persists_Document_And_First_Version_On_Success()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var accessor = new TestCurrentTenantContextAccessor { Current = new TestTenantContext { TenantId = tenantId, UserId = "user-1" } };
        var blobStore = new FakeBlobDocumentStore();
        var facade = CreateFacade(dbContext, accessor, blobStore);
        var metadata = CreateMetadata();

        using var content = new MemoryStream("statement of work content"u8.ToArray());
        var result = await facade.UploadAsync(metadata, content, "sow.pdf", "application/pdf");

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Result!.VersionNumber);

        var persistedDocument = await dbContext.Documents.SingleAsync(d => d.Id == result.Result.DocumentId);
        Assert.Equal(tenantId, persistedDocument.TenantId);
        Assert.Equal(result.Result.VersionId, persistedDocument.CurrentVersionId);

        var persistedVersion = await dbContext.DocumentVersions.SingleAsync(v => v.Id == result.Result.VersionId);
        Assert.Equal(blobStore.LastPutObjectKey, persistedVersion.BlobObjectKey);
    }
}
