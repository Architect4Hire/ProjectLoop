using System.Diagnostics;
using System.Text.Json;
using ProjectLoop.Contracts;

namespace ProjectLoop.Documents.Core;

public sealed class DocumentPublishTransaction : IDocumentPublishTransaction
{
    private readonly DocumentsDbContext _dbContext;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _documentVersionRepository;

    public DocumentPublishTransaction(
        DocumentsDbContext dbContext,
        IDocumentRepository documentRepository,
        IDocumentVersionRepository documentVersionRepository)
    {
        _dbContext = dbContext;
        _documentRepository = documentRepository;
        _documentVersionRepository = documentVersionRepository;
    }

    public async Task ExecuteAsync(
        Document document,
        DocumentVersion version,
        DateTimeOffset publishedAtUtc,
        CancellationToken cancellationToken = default)
    {
        _documentVersionRepository.MarkPublished(version, publishedAtUtc);
        _documentRepository.MarkPublished(document, publishedAtUtc);

        var envelope = new IntegrationEventEnvelope<DocumentPublishedV1>(
            EventId: Guid.NewGuid(),
            EventType: "DocumentPublished",
            EventVersion: 1,
            OccurredAtUtc: publishedAtUtc,
            TenantId: document.TenantId,
            CorrelationId: null,
            CausationId: null,
            TraceParent: Activity.Current?.Id,
            Data: new DocumentPublishedV1(
                DocumentId: document.Id,
                DocumentVersionId: version.Id,
                VersionNumber: version.VersionNumber,
                ProjectId: document.ProjectId,
                TenantId: document.TenantId,
                Category: document.Category,
                Visibility: document.Visibility,
                PublishedAtUtc: publishedAtUtc));

        _dbContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventId = envelope.EventId,
            EventType = envelope.EventType,
            EventVersion = envelope.EventVersion,
            Payload = JsonSerializer.Serialize(envelope),
            CorrelationId = envelope.CorrelationId,
            Status = OutboxMessageStatus.Pending,
            AttemptCount = 0,
            CreatedAtUtc = publishedAtUtc,
        });

        // A single SaveChangesAsync call commits the version's publication
        // state, the document's status transition, and the DocumentPublished
        // outbox row as one atomic unit of work — the outbox relay, not this
        // transaction, is responsible for publishing to Service Bus.
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
