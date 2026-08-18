namespace ProjectLoop.Engagement.Core;

public interface IMilestoneRepository
{
    Task<IReadOnlyList<Milestone>> ListByProjectAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default);
}
