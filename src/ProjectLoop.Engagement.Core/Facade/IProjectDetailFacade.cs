namespace ProjectLoop.Engagement.Core;

public interface IProjectDetailFacade
{
    Task<ProjectDetailFacadeResult> GetProjectDetailAsync(Guid projectId, CancellationToken cancellationToken = default);
}
