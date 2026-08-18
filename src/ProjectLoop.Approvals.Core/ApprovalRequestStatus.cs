namespace ProjectLoop.Approvals.Core;

/// <summary>
/// The lifecycle state of an ApprovalRequest. Pending is the only state a
/// decision may transition out of; Approved and Rejected are terminal.
/// </summary>
public enum ApprovalRequestStatus
{
    Pending,
    Approved,
    Rejected,
}
