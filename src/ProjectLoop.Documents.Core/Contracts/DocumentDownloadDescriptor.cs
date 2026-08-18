namespace ProjectLoop.Documents.Core;

/// <summary>
/// HTTP/content-disposition metadata for downloading an exact authorized
/// document version. Never carries a Blob object key/URL — the binary
/// stream is opened and returned separately, never derived from this
/// descriptor by the caller.
/// </summary>
public sealed class DocumentDownloadDescriptor
{
    public required Guid DocumentId { get; init; }

    public required Guid VersionId { get; init; }

    public required int VersionNumber { get; init; }

    public required string FileName { get; init; }

    public required string ContentType { get; init; }

    public required long ContentLength { get; init; }
}
