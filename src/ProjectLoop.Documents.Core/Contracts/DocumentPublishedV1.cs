namespace ProjectLoop.Documents.Core;

/// <summary>
/// v1 payload for the DocumentPublished integration event: the completed
/// fact that a specific immutable DocumentVersion became publication-
/// eligible. Carries no Blob object key — access is always re-authorized by
/// the owning consumer rather than inferred from this event.
/// </summary>
public sealed record DocumentPublishedV1(
    Guid DocumentId,
    Guid DocumentVersionId,
    int VersionNumber,
    Guid ProjectId,
    Guid TenantId,
    string Category,
    DocumentVisibility Visibility,
    DateTimeOffset PublishedAtUtc);
