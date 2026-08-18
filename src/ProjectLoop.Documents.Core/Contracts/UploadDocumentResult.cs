namespace ProjectLoop.Documents.Core;

/// <summary>
/// Safe result of a completed document upload. Never carries a Blob object
/// key/URL — download access is authorized and issued separately per
/// request, not derived from this response.
/// </summary>
public sealed class UploadDocumentResult
{
    public required Guid DocumentId { get; init; }

    public required Guid VersionId { get; init; }

    public required int VersionNumber { get; init; }

    public required string Title { get; init; }

    public required string Category { get; init; }

    public required DocumentStatus Status { get; init; }

    public required DocumentVisibility Visibility { get; init; }

    public required long SizeBytes { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }
}
