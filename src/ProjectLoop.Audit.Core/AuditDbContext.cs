using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Audit.Core;

public sealed class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("audit");

        builder.ApplyConfiguration(new AuditRecordConfiguration());
        builder.ApplyConfiguration(new InboxMessageConfiguration());
    }
}
