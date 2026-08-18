namespace ProjectLoop.Approvals.Core;

public enum ApprovalRequestReadFacadeError
{
    NoTenantContext,
    NotFound,
}

public sealed class ApprovalRequestReadFacadeResult
{
    private ApprovalRequestReadFacadeResult(ApprovalRequestResponse? response, ApprovalRequestReadFacadeError? error)
    {
        Response = response;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public ApprovalRequestResponse? Response { get; }

    public ApprovalRequestReadFacadeError? Error { get; }

    public static ApprovalRequestReadFacadeResult Success(ApprovalRequestResponse response) => new(response, null);

    public static ApprovalRequestReadFacadeResult Failure(ApprovalRequestReadFacadeError error) => new(null, error);
}
