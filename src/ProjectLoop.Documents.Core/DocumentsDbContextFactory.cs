using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectLoop.Documents.Core;

public sealed class DocumentsDbContextFactory : IDesignTimeDbContextFactory<DocumentsDbContext>
{
    public DocumentsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopDocuments;Trusted_Connection=True;");

        return new DocumentsDbContext(optionsBuilder.Options);
    }
}
