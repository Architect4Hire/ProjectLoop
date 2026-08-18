using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class InvitationAcceptanceServiceTests
{
    private static IdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    private static async Task<(IdentityDbContext DbContext, ClientInvitation Invitation, string RawToken)> SeedInvitationAsync(
        DateTimeOffset? expiresAtUtc = null,
        ClientInvitationStatus status = ClientInvitationStatus.Pending)
    {
        var dbContext = CreateDbContext();
        var generator = new InvitationTokenGenerator();
        var token = generator.Generate();
        var now = DateTimeOffset.UtcNow;

        var invitation = new ClientInvitation
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Email = "client@example.com",
            TokenHash = token.TokenHash,
            ExpiresAtUtc = expiresAtUtc ?? now.AddDays(7),
            Status = status,
            InvitedByUserId = "inviter-1",
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        dbContext.ClientInvitations.Add(invitation);
        await dbContext.SaveChangesAsync();

        return (dbContext, invitation, token.RawToken);
    }

    [Fact]
    public async Task AcceptAsync_Succeeds_And_Creates_Active_Membership()
    {
        var (dbContext, invitation, rawToken) = await SeedInvitationAsync();
        await using var _ = dbContext;
        var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

        var result = await service.AcceptAsync(rawToken, "user-1", "client@example.com");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Membership);
        Assert.Equal(invitation.TenantId, result.Membership!.TenantId);
        Assert.Equal(TenantMembershipStatus.Active, result.Membership.Status);

        var persistedInvitation = await dbContext.ClientInvitations.SingleAsync(i => i.Id == invitation.Id);
        Assert.Equal(ClientInvitationStatus.Accepted, persistedInvitation.Status);
    }

    [Fact]
    public async Task AcceptAsync_Fails_For_Unknown_Token()
    {
        await using var dbContext = CreateDbContext();
        var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

        var result = await service.AcceptAsync("not-a-real-token", "user-1", "client@example.com");

        Assert.False(result.IsSuccess);
        Assert.Equal(InvitationAcceptanceError.InvalidToken, result.Error);
    }

    [Fact]
    public async Task AcceptAsync_Fails_When_Expired()
    {
        var (dbContext, _, rawToken) = await SeedInvitationAsync(expiresAtUtc: DateTimeOffset.UtcNow.AddDays(-1));
        await using var _ = dbContext;
        var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

        var result = await service.AcceptAsync(rawToken, "user-1", "client@example.com");

        Assert.False(result.IsSuccess);
        Assert.Equal(InvitationAcceptanceError.Expired, result.Error);
    }

    [Fact]
    public async Task AcceptAsync_Fails_When_Already_Accepted()
    {
        var (dbContext, _, rawToken) = await SeedInvitationAsync(status: ClientInvitationStatus.Accepted);
        await using var _ = dbContext;
        var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

        var result = await service.AcceptAsync(rawToken, "user-1", "client@example.com");

        Assert.False(result.IsSuccess);
        Assert.Equal(InvitationAcceptanceError.AlreadyAccepted, result.Error);
    }

    [Fact]
    public async Task AcceptAsync_Fails_When_Revoked()
    {
        var (dbContext, _, rawToken) = await SeedInvitationAsync(status: ClientInvitationStatus.Revoked);
        await using var _ = dbContext;
        var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

        var result = await service.AcceptAsync(rawToken, "user-1", "client@example.com");

        Assert.False(result.IsSuccess);
        Assert.Equal(InvitationAcceptanceError.Revoked, result.Error);
    }

    [Fact]
    public async Task AcceptAsync_Fails_When_Email_Does_Not_Match()
    {
        var (dbContext, _, rawToken) = await SeedInvitationAsync();
        await using var _ = dbContext;
        var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

        var result = await service.AcceptAsync(rawToken, "user-1", "someone-else@example.com");

        Assert.False(result.IsSuccess);
        Assert.Equal(InvitationAcceptanceError.EmailMismatch, result.Error);
    }
}
