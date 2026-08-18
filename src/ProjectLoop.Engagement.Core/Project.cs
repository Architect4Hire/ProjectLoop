namespace ProjectLoop.Engagement.Core;

public enum ProjectStatus
{
    Active,
    OnHold,
    Completed,
    Archived,
}

public enum ProjectHealth
{
    Green,
    Amber,
    Red,
}

public sealed class Project
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required string Name { get; init; }

    public required ProjectStatus Status { get; set; }

    public required ProjectHealth Health { get; set; }

    public required DateTimeOffset CreatedAtUtc { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; set; }
}
