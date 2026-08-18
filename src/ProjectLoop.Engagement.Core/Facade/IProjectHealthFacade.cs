namespace ProjectLoop.Engagement.Core;

public interface IProjectHealthFacade
{
    Task<ProjectHealthFacadeResult> GetProjectHealthAsync(Guid projectId, CancellationToken cancellationToken = default);
}
