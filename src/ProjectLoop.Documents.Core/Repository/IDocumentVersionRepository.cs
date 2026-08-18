namespace ProjectLoop.Documents.Core;

public interface IDocumentVersionRepository
{
    /// <summary>
    /// Stages a new DocumentVersion for persistence. Does not commit — the
    /// caller's transaction boundary decides when changes are saved.
    /// </summary>
    Task AddAsync(DocumentVersion version, CancellationToken cancellationToken = default);
}
