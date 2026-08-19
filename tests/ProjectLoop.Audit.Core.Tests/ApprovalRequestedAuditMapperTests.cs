using ProjectLoop.Audit.Core;
using ProjectLoop.Contracts;
using Xunit;

namespace ProjectLoop.Audit.Core.Tests;

public class ApprovalRequestedAuditMapperTests
{
    private static IntegrationEventEnvelope<ApprovalRequestedV1> CreateEnvelope() => new(
        EventId: Guid.NewGuid(),
        EventType: "ApprovalRequested",
        EventVersion: 1,
        OccurredAtUtc: DateTimeOffset.UtcNow,
        TenantId: Guid.NewGuid(),
        CorrelationId: "correlation-1",
        CausationId: "causation-1",
        TraceParent: null,
        Data: new ApprovalRequestedV1(
            ApprovalRequestId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid(),
            TargetType: "Document",
            TargetId: Guid.NewGuid(),
            TargetVersionId: Guid.NewGuid(),
            RequestedByUserId: "user-1",
            RequestedAtUtc: DateTimeOffset.UtcNow));

    [Fact]
    public void Map_Produces_Record_With_Actor_Resource_And_Correlation_From_The_Event()
    {
        var envelope = CreateEnvelope();

        var record = ApprovalRequestedAuditMapper.Map(envelope);

        Assert.Equal(envelope.Data.TenantId, record.TenantId);
        Assert.Equal(envelope.Data.RequestedByUserId, record.ActorUserId);
        Assert.Equal("ApprovalRequested", record.Action);
        Assert.Equal(envelope.Data.TargetType, record.ResourceType);
        Assert.Equal(envelope.Data.TargetId, record.ResourceId);
        Assert.Equal(envelope.Data.TargetVersionId, record.ResourceVersionId);
        Assert.Equal(envelope.Data.RequestedAtUtc, record.OccurredAtUtc);
        Assert.Equal(envelope.CorrelationId, record.CorrelationId);
        Assert.Equal(envelope.CausationId, record.CausationId);
        Assert.Equal("Approvals", record.Source);
    }

    [Fact]
    public void Map_Does_Not_Leak_Raw_Event_Payload_Fields_Not_Meant_For_Audit_Metadata()
    {
        var envelope = CreateEnvelope();

        var record = ApprovalRequestedAuditMapper.Map(envelope);

        // Only a minimal, safe projection is carried in AfterMetadata.
        Assert.DoesNotContain(envelope.Data.RequestedByUserId, record.AfterMetadata);
        Assert.Null(record.BeforeMetadata);
    }
}
