using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class MilestoneConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new MilestoneConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void Milestone_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(Milestone))!;

        Assert.Equal("Milestones", entity.GetTableName());
    }

    [Fact]
    public void Milestone_Has_Index_On_TenantId_And_ProjectId()
    {
        var entity = BuildModel().FindEntityType(typeof(Milestone))!;

        var index = entity.GetIndexes().Single(i => !i.IsUnique);

        Assert.Equal(
            new[] { nameof(Milestone.TenantId), nameof(Milestone.ProjectId) },
            index.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Milestone_Has_Required_ForeignKey_To_Project()
    {
        var entity = BuildModel().FindEntityType(typeof(Milestone))!;

        var foreignKey = entity.GetForeignKeys().Single();

        Assert.Equal(nameof(Project), foreignKey.PrincipalEntityType.ClrType.Name);
        Assert.Equal(new[] { nameof(Milestone.ProjectId) }, foreignKey.Properties.Select(p => p.Name));
        Assert.True(foreignKey.IsRequired);
    }

    [Fact]
    public void Milestone_UpdatedAtUtc_Is_Concurrency_Token()
    {
        var entity = BuildModel().FindEntityType(typeof(Milestone))!;
        var updatedAt = entity.FindProperty(nameof(Milestone.UpdatedAtUtc))!;

        Assert.True(updatedAt.IsConcurrencyToken);
    }
}
