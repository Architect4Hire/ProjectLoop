using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void Document_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(Document))!;

        Assert.Equal("Documents", entity.GetTableName());
    }

    [Fact]
    public void Document_Has_Index_On_TenantId_And_ProjectId()
    {
        var entity = BuildModel().FindEntityType(typeof(Document))!;

        var index = entity.GetIndexes().Single();

        Assert.Equal(
            new[] { nameof(Document.TenantId), nameof(Document.ProjectId) },
            index.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Document_UpdatedAtUtc_Is_Concurrency_Token()
    {
        var entity = BuildModel().FindEntityType(typeof(Document))!;
        var updatedAt = entity.FindProperty(nameof(Document.UpdatedAtUtc))!;

        Assert.True(updatedAt.IsConcurrencyToken);
    }
}
