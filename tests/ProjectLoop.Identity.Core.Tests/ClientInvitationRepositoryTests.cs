using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class ClientInvitationRepositoryTests
{
    private static IdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task AddAsync_Persists_The_Invitation()
    {
        await using var dbContext = CreateDbContext();
        var repository = new ClientInvitationRepository(dbContext);
        var now = DateTimeOffset.UtcNow;

        var invitation = new ClientInvitation
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Email = "client@example.com",
            TokenHash = "hashed-token",
            ExpiresAtUtc = now.AddDays(7),
            Status = ClientInvitationStatus.Pending,
            InvitedByUserId = "inviter-1",
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        await repository.AddAsync(invitation);

        var persisted = await dbContext.ClientInvitations.SingleAsync(i => i.Id == invitation.Id);
        Assert.Equal("client@example.com", persisted.Email);
        Assert.Equal(ClientInvitationStatus.Pending, persisted.Status);
    }
}
