namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Pure rule for whether an approval outcome targets a specific milestone
/// and, if so, which milestone transition is eligible. No persistence or
/// I/O happens here — this is the decision only, applied by a consumer
/// transaction against durable state.
/// </summary>
public static class MilestoneApprovalTargetMatcher
{
    public const string MilestoneTargetType = "Milestone";

    public static bool TargetsMilestone(string targetType, Guid targetId, Guid milestoneId) =>
        string.Equals(targetType, MilestoneTargetType, StringComparison.Ordinal) && targetId == milestoneId;

    public static MilestoneStatus? GetEligibleStatusOnGranted(MilestoneStatus currentStatus) =>
        currentStatus == MilestoneStatus.Completed ? null : MilestoneStatus.Completed;

    public static MilestoneStatus? GetEligibleStatusOnRejected(MilestoneStatus currentStatus) =>
        currentStatus == MilestoneStatus.AtRisk ? null : MilestoneStatus.AtRisk;
}
