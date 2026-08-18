namespace ProjectLoop.Engagement.Core;

public sealed class ProjectDetailResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required ProjectStatus Status { get; init; }

    public required ProjectHealth Health { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; init; }
}
