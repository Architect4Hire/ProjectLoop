using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectLoop.Audit.Core;

public sealed class AuditDbContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
{
    public AuditDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopAudit;Trusted_Connection=True;");

        return new AuditDbContext(optionsBuilder.Options);
    }
}
