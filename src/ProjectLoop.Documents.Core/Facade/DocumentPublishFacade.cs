namespace ProjectLoop.Documents.Core;

/// <summary>
/// Use-case validation and orchestration for publishing a document version to
/// client visibility: only internal users may publish (see
/// ADR-014-internal-client-user-classification), the target document and
/// version must exist within the caller's tenant, and the version must be
/// eligible per <see cref="DocumentPublicationEligibility"/>.
/// </summary>
public sealed class DocumentPublishFacade : IDocumentPublishFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _documentVersionRepository;
    private readonly IDocumentPublishTransaction _publishTransaction;
    private readonly TimeProvider _timeProvider;

    public DocumentPublishFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IDocumentRepository documentRepository,
        IDocumentVersionRepository documentVersionRepository,
        IDocumentPublishTransaction publishTransaction,
        TimeProvider? timeProvider = null)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _documentRepository = documentRepository;
        _documentVersionRepository = documentVersionRepository;
        _publishTransaction = publishTransaction;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<DocumentPublishFacadeResult> PublishAsync(
        PublishDocumentVersionRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return DocumentPublishFacadeResult.Failure(DocumentPublishFacadeError.NoTenantContext);
        }

        if (tenantContext.IsClientUser)
        {
            return DocumentPublishFacadeResult.Failure(DocumentPublishFacadeError.Forbidden);
        }

        var document = await _documentRepository.GetByIdAsync(tenantContext.TenantId, request.DocumentId, cancellationToken);
        if (document is null)
        {
            return DocumentPublishFacadeResult.Failure(DocumentPublishFacadeError.DocumentNotFound);
        }

        var version = await _documentVersionRepository.GetByIdAsync(document.Id, request.VersionId, cancellationToken);
        if (version is null)
        {
            return DocumentPublishFacadeResult.Failure(DocumentPublishFacadeError.VersionNotFound);
        }

        if (!DocumentPublicationEligibility.IsEligible(document, version))
        {
            return DocumentPublishFacadeResult.Failure(DocumentPublishFacadeError.NotEligible);
        }

        var now = _timeProvider.GetUtcNow();
        await _publishTransaction.ExecuteAsync(document, version, now, cancellationToken);

        return DocumentPublishFacadeResult.Success(new PublishDocumentVersionResult
        {
            DocumentId = document.Id,
            VersionId = version.Id,
            VersionNumber = version.VersionNumber,
            Status = document.Status,
            PublishedAtUtc = now,
        });
    }
}
