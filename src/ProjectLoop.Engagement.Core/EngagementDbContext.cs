using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Engagement.Core;

public sealed class EngagementDbContext : DbContext
{
    public EngagementDbContext(DbContextOptions<EngagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Milestone> Milestones => Set<Milestone>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("engagement");

        builder.ApplyConfiguration(new ProjectConfiguration());
        builder.ApplyConfiguration(new MilestoneConfiguration());
        builder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}
