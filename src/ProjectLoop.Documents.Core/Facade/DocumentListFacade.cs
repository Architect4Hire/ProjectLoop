namespace ProjectLoop.Documents.Core;

/// <summary>
/// Use-case orchestration for the project document catalog. Tenant scope
/// comes only from the server-established
/// <see cref="ICurrentTenantContextAccessor"/>, never from the query.
/// </summary>
public sealed class DocumentListFacade : IDocumentListFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IDocumentRepository _documentRepository;

    public DocumentListFacade(ICurrentTenantContextAccessor tenantContextAccessor, IDocumentRepository documentRepository)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _documentRepository = documentRepository;
    }

    public async Task<DocumentListFacadeResult> ListAsync(DocumentListQuery query, CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return DocumentListFacadeResult.Failure(DocumentListFacadeError.NoTenantContext);
        }

        var (items, totalCount) = await _documentRepository.ListAsync(tenantContext.TenantId, query, cancellationToken);

        return DocumentListFacadeResult.Success(DocumentSummaryMapper.ToListResponse(items, totalCount, query.Page, query.PageSize));
    }
}
