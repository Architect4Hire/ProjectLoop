using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Identity.Core;

/// <summary>
/// Validates invitation state/expiry and, when valid, marks the invitation
/// accepted and creates the resulting membership in a single transaction.
/// </summary>
public sealed class InvitationAcceptanceService : IInvitationAcceptanceService
{
    private readonly IdentityDbContext _dbContext;
    private readonly IInvitationTokenGenerator _tokenGenerator;
    private readonly TimeProvider _timeProvider;

    public InvitationAcceptanceService(
        IdentityDbContext dbContext,
        IInvitationTokenGenerator tokenGenerator,
        TimeProvider? timeProvider = null)
    {
        _dbContext = dbContext;
        _tokenGenerator = tokenGenerator;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<InvitationAcceptanceResult> AcceptAsync(
        string rawToken,
        string userId,
        string userEmail,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenGenerator.Hash(rawToken);
        var invitation = await _dbContext.ClientInvitations
            .SingleOrDefaultAsync(i => i.TokenHash == tokenHash, cancellationToken);

        if (invitation is null)
        {
            return InvitationAcceptanceResult.Failure(InvitationAcceptanceError.InvalidToken);
        }

        if (invitation.Status == ClientInvitationStatus.Accepted)
        {
            return InvitationAcceptanceResult.Failure(InvitationAcceptanceError.AlreadyAccepted);
        }

        if (invitation.Status == ClientInvitationStatus.Revoked)
        {
            return InvitationAcceptanceResult.Failure(InvitationAcceptanceError.Revoked);
        }

        var now = _timeProvider.GetUtcNow();

        if (invitation.ExpiresAtUtc <= now)
        {
            return InvitationAcceptanceResult.Failure(InvitationAcceptanceError.Expired);
        }

        if (!string.Equals(invitation.Email, userEmail, StringComparison.OrdinalIgnoreCase))
        {
            return InvitationAcceptanceResult.Failure(InvitationAcceptanceError.EmailMismatch);
        }

        invitation.Status = ClientInvitationStatus.Accepted;
        invitation.UpdatedAtUtc = now;

        var membership = new TenantMembership
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TenantId = invitation.TenantId,
            Role = TenantMembershipRole.Member,
            Status = TenantMembershipStatus.Active,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
        _dbContext.TenantMemberships.Add(membership);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return InvitationAcceptanceResult.Success(membership);
    }
}
