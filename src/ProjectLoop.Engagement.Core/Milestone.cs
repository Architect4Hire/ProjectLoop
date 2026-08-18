namespace ProjectLoop.Engagement.Core;

public enum MilestoneStatus
{
    Planned,
    InProgress,
    Completed,
    AtRisk,
}

public sealed class Milestone
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid ProjectId { get; init; }

    public required string Name { get; init; }

    public required MilestoneStatus Status { get; set; }

    public required DateTimeOffset CreatedAtUtc { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; set; }
}
