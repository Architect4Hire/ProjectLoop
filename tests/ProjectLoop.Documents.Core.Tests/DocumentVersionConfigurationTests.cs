using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentVersionConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentVersionConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void DocumentVersion_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(DocumentVersion))!;

        Assert.Equal("DocumentVersions", entity.GetTableName());
    }

    [Fact]
    public void DocumentVersion_Has_Unique_Index_On_DocumentId_And_VersionNumber()
    {
        var entity = BuildModel().FindEntityType(typeof(DocumentVersion))!;

        var index = entity.GetIndexes().Single();

        Assert.True(index.IsUnique);
        Assert.Equal(
            new[] { nameof(DocumentVersion.DocumentId), nameof(DocumentVersion.VersionNumber) },
            index.Properties.Select(p => p.Name));
    }

    [Fact]
    public void DocumentVersion_ForeignKey_To_Document_Restricts_Delete()
    {
        var entity = BuildModel().FindEntityType(typeof(DocumentVersion))!;

        var foreignKey = entity.GetForeignKeys().Single();

        Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
        Assert.Equal("Documents", foreignKey.PrincipalEntityType.GetTableName());
    }
}
