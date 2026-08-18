namespace ProjectLoop.Documents.Core;

/// <summary>
/// Default-deny allow-list of MIME types accepted for document upload.
/// Content type is validated by declared value here; deeper content
/// inspection/quarantine scanning is a separate upload-lifecycle concern.
/// </summary>
public static class DocumentMimeTypePolicy
{
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain",
        "text/csv",
        "image/png",
        "image/jpeg",
    };

    public static bool IsAllowed(string? mimeType) =>
        !string.IsNullOrWhiteSpace(mimeType) && AllowedMimeTypes.Contains(mimeType);
}
