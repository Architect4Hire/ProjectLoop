using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Notifications.Core;

public sealed class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<NotificationDelivery> NotificationDeliveries => Set<NotificationDelivery>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("notifications");

        builder.ApplyConfiguration(new NotificationDeliveryConfiguration());
        builder.ApplyConfiguration(new InboxMessageConfiguration());
    }
}
