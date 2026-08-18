namespace ProjectLoop.Identity.Core;

public interface IInvitationAcceptanceFacade
{
    Task<InvitationAcceptanceResult> AcceptInvitationAsync(
        string rawToken,
        string userId,
        string userEmail,
        CancellationToken cancellationToken = default);
}
