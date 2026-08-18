namespace ProjectLoop.Identity.Core;

public interface IClientInvitationRepository
{
    Task AddAsync(ClientInvitation invitation, CancellationToken cancellationToken = default);

    Task<ClientInvitation?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
}
