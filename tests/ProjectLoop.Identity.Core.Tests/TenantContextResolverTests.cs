using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class TenantContextResolverTests
{
    private static IdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task ResolveAsync_Allows_Active_Member_Of_Active_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        dbContext.Tenants.Add(new Tenant { Id = tenantId, Name = "Acme", Status = TenantStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now });
        dbContext.TenantMemberships.Add(new TenantMembership
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            TenantId = tenantId,
            Role = TenantMembershipRole.Admin,
            Status = TenantMembershipStatus.Active,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        });
        await dbContext.SaveChangesAsync();

        var resolver = new TenantContextResolver(dbContext);
        var result = await resolver.ResolveAsync("user-1", tenantId);

        Assert.True(result.IsAllowed);
        Assert.NotNull(result.Context);
        Assert.Equal(tenantId, result.Context!.TenantId);
        Assert.Equal("user-1", result.Context.UserId);
        Assert.Equal(TenantMembershipRole.Admin, result.Context.Role);
    }

    [Fact]
    public async Task ResolveAsync_Denies_When_No_Membership_Exists()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        dbContext.Tenants.Add(new Tenant { Id = tenantId, Name = "Acme", Status = TenantStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now });
        await dbContext.SaveChangesAsync();

        var resolver = new TenantContextResolver(dbContext);
        var result = await resolver.ResolveAsync("user-1", tenantId);

        Assert.False(result.IsAllowed);
        Assert.Null(result.Context);
        Assert.Equal(TenantContextDenialReason.MembershipNotFound, result.DenialReason);
    }

    [Fact]
    public async Task ResolveAsync_Denies_When_Membership_Is_Revoked()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        dbContext.Tenants.Add(new Tenant { Id = tenantId, Name = "Acme", Status = TenantStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now });
        dbContext.TenantMemberships.Add(new TenantMembership
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            TenantId = tenantId,
            Role = TenantMembershipRole.Member,
            Status = TenantMembershipStatus.Revoked,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        });
        await dbContext.SaveChangesAsync();

        var resolver = new TenantContextResolver(dbContext);
        var result = await resolver.ResolveAsync("user-1", tenantId);

        Assert.False(result.IsAllowed);
        Assert.Equal(TenantContextDenialReason.MembershipNotActive, result.DenialReason);
    }

    [Fact]
    public async Task ResolveAsync_Denies_When_Tenant_Is_Suspended()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        dbContext.Tenants.Add(new Tenant { Id = tenantId, Name = "Acme", Status = TenantStatus.Suspended, CreatedAtUtc = now, UpdatedAtUtc = now });
        dbContext.TenantMemberships.Add(new TenantMembership
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            TenantId = tenantId,
            Role = TenantMembershipRole.Member,
            Status = TenantMembershipStatus.Active,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        });
        await dbContext.SaveChangesAsync();

        var resolver = new TenantContextResolver(dbContext);
        var result = await resolver.ResolveAsync("user-1", tenantId);

        Assert.False(result.IsAllowed);
        Assert.Equal(TenantContextDenialReason.TenantNotActive, result.DenialReason);
    }

    [Fact]
    public async Task ResolveAsync_Denies_When_Requested_Tenant_Does_Not_Exist()
    {
        await using var dbContext = CreateDbContext();

        var resolver = new TenantContextResolver(dbContext);
        var result = await resolver.ResolveAsync("user-1", Guid.NewGuid());

        Assert.False(result.IsAllowed);
        Assert.Equal(TenantContextDenialReason.MembershipNotFound, result.DenialReason);
    }
}
