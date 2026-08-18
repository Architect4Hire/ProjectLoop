namespace ProjectLoop.Identity.Core;

public enum InvitationCreationFacadeError
{
    NoTenantContext,
    NotAuthorized,
    InvalidEmail,
}

public sealed class InvitationCreationFacadeResult
{
    private InvitationCreationFacadeResult(InvitationCreationResult? result, InvitationCreationFacadeError? error)
    {
        Result = result;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public InvitationCreationResult? Result { get; }

    public InvitationCreationFacadeError? Error { get; }

    public static InvitationCreationFacadeResult Success(InvitationCreationResult result) => new(result, null);

    public static InvitationCreationFacadeResult Failure(InvitationCreationFacadeError error) => new(null, error);
}
