namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Use-case orchestration for the project milestone list. Tenant scope comes
/// only from the server-established <see cref="ICurrentTenantContextAccessor"/>,
/// and a project owned by a different tenant is reported as not found rather
/// than forbidden.
/// </summary>
public sealed class ProjectMilestonesFacade : IProjectMilestonesFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IProjectRepository _projectRepository;
    private readonly IMilestoneRepository _milestoneRepository;

    public ProjectMilestonesFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IProjectRepository projectRepository,
        IMilestoneRepository milestoneRepository)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _projectRepository = projectRepository;
        _milestoneRepository = milestoneRepository;
    }

    public async Task<ProjectMilestonesFacadeResult> GetProjectMilestonesAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return ProjectMilestonesFacadeResult.Failure(ProjectMilestonesFacadeError.NoTenantContext);
        }

        var project = await _projectRepository.FindByIdAsync(tenantContext.TenantId, projectId, cancellationToken);
        if (project is null)
        {
            return ProjectMilestonesFacadeResult.Failure(ProjectMilestonesFacadeError.NotFound);
        }

        var milestones = await _milestoneRepository.ListByProjectAsync(tenantContext.TenantId, projectId, cancellationToken);

        return ProjectMilestonesFacadeResult.Success(MilestoneMapper.ToSummaryResponses(milestones));
    }
}
