using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class InvitationCreationFacadeTests
{
    private static IdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    private static InvitationCreationFacade CreateFacade(IdentityDbContext dbContext, ICurrentTenantContextAccessor accessor)
    {
        var repository = new ClientInvitationRepository(dbContext);
        var creationService = new InvitationCreationService(new InvitationTokenGenerator(), repository);
        return new InvitationCreationFacade(accessor, creationService);
    }

    [Fact]
    public async Task CreateInvitationAsync_Fails_When_No_Tenant_Context()
    {
        await using var dbContext = CreateDbContext();
        var accessor = new CurrentTenantContextAccessor();
        var facade = CreateFacade(dbContext, accessor);

        var result = await facade.CreateInvitationAsync("client@example.com");

        Assert.False(result.IsSuccess);
        Assert.Equal(InvitationCreationFacadeError.NoTenantContext, result.Error);
    }

    [Fact]
    public async Task CreateInvitationAsync_Fails_When_Caller_Is_Not_Admin()
    {
        await using var dbContext = CreateDbContext();
        var accessor = new CurrentTenantContextAccessor
        {
            Current = new TenantContext { TenantId = Guid.NewGuid(), UserId = "user-1", Role = TenantMembershipRole.Member },
        };
        var facade = CreateFacade(dbContext, accessor);

        var result = await facade.CreateInvitationAsync("client@example.com");

        Assert.False(result.IsSuccess);
        Assert.Equal(InvitationCreationFacadeError.NotAuthorized, result.Error);
    }

    [Fact]
    public async Task CreateInvitationAsync_Fails_When_Email_Is_Invalid()
    {
        await using var dbContext = CreateDbContext();
        var accessor = new CurrentTenantContextAccessor
        {
            Current = new TenantContext { TenantId = Guid.NewGuid(), UserId = "user-1", Role = TenantMembershipRole.Admin },
        };
        var facade = CreateFacade(dbContext, accessor);

        var result = await facade.CreateInvitationAsync("not-an-email");

        Assert.False(result.IsSuccess);
        Assert.Equal(InvitationCreationFacadeError.InvalidEmail, result.Error);
    }

    [Fact]
    public async Task CreateInvitationAsync_Succeeds_For_Authorized_Admin_With_Valid_Email()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var accessor = new CurrentTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "admin-1", Role = TenantMembershipRole.Admin },
        };
        var facade = CreateFacade(dbContext, accessor);

        var result = await facade.CreateInvitationAsync("client@example.com");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(tenantId, result.Result!.Invitation.TenantId);
        Assert.Equal("admin-1", result.Result.Invitation.InvitedByUserId);
    }
}
