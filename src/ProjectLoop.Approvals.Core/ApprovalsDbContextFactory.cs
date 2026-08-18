using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalsDbContextFactory : IDesignTimeDbContextFactory<ApprovalsDbContext>
{
    public ApprovalsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApprovalsDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopApprovals;Trusted_Connection=True;");

        return new ApprovalsDbContext(optionsBuilder.Options);
    }
}
