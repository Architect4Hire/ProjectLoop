namespace ProjectLoop.Documents.Core;

/// <summary>
/// Owns the transaction boundary for creating a Document together with its
/// first DocumentVersion and current-version pointer from already-stored
/// Blob object metadata. Does not touch Blob Storage itself.
/// </summary>
public interface IDocumentUploadTransaction
{
    Task ExecuteAsync(Document document, DocumentVersion firstVersion, CancellationToken cancellationToken = default);
}
