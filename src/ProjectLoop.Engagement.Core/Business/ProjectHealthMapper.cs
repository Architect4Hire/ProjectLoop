namespace ProjectLoop.Engagement.Core;

public static class ProjectHealthMapper
{
    public static ProjectHealthResponse ToHealthResponse(Project project) =>
        new()
        {
            ProjectId = project.Id,
            Status = project.Status,
            Health = project.Health,
            UpdatedAtUtc = project.UpdatedAtUtc,
        };
}
