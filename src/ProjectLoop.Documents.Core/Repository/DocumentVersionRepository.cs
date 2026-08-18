using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Documents.Core;

public sealed class DocumentVersionRepository : IDocumentVersionRepository
{
    private readonly DocumentsDbContext _dbContext;

    public DocumentVersionRepository(DocumentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(DocumentVersion version, CancellationToken cancellationToken = default)
    {
        _dbContext.DocumentVersions.Add(version);
        return Task.CompletedTask;
    }

    public async Task<int> GetNextVersionNumberAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var maxVersionNumber = await _dbContext.DocumentVersions
            .Where(v => v.DocumentId == documentId)
            .Select(v => (int?)v.VersionNumber)
            .MaxAsync(cancellationToken);

        return (maxVersionNumber ?? 0) + 1;
    }

    public Task<DocumentVersion?> GetByIdAsync(Guid documentId, Guid versionId, CancellationToken cancellationToken = default) =>
        _dbContext.DocumentVersions
            .FirstOrDefaultAsync(v => v.Id == versionId && v.DocumentId == documentId, cancellationToken);

    public void MarkPublished(DocumentVersion version, DateTimeOffset publishedAtUtc)
    {
        if (_dbContext.Entry(version).State == EntityState.Detached)
        {
            _dbContext.DocumentVersions.Attach(version);
        }

        version.IsPublished = true;
        version.PublishedAtUtc = publishedAtUtc;
    }
}
