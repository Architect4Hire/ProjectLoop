namespace ProjectLoop.Identity.Core;

/// <summary>
/// Orchestrates the existing invitation acceptance business operation.
/// Caller identity comes only from the authenticated principal (relayed by
/// the controller), never from request payload values.
/// </summary>
public sealed class InvitationAcceptanceFacade : IInvitationAcceptanceFacade
{
    private readonly IInvitationAcceptanceService _acceptanceService;

    public InvitationAcceptanceFacade(IInvitationAcceptanceService acceptanceService)
    {
        _acceptanceService = acceptanceService;
    }

    public Task<InvitationAcceptanceResult> AcceptInvitationAsync(
        string rawToken,
        string userId,
        string userEmail,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return Task.FromResult(InvitationAcceptanceResult.Failure(InvitationAcceptanceError.InvalidToken));
        }

        return _acceptanceService.AcceptAsync(rawToken, userId, userEmail, cancellationToken);
    }
}
