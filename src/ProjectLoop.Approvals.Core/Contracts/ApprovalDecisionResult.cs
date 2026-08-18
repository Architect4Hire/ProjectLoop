namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalDecisionResult
{
    public required Guid ApprovalRequestId { get; init; }

    public required Guid DecisionId { get; init; }

    public required ApprovalRequestStatus Status { get; init; }

    public required string ApproverUserId { get; init; }

    public required DateTimeOffset DecidedAtUtc { get; init; }
}
