namespace ProjectLoop.Documents.Core;

/// <summary>
/// Safe catalog-row metadata for one document. Never carries a Blob object
/// key/URL — download access is authorized and issued separately per
/// request, not derived from this response.
/// </summary>
public sealed class DocumentSummaryResponse
{
    public required Guid Id { get; init; }

    public required Guid ProjectId { get; init; }

    public required string Title { get; init; }

    public required string Category { get; init; }

    public required DocumentStatus Status { get; init; }

    public required DocumentVisibility Visibility { get; init; }

    public required bool HasCurrentVersion { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; init; }
}
