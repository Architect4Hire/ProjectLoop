using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace ProjectLoop.Documents.Core;

public sealed class AzureBlobDocumentStore : IBlobDocumentStore
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobDocumentStore(BlobServiceClient blobServiceClient, IOptions<DocumentBlobStorageOptions> options)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = options.Value.ContainerName;
    }

    public async Task<string> PutAsync(Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = await EnsureContainerAsync(cancellationToken);

        // The object key is service-generated and opaque; it carries no
        // document metadata and is never returned to callers as a URL.
        var objectKey = Guid.NewGuid().ToString("n");
        var blobClient = containerClient.GetBlobClient(objectKey);

        await blobClient.UploadAsync(
            content,
            new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } },
            cancellationToken);

        return objectKey;
    }

    public async Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        // Internal storage-abstraction method only: callers must authorize
        // document access before invoking this. No HTTP endpoint exposes it
        // directly.
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(objectKey);

        return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
    }

    public Task DeleteIfOrphanAsync(string objectKey, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    private async Task<BlobContainerClient> EnsureContainerAsync(CancellationToken cancellationToken)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        // Private container: no anonymous/public read access at any level.
        // A Blob URL must never function as an authorization boundary.
        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.None,
            cancellationToken: cancellationToken);

        return containerClient;
    }
}
