using System.Diagnostics;
using System.Text.Json;
using ProjectLoop.Contracts;

namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalDecisionTransaction : IApprovalDecisionTransaction
{
    private readonly ApprovalsDbContext _dbContext;
    private readonly IApprovalRequestRepository _approvalRequestRepository;
    private readonly IApprovalDecisionRepository _approvalDecisionRepository;

    public ApprovalDecisionTransaction(
        ApprovalsDbContext dbContext,
        IApprovalRequestRepository approvalRequestRepository,
        IApprovalDecisionRepository approvalDecisionRepository)
    {
        _dbContext = dbContext;
        _approvalRequestRepository = approvalRequestRepository;
        _approvalDecisionRepository = approvalDecisionRepository;
    }

    public async Task<ApprovalDecision> ExecuteAsync(
        ApprovalRequest request,
        ApprovalRequestStatus outcome,
        string approverUserId,
        string? comments,
        DateTimeOffset decidedAtUtc,
        CancellationToken cancellationToken = default)
    {
        _approvalRequestRepository.ApplyDecision(request, outcome);

        var decision = new ApprovalDecision
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            ApprovalRequestId = request.Id,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            TargetVersionId = request.TargetVersionId,
            ApproverUserId = approverUserId,
            Decision = outcome,
            Comments = comments,
            DecidedAtUtc = decidedAtUtc,
            CorrelationId = request.CorrelationId,
        };

        await _approvalDecisionRepository.AddAsync(decision, cancellationToken);

        _dbContext.OutboxMessages.Add(decision.Decision == ApprovalRequestStatus.Approved
            ? BuildOutboxMessage("ApprovalGranted", request, new ApprovalGrantedV1(
                ApprovalDecisionId: decision.Id,
                ApprovalRequestId: request.Id,
                TenantId: request.TenantId,
                ProjectId: request.ProjectId,
                TargetType: request.TargetType,
                TargetId: request.TargetId,
                TargetVersionId: request.TargetVersionId,
                ApproverUserId: decision.ApproverUserId,
                Comments: decision.Comments,
                DecidedAtUtc: decision.DecidedAtUtc), decidedAtUtc)
            : BuildOutboxMessage("ApprovalRejected", request, new ApprovalRejectedV1(
                ApprovalDecisionId: decision.Id,
                ApprovalRequestId: request.Id,
                TenantId: request.TenantId,
                ProjectId: request.ProjectId,
                TargetType: request.TargetType,
                TargetId: request.TargetId,
                TargetVersionId: request.TargetVersionId,
                ApproverUserId: decision.ApproverUserId,
                Comments: decision.Comments,
                DecidedAtUtc: decision.DecidedAtUtc), decidedAtUtc));

        // A single SaveChangesAsync call commits the request's terminal
        // state, the appended decision row, and the ApprovalGranted/
        // ApprovalRejected outbox row as one atomic unit of work — the
        // outbox relay, not this transaction, is responsible for publishing
        // to Service Bus.
        await _dbContext.SaveChangesAsync(cancellationToken);

        return decision;
    }

    private static OutboxMessage BuildOutboxMessage<TData>(string eventType, ApprovalRequest request, TData data, DateTimeOffset occurredAtUtc)
        where TData : notnull
    {
        var envelope = new IntegrationEventEnvelope<TData>(
            EventId: Guid.NewGuid(),
            EventType: eventType,
            EventVersion: 1,
            OccurredAtUtc: occurredAtUtc,
            TenantId: request.TenantId,
            CorrelationId: request.CorrelationId,
            CausationId: null,
            TraceParent: Activity.Current?.Id,
            Data: data);

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventId = envelope.EventId,
            EventType = envelope.EventType,
            EventVersion = envelope.EventVersion,
            Payload = JsonSerializer.Serialize(envelope),
            CorrelationId = envelope.CorrelationId,
            Status = OutboxMessageStatus.Pending,
            AttemptCount = 0,
            CreatedAtUtc = envelope.OccurredAtUtc,
        };
    }
}
