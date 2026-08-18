namespace ProjectLoop.Documents.Core;

public interface IDocumentRepository
{
    /// <summary>
    /// Stages a new Document for persistence. Does not commit — the caller's
    /// transaction boundary decides when changes are saved.
    /// </summary>
    Task AddAsync(Document document, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the Document with the given Id scoped to the given tenant, or
    /// null when it does not exist or belongs to a different tenant. Tenant
    /// scoping is applied in the query itself so an unauthorized cross-tenant
    /// Id can never be distinguished from a missing one.
    /// </summary>
    Task<Document?> GetByIdAsync(Guid tenantId, Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages the CurrentVersionId pointer update on an already-tracked
    /// Document. Relies on the entity's UpdatedAtUtc concurrency token to
    /// detect a conflicting concurrent modification at SaveChanges time.
    /// Does not commit and does not upload or publish anything.
    /// </summary>
    void SetCurrentVersion(Document document, Guid versionId, DateTimeOffset updatedAtUtc);

    /// <summary>
    /// Stages the Document's Status transition to Published on an
    /// already-tracked Document. Leaves Status untouched when it is already
    /// Approved — publication of a later version never downgrades an
    /// existing approval outcome. Does not commit.
    /// </summary>
    void MarkPublished(Document document, DateTimeOffset updatedAtUtc);

    /// <summary>
    /// Returns a tenant/project-scoped, filtered, paginated page of
    /// Documents together with the total matching count.
    /// </summary>
    Task<(IReadOnlyList<Document> Items, int TotalCount)> ListAsync(
        Guid tenantId,
        DocumentListQuery query,
        CancellationToken cancellationToken = default);
}
