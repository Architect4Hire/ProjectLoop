namespace ProjectLoop.Identity.Core;

public interface IInvitationCreationFacade
{
    Task<InvitationCreationFacadeResult> CreateInvitationAsync(string email, CancellationToken cancellationToken = default);
}
