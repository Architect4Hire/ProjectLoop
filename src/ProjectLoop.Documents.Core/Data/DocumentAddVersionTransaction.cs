namespace ProjectLoop.Documents.Core;

public sealed class DocumentAddVersionTransaction : IDocumentAddVersionTransaction
{
    private readonly DocumentsDbContext _dbContext;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _documentVersionRepository;

    public DocumentAddVersionTransaction(
        DocumentsDbContext dbContext,
        IDocumentRepository documentRepository,
        IDocumentVersionRepository documentVersionRepository)
    {
        _dbContext = dbContext;
        _documentRepository = documentRepository;
        _documentVersionRepository = documentVersionRepository;
    }

    public async Task<DocumentVersion> ExecuteAsync(
        Document document,
        Guid versionId,
        string blobObjectKey,
        string mimeType,
        long sizeBytes,
        string contentHash,
        string uploadedByUserId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var versionNumber = await _documentVersionRepository.GetNextVersionNumberAsync(document.Id, cancellationToken);

        var version = new DocumentVersion
        {
            Id = versionId,
            DocumentId = document.Id,
            VersionNumber = versionNumber,
            BlobObjectKey = blobObjectKey,
            MimeType = mimeType,
            SizeBytes = sizeBytes,
            ContentHash = contentHash,
            UploadedByUserId = uploadedByUserId,
            CreatedAtUtc = nowUtc,
        };

        await _documentVersionRepository.AddAsync(version, cancellationToken);
        _documentRepository.SetCurrentVersion(document, version.Id, nowUtc);

        // A single SaveChangesAsync call commits the new DocumentVersion and
        // the pointer update as one atomic unit of work, leaving every prior
        // version row untouched.
        await _dbContext.SaveChangesAsync(cancellationToken);

        return version;
    }
}
