namespace ProjectLoop.Identity.Core;

public interface IInvitationCreationService
{
    Task<InvitationCreationResult> CreateAsync(
        Guid tenantId,
        string email,
        string invitedByUserId,
        CancellationToken cancellationToken = default);
}
