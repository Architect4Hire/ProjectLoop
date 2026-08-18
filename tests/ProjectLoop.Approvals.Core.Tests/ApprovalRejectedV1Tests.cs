using System.Reflection;
using System.Text.Json;
using ProjectLoop.Approvals.Core;
using ProjectLoop.Contracts;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalRejectedV1Tests
{
    private static ApprovalRejectedV1 CreateEvent() => new(
        ApprovalDecisionId: Guid.NewGuid(),
        ApprovalRequestId: Guid.NewGuid(),
        TenantId: Guid.NewGuid(),
        ProjectId: Guid.NewGuid(),
        TargetType: "DocumentVersion",
        TargetId: Guid.NewGuid(),
        TargetVersionId: Guid.NewGuid(),
        ApproverUserId: "approver-1",
        Comments: "Needs revisions.",
        DecidedAtUtc: DateTimeOffset.UtcNow);

    [Fact]
    public void ApprovalRejectedV1_Round_Trips_Through_Envelope_Json()
    {
        var envelope = new IntegrationEventEnvelope<ApprovalRejectedV1>(
            EventId: Guid.NewGuid(),
            EventType: "ApprovalRejected",
            EventVersion: 1,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            TenantId: Guid.NewGuid(),
            CorrelationId: "correlation-1",
            CausationId: null,
            TraceParent: null,
            Data: CreateEvent());

        var json = JsonSerializer.Serialize(envelope);
        var roundTripped = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalRejectedV1>>(json);

        Assert.NotNull(roundTripped);
        Assert.Equal(envelope, roundTripped);
    }

    [Fact]
    public void ApprovalRejectedV1_Never_Exposes_A_Blob_Key_Or_Credential()
    {
        var propertyNames = typeof(ApprovalRejectedV1)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name);

        Assert.DoesNotContain(propertyNames, name =>
            name.Contains("Blob", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Url", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Uri", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Sas", StringComparison.OrdinalIgnoreCase));
    }
}
