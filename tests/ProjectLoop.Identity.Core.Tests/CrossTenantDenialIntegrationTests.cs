using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class CrossTenantDenialIntegrationTests
{
    [Fact]
    public async Task User_Authorized_For_Tenant_A_Cannot_Establish_Tenant_B_Context()
    {
        var connectionString =
            $"Server=(localdb)\\MSSQLLocalDB;Database=ProjectLoopIdentity_CrossTenantTest_{Guid.NewGuid():N};Trusted_Connection=True;TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var dbContext = new IdentityDbContext(options);

        try
        {
            await dbContext.Database.MigrateAsync();

            var now = DateTimeOffset.UtcNow;
            var tenantA = new Tenant { Id = Guid.NewGuid(), Name = "Tenant A", Status = TenantStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now };
            var tenantB = new Tenant { Id = Guid.NewGuid(), Name = "Tenant B", Status = TenantStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now };

            dbContext.Tenants.AddRange(tenantA, tenantB);
            dbContext.TenantMemberships.Add(new TenantMembership
            {
                Id = Guid.NewGuid(),
                UserId = "user-1",
                TenantId = tenantA.Id,
                Role = TenantMembershipRole.Member,
                Status = TenantMembershipStatus.Active,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
            });
            await dbContext.SaveChangesAsync();

            var resolver = new TenantContextResolver(dbContext);

            var allowedResult = await resolver.ResolveAsync("user-1", tenantA.Id);
            Assert.True(allowedResult.IsAllowed);

            var deniedResult = await resolver.ResolveAsync("user-1", tenantB.Id);

            Assert.False(deniedResult.IsAllowed);
            Assert.Null(deniedResult.Context);
            Assert.Equal(TenantContextDenialReason.MembershipNotFound, deniedResult.DenialReason);
        }
        finally
        {
            await dbContext.Database.EnsureDeletedAsync();
        }
    }
}
