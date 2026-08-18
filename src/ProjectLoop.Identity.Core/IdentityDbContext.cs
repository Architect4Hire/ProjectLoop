using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Identity.Core;

public sealed class IdentityDbContext : IdentityDbContext<ProjectLoopUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();

    public DbSet<ClientInvitation> ClientInvitations => Set<ClientInvitation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("identity");

        builder.ApplyConfiguration(new TenantConfiguration());
        builder.ApplyConfiguration(new TenantMembershipConfiguration());
        builder.ApplyConfiguration(new ClientInvitationConfiguration());

        builder.Entity<ProjectLoopUser>(user =>
        {
            user.Property(u => u.LockoutEnd)
                .HasConversion(
                    toProvider => toProvider.HasValue ? toProvider.Value.ToUniversalTime() : toProvider,
                    fromProvider => fromProvider);
        });
    }
}
