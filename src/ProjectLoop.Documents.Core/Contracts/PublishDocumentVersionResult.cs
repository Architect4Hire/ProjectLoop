namespace ProjectLoop.Documents.Core;

public sealed class PublishDocumentVersionResult
{
    public required Guid DocumentId { get; init; }

    public required Guid VersionId { get; init; }

    public required int VersionNumber { get; init; }

    public required DocumentStatus Status { get; init; }

    public required DateTimeOffset PublishedAtUtc { get; init; }
}
