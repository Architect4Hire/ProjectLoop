namespace ProjectLoop.Documents.Core;

/// <summary>
/// Identifies the existing Document a new version's binary/metadata is being
/// added to. Tenant ownership is never taken from this contract — it is
/// always derived server-side from the authenticated
/// <see cref="ICurrentTenantContextAccessor"/>.
/// </summary>
public sealed class AddDocumentVersionRequest
{
    public required Guid DocumentId { get; init; }
}
