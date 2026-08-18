using Microsoft.AspNetCore.Mvc;
using ProjectLoop.Engagement.Core;

namespace ProjectLoop.Engagement.Controllers;

[ApiController]
[Route("projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly IProjectDetailFacade _projectDetailFacade;
    private readonly IProjectMilestonesFacade _projectMilestonesFacade;
    private readonly IProjectHealthFacade _projectHealthFacade;

    public ProjectsController(
        IProjectDetailFacade projectDetailFacade,
        IProjectMilestonesFacade projectMilestonesFacade,
        IProjectHealthFacade projectHealthFacade)
    {
        _projectDetailFacade = projectDetailFacade;
        _projectMilestonesFacade = projectMilestonesFacade;
        _projectHealthFacade = projectHealthFacade;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _projectDetailFacade.GetProjectDetailAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Response);
        }

        return result.Error switch
        {
            ProjectDetailFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            ProjectDetailFacadeError.NotFound => NotFound(),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpGet("{projectId:guid}/milestones")]
    public async Task<IActionResult> GetMilestones(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _projectMilestonesFacade.GetProjectMilestonesAsync(projectId, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Milestones);
        }

        return result.Error switch
        {
            ProjectMilestonesFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            ProjectMilestonesFacadeError.NotFound => NotFound(),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpGet("{id:guid}/health")]
    public async Task<IActionResult> GetHealth(Guid id, CancellationToken cancellationToken)
    {
        var result = await _projectHealthFacade.GetProjectHealthAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Response);
        }

        return result.Error switch
        {
            ProjectHealthFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            ProjectHealthFacadeError.NotFound => NotFound(),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
