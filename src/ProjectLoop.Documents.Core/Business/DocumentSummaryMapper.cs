namespace ProjectLoop.Documents.Core;

public static class DocumentSummaryMapper
{
    public static DocumentSummaryResponse ToSummaryResponse(Document document) => new()
    {
        Id = document.Id,
        ProjectId = document.ProjectId,
        Title = document.Title,
        Category = document.Category,
        Status = document.Status,
        Visibility = document.Visibility,
        HasCurrentVersion = document.CurrentVersionId is not null,
        UpdatedAtUtc = document.UpdatedAtUtc,
    };

    public static DocumentListResponse ToListResponse(
        IReadOnlyList<Document> documents,
        int totalCount,
        int page,
        int pageSize) => new()
    {
        Items = documents.Select(ToSummaryResponse).ToList(),
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
    };
}
