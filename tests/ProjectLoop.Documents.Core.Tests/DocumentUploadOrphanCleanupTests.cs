using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

/// <summary>
/// Proves the upload facade deletes an already-stored blob when the SQL
/// metadata transaction fails afterward, so a partial upload never leaves an
/// unreferenced blob behind. Runs against a real Azurite/Aspire-compatible
/// Blob endpoint; skipped when no endpoint is configured, matching
/// <see cref="AzureBlobDocumentStoreIntegrationTests"/>.
/// </summary>
public class DocumentUploadOrphanCleanupTests
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

    private sealed class AlwaysFailingUploadTransaction : IDocumentUploadTransaction
    {
        public Task ExecuteAsync(Document document, DocumentVersion firstVersion, CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("Simulated SQL failure after the blob was already stored.");
    }

    private static bool TryGetConnectionString(out string connectionString)
    {
        connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable) ?? string.Empty;
        return !string.IsNullOrEmpty(connectionString);
    }

    [Fact]
    public async Task UploadAsync_Deletes_The_Blob_When_The_Sql_Transaction_Fails()
    {
        if (!TryGetConnectionString(out var connectionString))
        {
            return;
        }

        var containerName = $"documents-{Guid.NewGuid():n}";
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobStore = new AzureBlobDocumentStore(blobServiceClient, Options.Create(new DocumentBlobStorageOptions { ContainerName = containerName }));

        var accessor = new TestCurrentTenantContextAccessor
        {
            Current = new TestTenantContext { TenantId = Guid.NewGuid(), UserId = "user-1" },
        };
        var facade = new DocumentUploadFacade(
            accessor,
            new DocumentUploadSizeValidator(Options.Create(new DocumentUploadLimitsOptions())),
            blobStore,
            new AlwaysFailingUploadTransaction());

        using var content = new MemoryStream("content that will orphan"u8.ToArray());
        var result = await facade.UploadAsync(
            new UploadDocumentMetadata { ProjectId = Guid.NewGuid(), Title = "Statement of Work", Category = "Contract", Visibility = DocumentVisibility.Internal },
            content,
            "sow.pdf",
            "application/pdf");

        Assert.False(result.IsSuccess);
        Assert.Equal(DocumentUploadFacadeError.PersistenceFailed, result.Error);

        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var remainingBlobs = new List<string>();
        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            remainingBlobs.Add(blobItem.Name);
        }

        Assert.Empty(remainingBlobs);
    }
}
