using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectLoop.Engagement.Core;

public sealed class EngagementDbContextFactory : IDesignTimeDbContextFactory<EngagementDbContext>
{
    public EngagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EngagementDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopEngagement;Trusted_Connection=True;");

        return new EngagementDbContext(optionsBuilder.Options);
    }
}
