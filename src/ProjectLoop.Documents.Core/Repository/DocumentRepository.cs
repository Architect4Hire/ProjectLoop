using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Documents.Core;

public sealed class DocumentRepository : IDocumentRepository
{
    private readonly DocumentsDbContext _dbContext;

    public DocumentRepository(DocumentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Document document, CancellationToken cancellationToken = default)
    {
        _dbContext.Documents.Add(document);
        return Task.CompletedTask;
    }

    public void SetCurrentVersion(Document document, Guid versionId, DateTimeOffset updatedAtUtc)
    {
        if (_dbContext.Entry(document).State == EntityState.Detached)
        {
            _dbContext.Documents.Attach(document);
        }

        document.CurrentVersionId = versionId;
        document.UpdatedAtUtc = updatedAtUtc;
    }

    public async Task<(IReadOnlyList<Document> Items, int TotalCount)> ListAsync(
        Guid tenantId,
        DocumentListQuery query,
        CancellationToken cancellationToken = default)
    {
        var filtered = _dbContext.Documents
            .Where(d => d.TenantId == tenantId && d.ProjectId == query.ProjectId);

        if (query.Category is not null)
        {
            filtered = filtered.Where(d => d.Category == query.Category);
        }

        if (query.Status is { } status)
        {
            filtered = filtered.Where(d => d.Status == status);
        }

        if (query.Visibility is { } visibility)
        {
            filtered = filtered.Where(d => d.Visibility == visibility);
        }

        var totalCount = await filtered.CountAsync(cancellationToken);

        var pageSize = Math.Clamp(query.PageSize, 1, DocumentListQuery.MaxPageSize);
        var page = Math.Max(query.Page, 1);

        var items = await filtered
            .OrderByDescending(d => d.UpdatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
