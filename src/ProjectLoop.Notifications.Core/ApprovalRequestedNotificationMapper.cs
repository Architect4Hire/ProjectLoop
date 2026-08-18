namespace ProjectLoop.Notifications.Core;

/// <summary>
/// Maps an ApprovalRequested integration event to the recipient intent and
/// content for its notification. The event carries only RequestedByUserId
/// — it does not identify who is authorized to decide the request — so the
/// notification recipient is currently the requester (a submission
/// confirmation). Notifying the request's actual approver(s) requires
/// resolving project/role membership, which is out of this consumer's
/// boundary and is not yet wired; recipient resolution should be revisited
/// once that lookup exists.
/// </summary>
public static class ApprovalRequestedNotificationMapper
{
    public static NotificationEnvelope Map(ApprovalRequestedV1 data) => new(
        TenantId: data.TenantId,
        RecipientUserId: data.RequestedByUserId,
        Subject: $"Approval requested: {data.TargetType} {data.TargetId}",
        Body: $"An approval request was submitted for {data.TargetType} {data.TargetId} in project {data.ProjectId}.");
}
