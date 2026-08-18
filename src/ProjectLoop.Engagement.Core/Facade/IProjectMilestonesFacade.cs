namespace ProjectLoop.Engagement.Core;

public interface IProjectMilestonesFacade
{
    Task<ProjectMilestonesFacadeResult> GetProjectMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default);
}
