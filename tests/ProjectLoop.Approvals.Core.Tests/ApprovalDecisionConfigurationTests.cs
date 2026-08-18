using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalDecisionConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new ApprovalRequestConfiguration());
        modelBuilder.ApplyConfiguration(new ApprovalDecisionConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void ApprovalDecision_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(ApprovalDecision))!;

        Assert.Equal("ApprovalDecisions", entity.GetTableName());
    }

    [Fact]
    public void ApprovalDecision_ForeignKey_To_ApprovalRequest_Restricts_Delete()
    {
        var entity = BuildModel().FindEntityType(typeof(ApprovalDecision))!;

        var foreignKey = entity.GetForeignKeys().Single();

        Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
        Assert.Equal("ApprovalRequests", foreignKey.PrincipalEntityType.GetTableName());
    }

    [Fact]
    public void ApprovalDecision_Properties_Are_Init_Only()
    {
        // Every property is `init`, not `set` — the CLR type itself forbids
        // mutation after construction, so decision rows can only ever be
        // inserted, never updated.
        var isInitOnly = typeof(ApprovalDecision).GetProperties()
            .Where(p => p.CanWrite)
            .All(p => p.SetMethod!.ReturnParameter
                .GetRequiredCustomModifiers()
                .Any(m => m == typeof(System.Runtime.CompilerServices.IsExternalInit)));

        Assert.True(isInitOnly);
    }
}
