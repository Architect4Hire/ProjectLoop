using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class TenantConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void Tenant_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(Tenant))!;

        Assert.Equal("Tenants", entity.GetTableName());
    }

    [Fact]
    public void Tenant_Name_Has_Max_Length_And_Is_Required()
    {
        var entity = BuildModel().FindEntityType(typeof(Tenant))!;
        var name = entity.FindProperty(nameof(Tenant.Name))!;

        Assert.Equal(200, name.GetMaxLength());
        Assert.False(name.IsNullable);
    }

    [Fact]
    public void Tenant_UpdatedAtUtc_Is_Concurrency_Token()
    {
        var entity = BuildModel().FindEntityType(typeof(Tenant))!;
        var updatedAt = entity.FindProperty(nameof(Tenant.UpdatedAtUtc))!;

        Assert.True(updatedAt.IsConcurrencyToken);
    }
}
