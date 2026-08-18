namespace ProjectLoop.Documents.Core;

/// <summary>
/// Safe result of adding a new document version. Never carries a Blob object
/// key/URL — download access is authorized and issued separately per
/// request, not derived from this response.
/// </summary>
public sealed class AddDocumentVersionResult
{
    public required Guid DocumentId { get; init; }

    public required Guid VersionId { get; init; }

    public required int VersionNumber { get; init; }

    public required string MimeType { get; init; }

    public required long SizeBytes { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }
}
