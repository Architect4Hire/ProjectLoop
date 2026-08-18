namespace ProjectLoop.Documents.Core;

/// <summary>
/// Filter/pagination parameters for browsing a project's document catalog.
/// Tenant scope is never part of this contract — it is always derived
/// server-side from the authenticated <see cref="ICurrentTenantContextAccessor"/>.
/// </summary>
public sealed class DocumentListQuery
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public required Guid ProjectId { get; init; }

    public string? Category { get; init; }

    public DocumentStatus? Status { get; init; }

    public DocumentVisibility? Visibility { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;
}
