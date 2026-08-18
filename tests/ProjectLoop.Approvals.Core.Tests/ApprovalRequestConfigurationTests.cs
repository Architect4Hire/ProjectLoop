using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalRequestConfigurationTests
{
    private static IModel BuildModel()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new ApprovalRequestConfiguration());
        return modelBuilder.FinalizeModel();
    }

    [Fact]
    public void ApprovalRequest_Maps_To_Expected_Table()
    {
        var entity = BuildModel().FindEntityType(typeof(ApprovalRequest))!;

        Assert.Equal("ApprovalRequests", entity.GetTableName());
    }

    [Fact]
    public void ApprovalRequest_Has_Index_On_TenantId_And_ProjectId()
    {
        var entity = BuildModel().FindEntityType(typeof(ApprovalRequest))!;

        var index = entity.GetIndexes().Single(i =>
            i.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(ApprovalRequest.TenantId), nameof(ApprovalRequest.ProjectId) }));

        Assert.NotNull(index);
    }

    [Fact]
    public void ApprovalRequest_Has_Index_On_Exact_Target_Identity()
    {
        var entity = BuildModel().FindEntityType(typeof(ApprovalRequest))!;

        var index = entity.GetIndexes().Single(i =>
            i.Properties.Select(p => p.Name).SequenceEqual(new[]
            {
                nameof(ApprovalRequest.TenantId),
                nameof(ApprovalRequest.TargetType),
                nameof(ApprovalRequest.TargetId),
                nameof(ApprovalRequest.TargetVersionId),
            }));

        Assert.NotNull(index);
    }

    [Fact]
    public void ApprovalRequest_Status_Is_Stored_As_String()
    {
        var entity = BuildModel().FindEntityType(typeof(ApprovalRequest))!;
        var status = entity.FindProperty(nameof(ApprovalRequest.Status))!;

        Assert.Equal(typeof(string), status.GetProviderClrType());
    }
}
