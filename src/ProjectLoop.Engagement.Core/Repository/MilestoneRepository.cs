using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Engagement.Core;

public sealed class MilestoneRepository : IMilestoneRepository
{
    private readonly EngagementDbContext _dbContext;

    public MilestoneRepository(EngagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Milestone>> ListByProjectAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Milestones
            .Where(m => m.TenantId == tenantId && m.ProjectId == projectId)
            .ToListAsync(cancellationToken);
}
