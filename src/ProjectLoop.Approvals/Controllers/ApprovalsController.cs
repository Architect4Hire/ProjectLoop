using Microsoft.AspNetCore.Mvc;
using ProjectLoop.Approvals.Contracts;
using ProjectLoop.Approvals.Core;

namespace ProjectLoop.Approvals.Controllers;

[ApiController]
[Route("approvals")]
public sealed class ApprovalsController : ControllerBase
{
    private readonly IApprovalRequestReadFacade _approvalRequestReadFacade;
    private readonly IApprovalApproveFacade _approvalApproveFacade;
    private readonly IApprovalRejectFacade _approvalRejectFacade;

    public ApprovalsController(
        IApprovalRequestReadFacade approvalRequestReadFacade,
        IApprovalApproveFacade approvalApproveFacade,
        IApprovalRejectFacade approvalRejectFacade)
    {
        _approvalRequestReadFacade = approvalRequestReadFacade;
        _approvalApproveFacade = approvalApproveFacade;
        _approvalRejectFacade = approvalRejectFacade;
    }

    [HttpGet("{approvalRequestId:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid approvalRequestId, CancellationToken cancellationToken)
    {
        var result = await _approvalRequestReadFacade.GetAsync(approvalRequestId, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Response);
        }

        return result.Error switch
        {
            ApprovalRequestReadFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            ApprovalRequestReadFacadeError.NotFound => StatusCode(StatusCodes.Status404NotFound),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost("{approvalRequestId:guid}/approve")]
    public async Task<IActionResult> Approve(
        [FromRoute] Guid approvalRequestId,
        [FromBody] ApprovalDecisionBody body,
        CancellationToken cancellationToken)
    {
        var result = await _approvalApproveFacade.ApproveAsync(
            new ApprovalDecisionRequest { ApprovalRequestId = approvalRequestId, Comments = body.Comments },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Result);
        }

        return result.Error switch
        {
            ApprovalDecisionFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            ApprovalDecisionFacadeError.RequestNotFound => StatusCode(StatusCodes.Status404NotFound),
            ApprovalDecisionFacadeError.Conflict => Problem(
                detail: "The approval request is not eligible for this decision.",
                statusCode: StatusCodes.Status409Conflict),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost("{approvalRequestId:guid}/reject")]
    public async Task<IActionResult> Reject(
        [FromRoute] Guid approvalRequestId,
        [FromBody] ApprovalDecisionBody body,
        CancellationToken cancellationToken)
    {
        var result = await _approvalRejectFacade.RejectAsync(
            new ApprovalDecisionRequest { ApprovalRequestId = approvalRequestId, Comments = body.Comments },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Result);
        }

        return result.Error switch
        {
            ApprovalDecisionFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            ApprovalDecisionFacadeError.RequestNotFound => StatusCode(StatusCodes.Status404NotFound),
            ApprovalDecisionFacadeError.Conflict => Problem(
                detail: "The approval request is not eligible for this decision.",
                statusCode: StatusCodes.Status409Conflict),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
