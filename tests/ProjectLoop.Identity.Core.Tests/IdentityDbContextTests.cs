using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class IdentityDbContextTests
{
    [Fact]
    public void Model_Builds_Without_Error()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer("Server=(local);Database=ProjectLoopIdentity;Trusted_Connection=True;")
            .Options;

        using var context = new IdentityDbContext(options);

        var model = context.Model;

        Assert.NotNull(model);
        Assert.Equal("identity", model.GetDefaultSchema());
    }
}
