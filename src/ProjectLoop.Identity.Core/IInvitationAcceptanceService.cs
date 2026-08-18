namespace ProjectLoop.Identity.Core;

public interface IInvitationAcceptanceService
{
    Task<InvitationAcceptanceResult> AcceptAsync(
        string rawToken,
        string userId,
        string userEmail,
        CancellationToken cancellationToken = default);
}
