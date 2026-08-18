namespace ProjectLoop.Engagement.Core;

public enum ProjectHealthFacadeError
{
    NoTenantContext,
    NotFound,
}

public sealed class ProjectHealthFacadeResult
{
    private ProjectHealthFacadeResult(ProjectHealthResponse? response, ProjectHealthFacadeError? error)
    {
        Response = response;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public ProjectHealthResponse? Response { get; }

    public ProjectHealthFacadeError? Error { get; }

    public static ProjectHealthFacadeResult Success(ProjectHealthResponse response) => new(response, null);

    public static ProjectHealthFacadeResult Failure(ProjectHealthFacadeError error) => new(null, error);
}
