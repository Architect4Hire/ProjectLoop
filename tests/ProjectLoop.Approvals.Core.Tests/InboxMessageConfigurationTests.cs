using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class InboxMessageConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void InboxMessage_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(InboxMessage))!;

        Assert.Equal("InboxMessages", entity.GetTableName());
    }

    [Fact]
    public void InboxMessage_Has_A_Unique_Index_On_MessageId()
    {
        var entity = BuildModel().FindEntityType(typeof(InboxMessage))!;

        var index = entity.GetIndexes().Single(i =>
            i.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(InboxMessage.MessageId) }));

        Assert.True(index.IsUnique);
    }
}
