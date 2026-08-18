namespace ProjectLoop.Documents.Core;

/// <summary>
/// Abstraction over the private Blob container that holds document binaries.
/// Object keys returned by this store are opaque and are never, by
/// themselves, an authorization grant — access must still be authorized
/// server-side before a stream is opened.
/// </summary>
public interface IBlobDocumentStore
{
    Task<string> PutAsync(Stream content, string contentType, CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken = default);

    Task DeleteIfOrphanAsync(string objectKey, CancellationToken cancellationToken = default);
}
