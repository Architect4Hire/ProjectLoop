using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class ProjectConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void Project_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(Project))!;

        Assert.Equal("Projects", entity.GetTableName());
    }

    [Fact]
    public void Project_Has_Index_On_TenantId()
    {
        var entity = BuildModel().FindEntityType(typeof(Project))!;

        var index = entity.GetIndexes().Single();

        Assert.Equal(new[] { nameof(Project.TenantId) }, index.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Project_UpdatedAtUtc_Is_Concurrency_Token()
    {
        var entity = BuildModel().FindEntityType(typeof(Project))!;
        var updatedAt = entity.FindProperty(nameof(Project.UpdatedAtUtc))!;

        Assert.True(updatedAt.IsConcurrencyToken);
    }
}
