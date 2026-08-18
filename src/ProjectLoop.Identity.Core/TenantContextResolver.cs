using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Identity.Core;

public sealed class TenantContextResolver : ITenantContextResolver
{
    private readonly IdentityDbContext _dbContext;

    public TenantContextResolver(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TenantContextResolution> ResolveAsync(
        string userId,
        Guid requestedTenantId,
        CancellationToken cancellationToken = default)
    {
        var membership = await _dbContext.TenantMemberships
            .SingleOrDefaultAsync(m => m.UserId == userId && m.TenantId == requestedTenantId, cancellationToken);

        if (membership is null)
        {
            return TenantContextResolution.Denied(TenantContextDenialReason.MembershipNotFound);
        }

        if (membership.Status != TenantMembershipStatus.Active)
        {
            return TenantContextResolution.Denied(TenantContextDenialReason.MembershipNotActive);
        }

        var tenant = await _dbContext.Tenants
            .SingleOrDefaultAsync(t => t.Id == requestedTenantId, cancellationToken);

        if (tenant is null)
        {
            return TenantContextResolution.Denied(TenantContextDenialReason.TenantNotFound);
        }

        if (tenant.Status != TenantStatus.Active)
        {
            return TenantContextResolution.Denied(TenantContextDenialReason.TenantNotActive);
        }

        return TenantContextResolution.Allowed(new TenantContext
        {
            TenantId = requestedTenantId,
            UserId = userId,
            Role = membership.Role,
        });
    }
}
