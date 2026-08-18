namespace ProjectLoop.Engagement.Core;

public static class ProjectMapper
{
    public static ProjectDetailResponse ToDetailResponse(Project project) =>
        new()
        {
            Id = project.Id,
            Name = project.Name,
            Status = project.Status,
            Health = project.Health,
            UpdatedAtUtc = project.UpdatedAtUtc,
        };
}
