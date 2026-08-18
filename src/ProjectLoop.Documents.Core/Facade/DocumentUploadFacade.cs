namespace ProjectLoop.Documents.Core;

/// <summary>
/// Use-case validation and orchestration for document upload: authorizes
/// tenant, validates metadata/file, stores the binary in private Blob
/// Storage, then commits the SQL metadata transaction. If the SQL write
/// fails after the blob was stored, the now-orphaned blob is deleted so it
/// does not linger as unreferenced content.
/// </summary>
public sealed class DocumentUploadFacade : IDocumentUploadFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IDocumentUploadSizeValidator _sizeValidator;
    private readonly IBlobDocumentStore _blobStore;
    private readonly IDocumentUploadTransaction _uploadTransaction;
    private readonly TimeProvider _timeProvider;

    public DocumentUploadFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IDocumentUploadSizeValidator sizeValidator,
        IBlobDocumentStore blobStore,
        IDocumentUploadTransaction uploadTransaction,
        TimeProvider? timeProvider = null)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _sizeValidator = sizeValidator;
        _blobStore = blobStore;
        _uploadTransaction = uploadTransaction;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<DocumentUploadFacadeResult> UploadAsync(
        UploadDocumentMetadata metadata,
        Stream content,
        string originalFileName,
        string mimeType,
        CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return DocumentUploadFacadeResult.Failure(DocumentUploadFacadeError.NoTenantContext);
        }

        if (string.IsNullOrWhiteSpace(metadata.Title) || string.IsNullOrWhiteSpace(metadata.Category))
        {
            return DocumentUploadFacadeResult.Failure(DocumentUploadFacadeError.InvalidMetadata);
        }

        if (!DocumentFileNamePolicy.TryNormalize(originalFileName, out _))
        {
            return DocumentUploadFacadeResult.Failure(DocumentUploadFacadeError.InvalidFileName);
        }

        if (!DocumentMimeTypePolicy.IsAllowed(mimeType))
        {
            return DocumentUploadFacadeResult.Failure(DocumentUploadFacadeError.UnsupportedMediaType);
        }

        using var buffered = new MemoryStream();
        await content.CopyToAsync(buffered, cancellationToken);

        if (!_sizeValidator.IsWithinLimit(buffered.Length))
        {
            return DocumentUploadFacadeResult.Failure(DocumentUploadFacadeError.PayloadTooLarge);
        }

        buffered.Position = 0;
        var contentHash = await DocumentContentHasher.ComputeHashAsync(buffered, cancellationToken);

        buffered.Position = 0;
        var objectKey = await _blobStore.PutAsync(buffered, mimeType, cancellationToken);

        var now = _timeProvider.GetUtcNow();
        var document = new Document
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            ProjectId = metadata.ProjectId,
            Title = metadata.Title,
            Category = metadata.Category,
            Status = DocumentStatus.Draft,
            Visibility = metadata.Visibility,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
        var version = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            VersionNumber = 1,
            BlobObjectKey = objectKey,
            MimeType = mimeType,
            SizeBytes = buffered.Length,
            ContentHash = contentHash,
            UploadedByUserId = tenantContext.UserId,
            CreatedAtUtc = now,
        };

        try
        {
            await _uploadTransaction.ExecuteAsync(document, version, cancellationToken);
        }
        catch
        {
            await _blobStore.DeleteIfOrphanAsync(objectKey, cancellationToken);
            return DocumentUploadFacadeResult.Failure(DocumentUploadFacadeError.PersistenceFailed);
        }

        return DocumentUploadFacadeResult.Success(new UploadDocumentResult
        {
            DocumentId = document.Id,
            VersionId = version.Id,
            VersionNumber = version.VersionNumber,
            Title = document.Title,
            Category = document.Category,
            Status = document.Status,
            Visibility = document.Visibility,
            SizeBytes = version.SizeBytes,
            CreatedAtUtc = document.CreatedAtUtc,
        });
    }
}
