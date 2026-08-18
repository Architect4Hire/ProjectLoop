namespace ProjectLoop.Engagement.Core;

public enum ProjectDetailFacadeError
{
    NoTenantContext,
    NotFound,
}

public sealed class ProjectDetailFacadeResult
{
    private ProjectDetailFacadeResult(ProjectDetailResponse? response, ProjectDetailFacadeError? error)
    {
        Response = response;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public ProjectDetailResponse? Response { get; }

    public ProjectDetailFacadeError? Error { get; }

    public static ProjectDetailFacadeResult Success(ProjectDetailResponse response) => new(response, null);

    public static ProjectDetailFacadeResult Failure(ProjectDetailFacadeError error) => new(null, error);
}
