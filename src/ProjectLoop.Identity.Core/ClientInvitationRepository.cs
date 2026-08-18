using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Identity.Core;

public sealed class ClientInvitationRepository : IClientInvitationRepository
{
    private readonly IdentityDbContext _dbContext;

    public ClientInvitationRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ClientInvitation invitation, CancellationToken cancellationToken = default)
    {
        _dbContext.ClientInvitations.Add(invitation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<ClientInvitation?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _dbContext.ClientInvitations.SingleOrDefaultAsync(i => i.TokenHash == tokenHash, cancellationToken);
}
