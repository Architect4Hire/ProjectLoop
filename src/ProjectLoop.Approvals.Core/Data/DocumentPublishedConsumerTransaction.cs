using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectLoop.Contracts;

namespace ProjectLoop.Approvals.Core;

public sealed class DocumentPublishedConsumerTransaction : IDocumentPublishedConsumerTransaction
{
    private readonly ApprovalsDbContext _dbContext;
    private readonly IApprovalRequestRepository _approvalRequestRepository;

    public DocumentPublishedConsumerTransaction(
        ApprovalsDbContext dbContext,
        IApprovalRequestRepository approvalRequestRepository)
    {
        _dbContext = dbContext;
        _approvalRequestRepository = approvalRequestRepository;
    }

    public async Task ExecuteAsync(
        IntegrationEventEnvelope<DocumentPublishedV1> envelope,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _dbContext.InboxMessages
            .AnyAsync(m => m.MessageId == envelope.EventId, cancellationToken);
        if (alreadyProcessed)
        {
            // At-least-once redelivery of a message this consumer already
            // applied. Returning without touching ApprovalRequests or the
            // outbox is what keeps a duplicate DocumentPublished delivery
            // from ever creating a duplicate approval or a duplicate logical
            // ApprovalRequested event.
            return;
        }

        if (DocumentPublishedApprovalPolicy.RequiresApproval(envelope.Data))
        {
            var request = new ApprovalRequest
            {
                Id = Guid.NewGuid(),
                TenantId = envelope.Data.TenantId,
                ProjectId = envelope.Data.ProjectId,
                TargetType = "DocumentVersion",
                TargetId = envelope.Data.DocumentId,
                TargetVersionId = envelope.Data.DocumentVersionId,
                RequestedByUserId = "system:document-published",
                RequestedAtUtc = processedAtUtc,
                Status = ApprovalRequestStatus.Pending,
                CorrelationId = envelope.CorrelationId,
            };

            _approvalRequestRepository.Add(request);

            var requestedEnvelope = new IntegrationEventEnvelope<ApprovalRequestedV1>(
                EventId: Guid.NewGuid(),
                EventType: "ApprovalRequested",
                EventVersion: 1,
                OccurredAtUtc: processedAtUtc,
                TenantId: request.TenantId,
                CorrelationId: request.CorrelationId,
                CausationId: envelope.EventId.ToString(),
                TraceParent: envelope.TraceParent,
                Data: new ApprovalRequestedV1(
                    ApprovalRequestId: request.Id,
                    TenantId: request.TenantId,
                    ProjectId: request.ProjectId,
                    TargetType: request.TargetType,
                    TargetId: request.TargetId,
                    TargetVersionId: request.TargetVersionId,
                    RequestedByUserId: request.RequestedByUserId,
                    RequestedAtUtc: request.RequestedAtUtc));

            _dbContext.OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventId = requestedEnvelope.EventId,
                EventType = requestedEnvelope.EventType,
                EventVersion = requestedEnvelope.EventVersion,
                Payload = JsonSerializer.Serialize(requestedEnvelope),
                CorrelationId = requestedEnvelope.CorrelationId,
                Status = OutboxMessageStatus.Pending,
                AttemptCount = 0,
                CreatedAtUtc = processedAtUtc,
            });
        }

        _dbContext.InboxMessages.Add(new InboxMessage
        {
            Id = Guid.NewGuid(),
            MessageId = envelope.EventId,
            EventType = envelope.EventType,
            CorrelationId = envelope.CorrelationId,
            ProcessedAtUtc = processedAtUtc,
        });

        // A single SaveChangesAsync call commits the inbox completion record
        // together with the ApprovalRequest and ApprovalRequested outbox row
        // (when policy required one) as one atomic unit of work — the outbox
        // relay, not this transaction, is responsible for publishing to
        // Service Bus.
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
