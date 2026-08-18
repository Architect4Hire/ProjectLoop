using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class TenantMembershipTests
{
    [Fact]
    public void TenantMembership_Can_Be_Created_With_Required_Members()
    {
        var now = DateTimeOffset.UtcNow;

        var membership = new TenantMembership
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            TenantId = Guid.NewGuid(),
            Role = TenantMembershipRole.Member,
            Status = TenantMembershipStatus.Active,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        Assert.Equal("user-1", membership.UserId);
        Assert.Equal(TenantMembershipRole.Member, membership.Role);
        Assert.Equal(TenantMembershipStatus.Active, membership.Status);
    }

    [Fact]
    public void TenantMembership_Status_Can_Transition_To_Revoked()
    {
        var now = DateTimeOffset.UtcNow;
        var membership = new TenantMembership
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            TenantId = Guid.NewGuid(),
            Role = TenantMembershipRole.Member,
            Status = TenantMembershipStatus.Active,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        membership.Status = TenantMembershipStatus.Revoked;

        Assert.Equal(TenantMembershipStatus.Revoked, membership.Status);
    }
}
