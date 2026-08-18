namespace ProjectLoop.Identity.Core;

public enum InvitationAcceptanceError
{
    InvalidToken,
    Expired,
    AlreadyAccepted,
    Revoked,
    EmailMismatch,
}

public sealed class InvitationAcceptanceResult
{
    private InvitationAcceptanceResult(TenantMembership? membership, InvitationAcceptanceError? error)
    {
        Membership = membership;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public TenantMembership? Membership { get; }

    public InvitationAcceptanceError? Error { get; }

    public static InvitationAcceptanceResult Success(TenantMembership membership) => new(membership, null);

    public static InvitationAcceptanceResult Failure(InvitationAcceptanceError error) => new(null, error);
}
