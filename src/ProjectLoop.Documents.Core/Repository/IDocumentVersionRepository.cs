namespace ProjectLoop.Documents.Core;

public interface IDocumentVersionRepository
{
    /// <summary>
    /// Stages a new DocumentVersion for persistence. Does not commit — the
    /// caller's transaction boundary decides when changes are saved.
    /// </summary>
    Task AddAsync(DocumentVersion version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the version number the next DocumentVersion for this document
    /// should use. Safety against a concurrent duplicate is enforced by the
    /// unique (DocumentId, VersionNumber) index at SaveChanges time, not by
    /// this read alone — a caller must be prepared to retry on conflict.
    /// </summary>
    Task<int> GetNextVersionNumberAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the DocumentVersion with the given Id scoped to the given
    /// Document, or null when it does not exist or belongs to a different
    /// document.
    /// </summary>
    Task<DocumentVersion?> GetByIdAsync(Guid documentId, Guid versionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages the publication-state transition on an already-tracked
    /// DocumentVersion. Does not commit and never modifies any other
    /// version's row.
    /// </summary>
    void MarkPublished(DocumentVersion version, DateTimeOffset publishedAtUtc);
}
