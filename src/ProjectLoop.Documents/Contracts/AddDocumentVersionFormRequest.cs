namespace ProjectLoop.Documents.Contracts;

/// <summary>
/// Multipart form binding for adding a new version to an existing document.
/// The binary content travels as <see cref="File"/>; the target document is
/// identified by the route, not this body.
/// </summary>
public sealed class AddDocumentVersionFormRequest
{
    public required IFormFile File { get; init; }
}
