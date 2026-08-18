namespace ProjectLoop.Approvals.Core;

public enum ApprovalDecisionFacadeError
{
    NoTenantContext,
    RequestNotFound,
    Conflict,
}

public sealed class ApprovalDecisionFacadeResult
{
    private ApprovalDecisionFacadeResult(ApprovalDecisionResult? result, ApprovalDecisionFacadeError? error)
    {
        Result = result;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public ApprovalDecisionResult? Result { get; }

    public ApprovalDecisionFacadeError? Error { get; }

    public static ApprovalDecisionFacadeResult Success(ApprovalDecisionResult result) => new(result, null);

    public static ApprovalDecisionFacadeResult Failure(ApprovalDecisionFacadeError error) => new(null, error);
}
