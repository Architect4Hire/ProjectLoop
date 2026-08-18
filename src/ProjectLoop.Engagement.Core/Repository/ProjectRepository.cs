using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Engagement.Core;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly EngagementDbContext _dbContext;

    public ProjectRepository(EngagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Project?> FindByIdAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default) =>
        _dbContext.Projects.SingleOrDefaultAsync(p => p.TenantId == tenantId && p.Id == projectId, cancellationToken);
}
