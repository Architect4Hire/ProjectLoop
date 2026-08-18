namespace ProjectLoop.Documents.Core;

public sealed class Document
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid ProjectId { get; init; }

    public required string Title { get; init; }

    public required string Category { get; init; }

    public required DocumentStatus Status { get; set; }

    public required DocumentVisibility Visibility { get; set; }

    public Guid? CurrentVersionId { get; set; }

    public required DateTimeOffset CreatedAtUtc { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; set; }
}
