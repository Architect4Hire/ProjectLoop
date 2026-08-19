using Microsoft.EntityFrameworkCore;
using ProjectLoop.Contracts;

namespace ProjectLoop.Audit.Core;

public sealed class ApprovalRequestedAuditConsumerTransaction : IApprovalRequestedAuditConsumerTransaction
{
    private readonly AuditDbContext _dbContext;
    private readonly IAuditRecordRepository _repository;

    public ApprovalRequestedAuditConsumerTransaction(AuditDbContext dbContext, IAuditRecordRepository repository)
    {
        _dbContext = dbContext;
        _repository = repository;
    }

    public async Task ExecuteAsync(
        IntegrationEventEnvelope<ApprovalRequestedV1> envelope,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _dbContext.InboxMessages
            .AnyAsync(m => m.MessageId == envelope.EventId, cancellationToken);
        if (alreadyProcessed)
        {
            // At-least-once redelivery of a message this consumer already
            // applied. Returning without touching AuditRecords is what
            // keeps a duplicate ApprovalRequested delivery from ever
            // producing a duplicate audit record.
            return;
        }

        var record = ApprovalRequestedAuditMapper.Map(envelope);
        await _repository.AddAsync(record, cancellationToken);

        _dbContext.InboxMessages.Add(new InboxMessage
        {
            Id = Guid.NewGuid(),
            MessageId = envelope.EventId,
            EventType = envelope.EventType,
            CorrelationId = envelope.CorrelationId,
            ProcessedAtUtc = processedAtUtc,
        });

        // A single SaveChangesAsync call commits the inbox completion
        // record together with the AuditRecord as one atomic unit of work.
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
