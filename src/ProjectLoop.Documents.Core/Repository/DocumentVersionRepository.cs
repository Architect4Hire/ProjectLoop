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
}
