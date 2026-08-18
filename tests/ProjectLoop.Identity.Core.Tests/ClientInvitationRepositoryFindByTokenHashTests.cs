using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class ClientInvitationRepositoryFindByTokenHashTests
{
    private static IdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task FindByTokenHashAsync_Returns_Matching_Invitation()
    {
        await using var dbContext = CreateDbContext();
        var repository = new ClientInvitationRepository(dbContext);
        var now = DateTimeOffset.UtcNow;

        var invitation = new ClientInvitation
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Email = "client@example.com",
            TokenHash = "hash-abc",
            ExpiresAtUtc = now.AddDays(7),
            Status = ClientInvitationStatus.Pending,
            InvitedByUserId = "inviter-1",
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
        await repository.AddAsync(invitation);

        var found = await repository.FindByTokenHashAsync("hash-abc");

        Assert.NotNull(found);
        Assert.Equal(invitation.Id, found!.Id);
    }

    [Fact]
    public async Task FindByTokenHashAsync_Returns_Null_When_No_Match()
    {
        await using var dbContext = CreateDbContext();
        var repository = new ClientInvitationRepository(dbContext);

        var found = await repository.FindByTokenHashAsync("no-such-hash");

        Assert.Null(found);
    }
}
