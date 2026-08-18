using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class InvitationAcceptanceFacadeTests
{
    private static IdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task AcceptInvitationAsync_Fails_Without_Calling_Business_Operation_When_Token_Is_Empty()
    {
        await using var dbContext = CreateDbContext();
        var facade = new InvitationAcceptanceFacade(new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator()));

        var result = await facade.AcceptInvitationAsync(string.Empty, "user-1", "client@example.com");

        Assert.False(result.IsSuccess);
        Assert.Equal(InvitationAcceptanceError.InvalidToken, result.Error);
    }

    [Fact]
    public async Task AcceptInvitationAsync_Delegates_To_Business_Operation_For_A_Valid_Token()
    {
        await using var dbContext = CreateDbContext();
        var tokenGenerator = new InvitationTokenGenerator();
        var token = tokenGenerator.Generate();
        var now = DateTimeOffset.UtcNow;

        dbContext.ClientInvitations.Add(new ClientInvitation
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Email = "client@example.com",
            TokenHash = token.TokenHash,
            ExpiresAtUtc = now.AddDays(7),
            Status = ClientInvitationStatus.Pending,
            InvitedByUserId = "inviter-1",
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        });
        await dbContext.SaveChangesAsync();

        var facade = new InvitationAcceptanceFacade(new InvitationAcceptanceService(dbContext, tokenGenerator));

        var result = await facade.AcceptInvitationAsync(token.RawToken, "user-1", "client@example.com");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Membership);
        Assert.Equal("user-1", result.Membership!.UserId);
    }
}
