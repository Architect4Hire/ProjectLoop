using System.Text.Json;
using ProjectLoop.Contracts;

namespace ProjectLoop.Audit.Core;

/// <summary>
/// Maps an ApprovalRequested integration event to a safe AuditRecord
/// projection. AfterMetadata is a minimal, non-sensitive summary of the
/// request — never the target resource's content or Blob references.
/// </summary>
public static class ApprovalRequestedAuditMapper
{
    private const string ActionName = "ApprovalRequested";
    private const string SourceName = "Approvals";

    public static AuditRecord Map(IntegrationEventEnvelope<ApprovalRequestedV1> envelope)
    {
        var data = envelope.Data;

        var afterMetadata = JsonSerializer.Serialize(new
        {
            data.ApprovalRequestId,
            data.ProjectId,
        });

        return new AuditRecord
        {
            Id = Guid.NewGuid(),
            TenantId = data.TenantId,
            ActorUserId = data.RequestedByUserId,
            Action = ActionName,
            ResourceType = data.TargetType,
            ResourceId = data.TargetId,
            ResourceVersionId = data.TargetVersionId,
            OccurredAtUtc = data.RequestedAtUtc,
            CorrelationId = envelope.CorrelationId,
            CausationId = envelope.CausationId,
            Source = SourceName,
            BeforeMetadata = null,
            AfterMetadata = afterMetadata,
        };
    }
}
