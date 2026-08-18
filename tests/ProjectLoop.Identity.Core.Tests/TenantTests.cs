using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class TenantTests
{
    [Fact]
    public void Tenant_Can_Be_Created_With_Required_Members()
    {
        var now = DateTimeOffset.UtcNow;

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Acme Corp",
            Status = TenantStatus.Active,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        Assert.Equal("Acme Corp", tenant.Name);
        Assert.Equal(TenantStatus.Active, tenant.Status);
    }

    [Fact]
    public void Tenant_Status_Can_Transition_To_Suspended()
    {
        var now = DateTimeOffset.UtcNow;
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Acme Corp",
            Status = TenantStatus.Active,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        tenant.Status = TenantStatus.Suspended;

        Assert.Equal(TenantStatus.Suspended, tenant.Status);
    }
}
