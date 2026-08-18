using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectLoop.Notifications.Core;

public sealed class NotificationsDbContextFactory : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopNotifications;Trusted_Connection=True;");

        return new NotificationsDbContext(optionsBuilder.Options);
    }
}
