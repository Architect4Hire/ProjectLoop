namespace ProjectLoop.Engagement.Core;

public enum ProjectMilestonesFacadeError
{
    NoTenantContext,
    NotFound,
}

public sealed class ProjectMilestonesFacadeResult
{
    private ProjectMilestonesFacadeResult(IReadOnlyList<MilestoneSummaryResponse>? milestones, ProjectMilestonesFacadeError? error)
    {
        Milestones = milestones;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public IReadOnlyList<MilestoneSummaryResponse>? Milestones { get; }

    public ProjectMilestonesFacadeError? Error { get; }

    public static ProjectMilestonesFacadeResult Success(IReadOnlyList<MilestoneSummaryResponse> milestones) => new(milestones, null);

    public static ProjectMilestonesFacadeResult Failure(ProjectMilestonesFacadeError error) => new(null, error);
}
