namespace ProjectLoop.Documents.Core;

/// <summary>
/// Identifies the exact Document and DocumentVersion to publish to client
/// visibility. Tenant ownership is never taken from this contract — it is
/// always derived server-side from the authenticated
/// <see cref="ICurrentTenantContextAccessor"/>.
/// </summary>
public sealed class PublishDocumentVersionRequest
{
    public required Guid DocumentId { get; init; }

    public required Guid VersionId { get; init; }
}
