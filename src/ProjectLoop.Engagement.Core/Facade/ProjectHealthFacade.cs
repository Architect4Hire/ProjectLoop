namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Use-case orchestration for the project-health read. Tenant scope comes
/// only from the server-established <see cref="ICurrentTenantContextAccessor"/>,
/// and a project owned by a different tenant is reported as not found rather
/// than forbidden.
/// </summary>
public sealed class ProjectHealthFacade : IProjectHealthFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IProjectRepository _repository;

    public ProjectHealthFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IProjectRepository repository)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _repository = repository;
    }

    public async Task<ProjectHealthFacadeResult> GetProjectHealthAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return ProjectHealthFacadeResult.Failure(ProjectHealthFacadeError.NoTenantContext);
        }

        var project = await _repository.FindByIdAsync(tenantContext.TenantId, projectId, cancellationToken);
        if (project is null)
        {
            return ProjectHealthFacadeResult.Failure(ProjectHealthFacadeError.NotFound);
        }

        return ProjectHealthFacadeResult.Success(ProjectHealthMapper.ToHealthResponse(project));
    }
}
