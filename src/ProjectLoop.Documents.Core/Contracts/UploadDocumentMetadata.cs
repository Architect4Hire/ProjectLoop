namespace ProjectLoop.Documents.Core;

/// <summary>
/// Non-binary metadata supplied with a document upload. Tenant ownership is
/// never taken from this contract — it is always derived server-side from
/// the authenticated <see cref="ICurrentTenantContextAccessor"/>.
/// </summary>
public sealed class UploadDocumentMetadata
{
    public required Guid ProjectId { get; init; }

    public required string Title { get; init; }

    public required string Category { get; init; }

    public required DocumentVisibility Visibility { get; init; }
}
