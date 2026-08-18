using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class DocumentPublishedApprovalPolicyTests
{
    private static DocumentPublishedV1 CreateEvent(DocumentPublishedVisibility visibility) => new(
        DocumentId: Guid.NewGuid(),
        DocumentVersionId: Guid.NewGuid(),
        VersionNumber: 1,
        ProjectId: Guid.NewGuid(),
        TenantId: Guid.NewGuid(),
        Category: "Contract",
        Visibility: visibility,
        PublishedAtUtc: DateTimeOffset.UtcNow);

    [Fact]
    public void RequiresApproval_Is_True_For_Client_Visibility()
    {
        var result = DocumentPublishedApprovalPolicy.RequiresApproval(CreateEvent(DocumentPublishedVisibility.Client));

        Assert.True(result);
    }

    [Fact]
    public void RequiresApproval_Is_False_For_Internal_Visibility()
    {
        var result = DocumentPublishedApprovalPolicy.RequiresApproval(CreateEvent(DocumentPublishedVisibility.Internal));

        Assert.False(result);
    }
}
