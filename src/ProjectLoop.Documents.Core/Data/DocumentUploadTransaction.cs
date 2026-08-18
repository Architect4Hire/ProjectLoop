namespace ProjectLoop.Documents.Core;

public sealed class DocumentUploadTransaction : IDocumentUploadTransaction
{
    private readonly DocumentsDbContext _dbContext;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _documentVersionRepository;

    public DocumentUploadTransaction(
        DocumentsDbContext dbContext,
        IDocumentRepository documentRepository,
        IDocumentVersionRepository documentVersionRepository)
    {
        _dbContext = dbContext;
        _documentRepository = documentRepository;
        _documentVersionRepository = documentVersionRepository;
    }

    public async Task ExecuteAsync(Document document, DocumentVersion firstVersion, CancellationToken cancellationToken = default)
    {
        await _documentRepository.AddAsync(document, cancellationToken);
        await _documentVersionRepository.AddAsync(firstVersion, cancellationToken);
        _documentRepository.SetCurrentVersion(document, firstVersion.Id, firstVersion.CreatedAtUtc);

        // A single SaveChangesAsync call commits the staged Document,
        // DocumentVersion and pointer update as one atomic unit of work.
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
