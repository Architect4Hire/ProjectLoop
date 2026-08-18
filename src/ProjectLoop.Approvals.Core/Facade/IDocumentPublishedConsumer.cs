using ProjectLoop.Contracts;

namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Use-case boundary the DocumentPublished Service Bus trigger delegates
/// into. There is no authenticated end user or tenant-context accessor here
/// — the caller is the Documents service via an asynchronous message, so
/// tenant scoping comes from the event's own TenantId rather than from
/// <see cref="ICurrentTenantContextAccessor"/>.
/// </summary>
public interface IDocumentPublishedConsumer
{
    Task ConsumeAsync(IntegrationEventEnvelope<DocumentPublishedV1> envelope, CancellationToken cancellationToken = default);
}
