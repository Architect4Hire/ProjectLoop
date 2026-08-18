namespace ProjectLoop.Documents.Core;

/// <summary>
/// Use-case validation and orchestration for adding a new version to an
/// existing document: authorizes tenant ownership of the target document,
/// validates the file, stores the binary in private Blob Storage, then
/// commits the SQL transaction. If the SQL write fails after the blob was
/// stored, the now-orphaned blob is deleted so it does not linger as
/// unreferenced content. Never mutates a prior version's row.
/// </summary>
public sealed class DocumentAddVersionFacade : IDocumentAddVersionFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentUploadSizeValidator _sizeValidator;
    private readonly IBlobDocumentStore _blobStore;
    private readonly IDocumentAddVersionTransaction _addVersionTransaction;
    private readonly TimeProvider _timeProvider;

    public DocumentAddVersionFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IDocumentRepository documentRepository,
        IDocumentUploadSizeValidator sizeValidator,
        IBlobDocumentStore blobStore,
        IDocumentAddVersionTransaction addVersionTransaction,
        TimeProvider? timeProvider = null)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _documentRepository = documentRepository;
        _sizeValidator = sizeValidator;
        _blobStore = blobStore;
        _addVersionTransaction = addVersionTransaction;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<DocumentAddVersionFacadeResult> AddVersionAsync(
        AddDocumentVersionRequest request,
        Stream content,
        string originalFileName,
        string mimeType,
        CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return DocumentAddVersionFacadeResult.Failure(DocumentAddVersionFacadeError.NoTenantContext);
        }

        var document = await _documentRepository.GetByIdAsync(tenantContext.TenantId, request.DocumentId, cancellationToken);
        if (document is null)
        {
            return DocumentAddVersionFacadeResult.Failure(DocumentAddVersionFacadeError.DocumentNotFound);
        }

        if (!DocumentFileNamePolicy.TryNormalize(originalFileName, out _))
        {
            return DocumentAddVersionFacadeResult.Failure(DocumentAddVersionFacadeError.InvalidFileName);
        }

        if (!DocumentMimeTypePolicy.IsAllowed(mimeType))
        {
            return DocumentAddVersionFacadeResult.Failure(DocumentAddVersionFacadeError.UnsupportedMediaType);
        }

        using var buffered = new MemoryStream();
        await content.CopyToAsync(buffered, cancellationToken);

        if (!_sizeValidator.IsWithinLimit(buffered.Length))
        {
            return DocumentAddVersionFacadeResult.Failure(DocumentAddVersionFacadeError.PayloadTooLarge);
        }

        buffered.Position = 0;
        var contentHash = await DocumentContentHasher.ComputeHashAsync(buffered, cancellationToken);

        buffered.Position = 0;
        var objectKey = await _blobStore.PutAsync(buffered, mimeType, cancellationToken);

        var now = _timeProvider.GetUtcNow();
        DocumentVersion version;
        try
        {
            version = await _addVersionTransaction.ExecuteAsync(
                document,
                Guid.NewGuid(),
                objectKey,
                mimeType,
                buffered.Length,
                contentHash,
                tenantContext.UserId,
                now,
                cancellationToken);
        }
        catch
        {
            await _blobStore.DeleteIfOrphanAsync(objectKey, cancellationToken);
            return DocumentAddVersionFacadeResult.Failure(DocumentAddVersionFacadeError.PersistenceFailed);
        }

        return DocumentAddVersionFacadeResult.Success(new AddDocumentVersionResult
        {
            DocumentId = document.Id,
            VersionId = version.Id,
            VersionNumber = version.VersionNumber,
            MimeType = version.MimeType,
            SizeBytes = version.SizeBytes,
            CreatedAtUtc = version.CreatedAtUtc,
        });
    }
}
