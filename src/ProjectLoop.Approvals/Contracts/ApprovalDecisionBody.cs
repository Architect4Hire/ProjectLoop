namespace ProjectLoop.Approvals.Contracts;

/// <summary>
/// JSON body for an approve/reject action. The target ApprovalRequest is
/// identified by the route, not this body.
/// </summary>
public sealed class ApprovalDecisionBody
{
    public string? Comments { get; init; }
}
