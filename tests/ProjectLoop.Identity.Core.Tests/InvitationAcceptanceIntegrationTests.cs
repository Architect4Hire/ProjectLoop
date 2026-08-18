using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class InvitationAcceptanceIntegrationTests
{
    private static async Task<IdentityDbContext> CreateMigratedDbContextAsync()
    {
        var connectionString =
            $"Server=(localdb)\\MSSQLLocalDB;Database=ProjectLoopIdentity_AcceptanceTest_{Guid.NewGuid():N};Trusted_Connection=True;TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        var dbContext = new IdentityDbContext(options);
        await dbContext.Database.MigrateAsync();
        return dbContext;
    }

    private static async Task<(ClientInvitation Invitation, string RawToken)> SeedInvitationAsync(
        IdentityDbContext dbContext,
        DateTimeOffset? expiresAtUtc = null,
        ClientInvitationStatus status = ClientInvitationStatus.Pending)
    {
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

        return (invitation, token.RawToken);
    }

    [Fact]
    public async Task Valid_Invitation_Is_Accepted_And_Creates_Active_Membership()
    {
        await using var dbContext = await CreateMigratedDbContextAsync();
        try
        {
            var (invitation, rawToken) = await SeedInvitationAsync(dbContext);
            var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

            var result = await service.AcceptAsync(rawToken, "user-1", "client@example.com");

            Assert.True(result.IsSuccess);
            var persistedInvitation = await dbContext.ClientInvitations.SingleAsync(i => i.Id == invitation.Id);
            Assert.Equal(ClientInvitationStatus.Accepted, persistedInvitation.Status);
            var persistedMembership = await dbContext.TenantMemberships.SingleAsync(m => m.TenantId == invitation.TenantId);
            Assert.Equal("user-1", persistedMembership.UserId);
            Assert.Equal(TenantMembershipStatus.Active, persistedMembership.Status);
        }
        finally
        {
            await dbContext.Database.EnsureDeletedAsync();
        }
    }

    [Fact]
    public async Task Expired_Invitation_Is_Rejected()
    {
        await using var dbContext = await CreateMigratedDbContextAsync();
        try
        {
            var (_, rawToken) = await SeedInvitationAsync(dbContext, expiresAtUtc: DateTimeOffset.UtcNow.AddDays(-1));
            var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

            var result = await service.AcceptAsync(rawToken, "user-1", "client@example.com");

            Assert.False(result.IsSuccess);
            Assert.Equal(InvitationAcceptanceError.Expired, result.Error);
        }
        finally
        {
            await dbContext.Database.EnsureDeletedAsync();
        }
    }

    [Fact]
    public async Task Reused_Invitation_Token_Is_Rejected_On_Second_Acceptance()
    {
        await using var dbContext = await CreateMigratedDbContextAsync();
        try
        {
            var (_, rawToken) = await SeedInvitationAsync(dbContext);
            var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

            var firstResult = await service.AcceptAsync(rawToken, "user-1", "client@example.com");
            Assert.True(firstResult.IsSuccess);

            var secondResult = await service.AcceptAsync(rawToken, "user-2", "client@example.com");

            Assert.False(secondResult.IsSuccess);
            Assert.Equal(InvitationAcceptanceError.AlreadyAccepted, secondResult.Error);
        }
        finally
        {
            await dbContext.Database.EnsureDeletedAsync();
        }
    }

    [Fact]
    public async Task Wrong_Email_Is_Rejected_And_Does_Not_Create_Membership()
    {
        await using var dbContext = await CreateMigratedDbContextAsync();
        try
        {
            var (invitation, rawToken) = await SeedInvitationAsync(dbContext);
            var service = new InvitationAcceptanceService(dbContext, new InvitationTokenGenerator());

            var result = await service.AcceptAsync(rawToken, "user-1", "someone-else@example.com");

            Assert.False(result.IsSuccess);
            Assert.Equal(InvitationAcceptanceError.EmailMismatch, result.Error);

            var persistedInvitation = await dbContext.ClientInvitations.SingleAsync(i => i.Id == invitation.Id);
            Assert.Equal(ClientInvitationStatus.Pending, persistedInvitation.Status);
            Assert.False(await dbContext.TenantMemberships.AnyAsync(m => m.TenantId == invitation.TenantId));
        }
        finally
        {
            await dbContext.Database.EnsureDeletedAsync();
        }
    }
}
