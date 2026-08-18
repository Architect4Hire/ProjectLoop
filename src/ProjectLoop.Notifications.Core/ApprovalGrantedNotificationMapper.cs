namespace ProjectLoop.Notifications.Core;

/// <summary>
/// Maps an ApprovalGranted integration event to the recipient intent and
/// content for its notification. The event carries ApproverUserId but not
/// the original request's requester, so the notification recipient is
/// currently the approver (a decision receipt). Notifying the original
/// requester of the outcome requires resolving that identity — either by
/// extending the event contract via ADR or through an additional lookup —
/// which is out of this consumer's boundary and is not yet wired.
/// </summary>
public static class ApprovalGrantedNotificationMapper
{
    public static NotificationEnvelope Map(ApprovalGrantedV1 data) => new(
        TenantId: data.TenantId,
        RecipientUserId: data.ApproverUserId,
        Subject: $"Approval granted: {data.TargetType} {data.TargetId}",
        Body: $"The approval request for {data.TargetType} {data.TargetId} in project {data.ProjectId} was granted.");
}
