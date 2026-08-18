namespace ProjectLoop.Documents.Core;

/// <summary>
/// Use-case validation and orchestration for downloading a document version:
/// resolves the document and version scoped to the caller's tenant,
/// authorizes visibility via <see cref="DocumentDownloadAuthorization"/>, and
/// only then opens the Blob stream. Authorization always completes before
/// any Blob read is issued.
/// </summary>
public sealed class DocumentDownloadFacade : IDocumentDownloadFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _documentVersionRepository;
    private readonly IBlobDocumentStore _blobStore;

    public DocumentDownloadFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IDocumentRepository documentRepository,
        IDocumentVersionRepository documentVersionRepository,
        IBlobDocumentStore blobStore)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _documentRepository = documentRepository;
        _documentVersionRepository = documentVersionRepository;
        _blobStore = blobStore;
    }

    public async Task<DocumentDownloadFacadeResult> DownloadAsync(
        Guid documentId,
        Guid versionId,
        CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return DocumentDownloadFacadeResult.Failure(DocumentDownloadFacadeError.NoTenantContext);
        }

        var document = await _documentRepository.GetByIdAsync(tenantContext.TenantId, documentId, cancellationToken);
        if (document is null)
        {
            return DocumentDownloadFacadeResult.Failure(DocumentDownloadFacadeError.NotFound);
        }

        var version = await _documentVersionRepository.GetByIdAsync(document.Id, versionId, cancellationToken);
        if (version is null)
        {
            return DocumentDownloadFacadeResult.Failure(DocumentDownloadFacadeError.NotFound);
        }

        if (!DocumentDownloadAuthorization.IsAuthorized(document, version, tenantContext.IsClientUser))
        {
            return DocumentDownloadFacadeResult.Failure(DocumentDownloadFacadeError.Forbidden);
        }

        var content = await _blobStore.OpenReadAsync(version.BlobObjectKey, cancellationToken);

        var descriptor = new DocumentDownloadDescriptor
        {
            DocumentId = document.Id,
            VersionId = version.Id,
            VersionNumber = version.VersionNumber,
            FileName = document.Title,
            ContentType = version.MimeType,
            ContentLength = version.SizeBytes,
        };

        return DocumentDownloadFacadeResult.Success(descriptor, content);
    }
}
