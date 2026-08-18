namespace ProjectLoop.Documents.Core;

public sealed class DocumentPublishTransaction : IDocumentPublishTransaction
{
    private readonly DocumentsDbContext _dbContext;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _documentVersionRepository;

    public DocumentPublishTransaction(
        DocumentsDbContext dbContext,
        IDocumentRepository documentRepository,
        IDocumentVersionRepository documentVersionRepository)
    {
        _dbContext = dbContext;
        _documentRepository = documentRepository;
        _documentVersionRepository = documentVersionRepository;
    }

    public async Task ExecuteAsync(
        Document document,
        DocumentVersion version,
        DateTimeOffset publishedAtUtc,
        CancellationToken cancellationToken = default)
    {
        _documentVersionRepository.MarkPublished(version, publishedAtUtc);
        _documentRepository.MarkPublished(document, publishedAtUtc);

        // A single SaveChangesAsync call commits the version's publication
        // state and the document's status transition as one atomic unit of
        // work, without touching any other version's row.
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
