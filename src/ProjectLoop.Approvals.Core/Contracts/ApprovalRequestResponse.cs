namespace ProjectLoop.Approvals.Core;

/// <summary>
/// The information a caller reviewing one approval request needs: the exact
/// target resource identity and, when the target is version-bound, the exact
/// immutable version the request was raised against. Never carries the
/// target's own content/metadata — that remains owned and authorized by the
/// target's own service.
/// </summary>
public sealed class ApprovalRequestResponse
{
    public required Guid Id { get; init; }

    public required Guid ProjectId { get; init; }

    public required string TargetType { get; init; }

    public required Guid TargetId { get; init; }

    public Guid? TargetVersionId { get; init; }

    public required ApprovalRequestStatus Status { get; init; }

    public required string RequestedByUserId { get; init; }

    public required DateTimeOffset RequestedAtUtc { get; init; }
}
