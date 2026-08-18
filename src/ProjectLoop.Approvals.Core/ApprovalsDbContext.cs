using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalsDbContext : DbContext
{
    public ApprovalsDbContext(DbContextOptions<ApprovalsDbContext> options)
        : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("approvals");

        builder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}
