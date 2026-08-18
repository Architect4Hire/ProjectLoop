using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Identity.Core;

public sealed class IdentityDbContext : IdentityDbContext<ProjectLoopUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("identity");

        builder.Entity<ProjectLoopUser>(user =>
        {
            user.Property(u => u.LockoutEnd)
                .HasConversion(
                    toProvider => toProvider.HasValue ? toProvider.Value.ToUniversalTime() : toProvider,
                    fromProvider => fromProvider);
        });
    }
}
