using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class TenantMembershipConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new TenantMembershipConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void TenantMembership_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(TenantMembership))!;

        Assert.Equal("TenantMemberships", entity.GetTableName());
    }

    [Fact]
    public void TenantMembership_Has_Unique_Filtered_Index_On_Active_Membership()
    {
        var entity = BuildModel().FindEntityType(typeof(TenantMembership))!;

        var index = entity.GetIndexes().Single(i => i.IsUnique);

        Assert.Equal(
            new[] { nameof(TenantMembership.UserId), nameof(TenantMembership.TenantId), nameof(TenantMembership.Status) },
            index.Properties.Select(p => p.Name));
        Assert.Equal("[Status] = 'Active'", index.GetFilter());
    }

    [Fact]
    public void TenantMembership_UpdatedAtUtc_Is_Concurrency_Token()
    {
        var entity = BuildModel().FindEntityType(typeof(TenantMembership))!;
        var updatedAt = entity.FindProperty(nameof(TenantMembership.UpdatedAtUtc))!;

        Assert.True(updatedAt.IsConcurrencyToken);
    }
}
