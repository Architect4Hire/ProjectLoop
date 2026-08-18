namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Use-case orchestration for project detail lookup. Tenant scope comes only
/// from the server-established <see cref="ICurrentTenantContextAccessor"/>,
/// never from the request payload/route, and a project owned by a different
/// tenant is reported as not found rather than forbidden.
/// </summary>
public sealed class ProjectDetailFacade : IProjectDetailFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IProjectRepository _repository;

    public ProjectDetailFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IProjectRepository repository)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _repository = repository;
    }

    public async Task<ProjectDetailFacadeResult> GetProjectDetailAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return ProjectDetailFacadeResult.Failure(ProjectDetailFacadeError.NoTenantContext);
        }

        var project = await _repository.FindByIdAsync(tenantContext.TenantId, projectId, cancellationToken);
        if (project is null)
        {
            return ProjectDetailFacadeResult.Failure(ProjectDetailFacadeError.NotFound);
        }

        return ProjectDetailFacadeResult.Success(ProjectMapper.ToDetailResponse(project));
    }
}
