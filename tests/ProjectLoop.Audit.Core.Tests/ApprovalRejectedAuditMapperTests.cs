using ProjectLoop.Audit.Core;
using ProjectLoop.Contracts;
using Xunit;

namespace ProjectLoop.Audit.Core.Tests;

public class ApprovalRejectedAuditMapperTests
{
    private static IntegrationEventEnvelope<ApprovalRejectedV1> CreateEnvelope(string? comments = "Missing sign-off") => new(
        EventId: Guid.NewGuid(),
        EventType: "ApprovalRejected",
        EventVersion: 1,
        OccurredAtUtc: DateTimeOffset.UtcNow,
        TenantId: Guid.NewGuid(),
        CorrelationId: "correlation-1",
        CausationId: "causation-1",
        TraceParent: null,
        Data: new ApprovalRejectedV1(
            ApprovalDecisionId: Guid.NewGuid(),
            ApprovalRequestId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid(),
            TargetType: "Document",
            TargetId: Guid.NewGuid(),
            TargetVersionId: Guid.NewGuid(),
            ApproverUserId: "approver-1",
            Comments: comments,
            DecidedAtUtc: DateTimeOffset.UtcNow));

    [Fact]
    public void Map_Produces_Record_With_Actor_Resource_And_Correlation_From_The_Event()
    {
        var envelope = CreateEnvelope();

        var record = ApprovalRejectedAuditMapper.Map(envelope);

        Assert.Equal(envelope.Data.TenantId, record.TenantId);
        Assert.Equal(envelope.Data.ApproverUserId, record.ActorUserId);
        Assert.Equal("ApprovalRejected", record.Action);
        Assert.Equal(envelope.Data.TargetType, record.ResourceType);
        Assert.Equal(envelope.Data.TargetId, record.ResourceId);
        Assert.Equal(envelope.Data.TargetVersionId, record.ResourceVersionId);
        Assert.Equal(envelope.Data.DecidedAtUtc, record.OccurredAtUtc);
        Assert.Equal(envelope.CorrelationId, record.CorrelationId);
        Assert.Equal(envelope.CausationId, record.CausationId);
        Assert.Equal("Approvals", record.Source);
        Assert.Null(record.BeforeMetadata);
        Assert.Contains("Rejected", record.AfterMetadata);
    }
}
