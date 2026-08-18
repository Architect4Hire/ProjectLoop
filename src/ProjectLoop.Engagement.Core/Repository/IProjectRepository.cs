namespace ProjectLoop.Engagement.Core;

public interface IProjectRepository
{
    Task<Project?> FindByIdAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default);
}
