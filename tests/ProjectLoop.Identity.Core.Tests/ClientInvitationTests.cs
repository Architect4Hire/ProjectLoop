using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class ClientInvitationTests
{
    [Fact]
    public void ClientInvitation_Can_Be_Created_With_Required_Members()
    {
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

        Assert.Equal("client@example.com", invitation.Email);
        Assert.Equal(ClientInvitationStatus.Pending, invitation.Status);
    }

    [Fact]
    public void ClientInvitation_Status_Can_Transition_To_Accepted()
    {
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

        invitation.Status = ClientInvitationStatus.Accepted;

        Assert.Equal(ClientInvitationStatus.Accepted, invitation.Status);
    }
}
