namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Approvals' consumer-side shape for the DocumentPublished v1 integration
/// event produced by the Documents service. Declared locally rather than
/// referencing ProjectLoop.Documents.Core so Approvals only depends on the
/// versioned wire contract, not another service's internals — only the
/// fields Approvals' policy and approval-request creation actually need are
/// declared here.
/// </summary>
public sealed record DocumentPublishedV1(
    Guid DocumentId,
    Guid DocumentVersionId,
    int VersionNumber,
    Guid ProjectId,
    Guid TenantId,
    string Category,
    DocumentPublishedVisibility Visibility,
    DateTimeOffset PublishedAtUtc);
