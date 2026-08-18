using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

/// <summary>
/// Exercises the full upload seam — private Blob Storage plus SQL metadata —
/// against a real Azurite/Aspire-compatible Blob endpoint. Skipped when no
/// endpoint is configured, matching <see cref="AzureBlobDocumentStoreIntegrationTests"/>.
/// </summary>
public class DocumentUploadEndToEndTests
{
    private const string ConnectionStringEnvironmentVariable = "DOCUMENTS_BLOB_TEST_CONNECTION_STRING";

    private sealed class TestTenantContext : ITenantContext
    {
        public required Guid TenantId { get; init; }

        public required string UserId { get; init; }
    }

    private sealed class TestCurrentTenantContextAccessor : ICurrentTenantContextAccessor
    {
        public ITenantContext? Current { get; set; }
    }

    private static bool TryGetConnectionString(out string connectionString)
    {
        connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable) ?? string.Empty;
        return !string.IsNullOrEmpty(connectionString);
    }

    [Fact]
    public async Task UploadAsync_Creates_One_Blob_One_Document_And_One_Version_With_Matching_Hash_And_Size()
    {
        if (!TryGetConnectionString(out var connectionString))
        {
            return;
        }

        var containerName = $"documents-{Guid.NewGuid():n}";
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobStore = new AzureBlobDocumentStore(blobServiceClient, Options.Create(new DocumentBlobStorageOptions { ContainerName = containerName }));

        await using var dbContext = new DocumentsDbContext(new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        var tenantId = Guid.NewGuid();
        var accessor = new TestCurrentTenantContextAccessor
        {
            Current = new TestTenantContext { TenantId = tenantId, UserId = "user-1" },
        };
        var facade = new DocumentUploadFacade(
            accessor,
            new DocumentUploadSizeValidator(Options.Create(new DocumentUploadLimitsOptions())),
            blobStore,
            new DocumentUploadTransaction(dbContext, new DocumentRepository(dbContext), new DocumentVersionRepository(dbContext)));

        var payload = "statement of work end-to-end content"u8.ToArray();
        var expectedHash = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(payload));

        using var content = new MemoryStream(payload);
        var result = await facade.UploadAsync(
            new UploadDocumentMetadata { ProjectId = Guid.NewGuid(), Title = "Statement of Work", Category = "Contract", Visibility = DocumentVisibility.Internal },
            content,
            "sow.pdf",
            "application/pdf");

        Assert.True(result.IsSuccess);

        var document = await dbContext.Documents.SingleAsync(d => d.Id == result.Result!.DocumentId);
        var version = await dbContext.DocumentVersions.SingleAsync(v => v.DocumentId == document.Id);

        Assert.Equal(1, await dbContext.Documents.CountAsync());
        Assert.Equal(1, await dbContext.DocumentVersions.CountAsync());
        Assert.Equal(expectedHash, version.ContentHash);
        Assert.Equal(payload.Length, version.SizeBytes);

        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(version.BlobObjectKey);
        Assert.True(await blobClient.ExistsAsync());

        var properties = await containerClient.GetPropertiesAsync();
        Assert.NotEqual(PublicAccessType.Blob, properties.Value.PublicAccess);
        Assert.NotEqual(PublicAccessType.BlobContainer, properties.Value.PublicAccess);
    }
}
