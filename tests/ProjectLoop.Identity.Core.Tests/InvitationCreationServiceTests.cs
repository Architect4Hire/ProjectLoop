using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class InvitationCreationServiceTests
{
    private static IdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_Persists_Pending_Invitation_And_Returns_Raw_Token()
    {
        await using var dbContext = CreateDbContext();
        var repository = new ClientInvitationRepository(dbContext);
        var service = new InvitationCreationService(new InvitationTokenGenerator(), repository);
        var tenantId = Guid.NewGuid();

        var result = await service.CreateAsync(tenantId, "client@example.com", "inviter-1");

        Assert.NotEmpty(result.RawToken);
        Assert.Equal(ClientInvitationStatus.Pending, result.Invitation.Status);
        Assert.Equal(tenantId, result.Invitation.TenantId);
        Assert.Equal("client@example.com", result.Invitation.Email);
        Assert.True(result.Invitation.ExpiresAtUtc > DateTimeOffset.UtcNow);

        var persisted = await dbContext.ClientInvitations.SingleAsync(i => i.Id == result.Invitation.Id);
        Assert.Equal(result.Invitation.TokenHash, persisted.TokenHash);
        Assert.DoesNotContain(result.RawToken, persisted.TokenHash, StringComparison.Ordinal);
    }
}
