namespace ProjectLoop.Documents.Core;

public sealed class DocumentVersion
{
    public required Guid Id { get; init; }

    public required Guid DocumentId { get; init; }

    public required int VersionNumber { get; init; }

    public required string BlobObjectKey { get; init; }

    public required string MimeType { get; init; }

    public required long SizeBytes { get; init; }

    public required string ContentHash { get; init; }

    public required string UploadedByUserId { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }

    public bool IsPublished { get; set; }

    public DateTimeOffset? PublishedAtUtc { get; set; }
}
