namespace ProjectLoop.Documents.Core;

/// <summary>
/// Owns the transaction boundary for adding a new immutable DocumentVersion
/// to an existing Document and repointing CurrentVersion, from already-stored
/// Blob object metadata. Never modifies a prior version's row. Does not touch
/// Blob Storage itself.
/// </summary>
public interface IDocumentAddVersionTransaction
{
    Task<DocumentVersion> ExecuteAsync(
        Document document,
        Guid versionId,
        string blobObjectKey,
        string mimeType,
        long sizeBytes,
        string contentHash,
        string uploadedByUserId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
