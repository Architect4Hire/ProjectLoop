using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class ClientInvitationConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new ClientInvitationConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void ClientInvitation_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(ClientInvitation))!;

        Assert.Equal("ClientInvitations", entity.GetTableName());
    }

    [Fact]
    public void ClientInvitation_TokenHash_Has_Unique_Index()
    {
        var entity = BuildModel().FindEntityType(typeof(ClientInvitation))!;

        var index = entity.GetIndexes().Single(i => i.IsUnique);

        Assert.Equal(new[] { nameof(ClientInvitation.TokenHash) }, index.Properties.Select(p => p.Name));
    }
}
