namespace ProjectLoop.Engagement.Core;

public sealed class MilestoneSummaryResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required MilestoneStatus Status { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; init; }
}
