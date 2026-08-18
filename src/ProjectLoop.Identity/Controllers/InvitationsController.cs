using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ProjectLoop.Identity.Contracts;
using ProjectLoop.Identity.Core;

namespace ProjectLoop.Identity.Controllers;

[ApiController]
[Route("invitations")]
public sealed class InvitationsController : ControllerBase
{
    private readonly IInvitationCreationFacade _invitationCreationFacade;
    private readonly IInvitationAcceptanceFacade _invitationAcceptanceFacade;

    public InvitationsController(
        IInvitationCreationFacade invitationCreationFacade,
        IInvitationAcceptanceFacade invitationAcceptanceFacade)
    {
        _invitationCreationFacade = invitationCreationFacade;
        _invitationAcceptanceFacade = invitationAcceptanceFacade;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvitationRequest request, CancellationToken cancellationToken)
    {
        var result = await _invitationCreationFacade.CreateInvitationAsync(request.Email, cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        return result.Error switch
        {
            InvitationCreationFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            InvitationCreationFacadeError.NotAuthorized => StatusCode(StatusCodes.Status403Forbidden),
            InvitationCreationFacadeError.InvalidEmail => Problem(
                detail: "The provided email address is not valid.",
                statusCode: StatusCodes.Status400BadRequest),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost("accept")]
    [SkipTenantContext]
    public async Task<IActionResult> Accept([FromBody] AcceptInvitationRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var result = await _invitationAcceptanceFacade.AcceptInvitationAsync(request.Token, userId, userEmail, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok();
        }

        return result.Error switch
        {
            InvitationAcceptanceError.EmailMismatch => StatusCode(StatusCodes.Status403Forbidden),
            InvitationAcceptanceError.InvalidToken => Problem(
                detail: "The invitation token is not valid.",
                statusCode: StatusCodes.Status400BadRequest),
            InvitationAcceptanceError.Expired => Problem(
                detail: "The invitation has expired.",
                statusCode: StatusCodes.Status400BadRequest),
            InvitationAcceptanceError.AlreadyAccepted => Problem(
                detail: "The invitation has already been accepted.",
                statusCode: StatusCodes.Status400BadRequest),
            InvitationAcceptanceError.Revoked => Problem(
                detail: "The invitation has been revoked.",
                statusCode: StatusCodes.Status400BadRequest),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
