namespace ProjectLoop.Engagement.Core;

public sealed class ProjectHealthResponse
{
    public required Guid ProjectId { get; init; }

    public required ProjectStatus Status { get; init; }

    public required ProjectHealth Health { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; init; }
}
