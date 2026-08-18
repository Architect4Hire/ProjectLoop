using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

/// <summary>
/// Exercises AzureBlobDocumentStore against a real Azurite/Aspire-compatible
/// Blob endpoint. Skipped when no endpoint is configured, since it depends
/// on infrastructure external to the unit test suite.
/// </summary>
public class AzureBlobDocumentStoreIntegrationTests
{
    private const string ConnectionStringEnvironmentVariable = "DOCUMENTS_BLOB_TEST_CONNECTION_STRING";

    private static bool TryGetConnectionString(out string connectionString)
    {
        connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable) ?? string.Empty;
        return !string.IsNullOrEmpty(connectionString);
    }

    private static AzureBlobDocumentStore CreateStore(string connectionString, out string containerName)
    {
        containerName = $"documents-{Guid.NewGuid():n}";
        var options = Options.Create(new DocumentBlobStorageOptions { ContainerName = containerName });
        var blobServiceClient = new BlobServiceClient(connectionString);
        return new AzureBlobDocumentStore(blobServiceClient, options);
    }

    [Fact]
    public async Task PutAsync_Then_OpenReadAsync_Roundtrips_Content()
    {
        if (!TryGetConnectionString(out var connectionString))
        {
            return;
        }

        var store = CreateStore(connectionString, out _);
        var payload = "document-content"u8.ToArray();

        string objectKey;
        using (var upload = new MemoryStream(payload))
        {
            objectKey = await store.PutAsync(upload, "text/plain");
        }

        Assert.False(string.IsNullOrWhiteSpace(objectKey));

        await using var download = await store.OpenReadAsync(objectKey);
        using var downloaded = new MemoryStream();
        await download.CopyToAsync(downloaded);

        Assert.Equal(payload, downloaded.ToArray());
    }

    [Fact]
    public async Task PutAsync_Creates_Container_With_No_Public_Access()
    {
        if (!TryGetConnectionString(out var connectionString))
        {
            return;
        }

        var store = CreateStore(connectionString, out var containerName);
        using var upload = new MemoryStream("content"u8.ToArray());
        await store.PutAsync(upload, "text/plain");

        var containerClient = new BlobServiceClient(connectionString).GetBlobContainerClient(containerName);
        var properties = await containerClient.GetPropertiesAsync();

        Assert.NotEqual(PublicAccessType.Blob, properties.Value.PublicAccess);
        Assert.NotEqual(PublicAccessType.BlobContainer, properties.Value.PublicAccess);
    }
}
