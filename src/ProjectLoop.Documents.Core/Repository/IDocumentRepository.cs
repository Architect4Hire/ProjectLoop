namespace ProjectLoop.Documents.Core;

public interface IDocumentRepository
{
    /// <summary>
    /// Stages a new Document for persistence. Does not commit — the caller's
    /// transaction boundary decides when changes are saved.
    /// </summary>
    Task AddAsync(Document document, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages the CurrentVersionId pointer update on an already-tracked
    /// Document. Relies on the entity's UpdatedAtUtc concurrency token to
    /// detect a conflicting concurrent modification at SaveChanges time.
    /// Does not commit and does not upload or publish anything.
    /// </summary>
    void SetCurrentVersion(Document document, Guid versionId, DateTimeOffset updatedAtUtc);

    /// <summary>
    /// Returns a tenant/project-scoped, filtered, paginated page of
    /// Documents together with the total matching count.
    /// </summary>
    Task<(IReadOnlyList<Document> Items, int TotalCount)> ListAsync(
        Guid tenantId,
        DocumentListQuery query,
        CancellationToken cancellationToken = default);
}
