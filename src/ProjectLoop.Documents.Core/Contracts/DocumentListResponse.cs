namespace ProjectLoop.Documents.Core;

public sealed class DocumentListResponse
{
    public required IReadOnlyList<DocumentSummaryResponse> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }
}
