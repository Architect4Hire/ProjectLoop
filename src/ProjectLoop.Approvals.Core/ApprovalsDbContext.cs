using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalsDbContext : DbContext
{
    public ApprovalsDbContext(DbContextOptions<ApprovalsDbContext> options)
        : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>();

    public DbSet<ApprovalDecision> ApprovalDecisions => Set<ApprovalDecision>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("approvals");

        builder.ApplyConfiguration(new OutboxMessageConfiguration());
        builder.ApplyConfiguration(new ApprovalRequestConfiguration());
        builder.ApplyConfiguration(new ApprovalDecisionConfiguration());
        builder.ApplyConfiguration(new InboxMessageConfiguration());
    }
}
