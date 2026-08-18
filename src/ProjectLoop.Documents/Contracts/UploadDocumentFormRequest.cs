using ProjectLoop.Documents.Core;

namespace ProjectLoop.Documents.Contracts;

/// <summary>
/// Multipart form binding for a document upload. The binary content travels
/// as <see cref="File"/>; every other field is non-binary metadata.
/// </summary>
public sealed class UploadDocumentFormRequest
{
    public required IFormFile File { get; init; }

    public required Guid ProjectId { get; init; }

    public required string Title { get; init; }

    public required string Category { get; init; }

    public required DocumentVisibility Visibility { get; init; }
}
