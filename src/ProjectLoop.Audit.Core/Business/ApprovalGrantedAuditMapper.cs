using System.Text.Json;
using ProjectLoop.Contracts;

namespace ProjectLoop.Audit.Core;

/// <summary>
/// Maps an ApprovalGranted integration event to a safe AuditRecord
/// projection. Comments are approver-authored business content captured as
/// part of the decision record (per the Approvals rule), never raw document
/// bodies or credentials, so they are safe to include in AfterMetadata.
/// </summary>
public static class ApprovalGrantedAuditMapper
{
    private const string ActionName = "ApprovalGranted";
    private const string SourceName = "Approvals";

    public static AuditRecord Map(IntegrationEventEnvelope<ApprovalGrantedV1> envelope)
    {
        var data = envelope.Data;

        var afterMetadata = JsonSerializer.Serialize(new
        {
            data.ApprovalDecisionId,
            data.ApprovalRequestId,
            data.ProjectId,
            Decision = "Approved",
            data.Comments,
        });

        return new AuditRecord
        {
            Id = Guid.NewGuid(),
            TenantId = data.TenantId,
            ActorUserId = data.ApproverUserId,
            Action = ActionName,
            ResourceType = data.TargetType,
            ResourceId = data.TargetId,
            ResourceVersionId = data.TargetVersionId,
            OccurredAtUtc = data.DecidedAtUtc,
            CorrelationId = envelope.CorrelationId,
            CausationId = envelope.CausationId,
            Source = SourceName,
            BeforeMetadata = null,
            AfterMetadata = afterMetadata,
        };
    }
}
