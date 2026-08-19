using ProjectLoop.Contracts;

namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Use-case boundary the ApprovalRejected Service Bus trigger delegates
/// into. There is no authenticated end user or tenant-context accessor here
/// — the caller is the Approvals service via an asynchronous message, so
/// tenant scoping comes from the event's own TenantId.
/// </summary>
public interface IApprovalRejectedMilestoneConsumer
{
    Task ConsumeAsync(IntegrationEventEnvelope<ApprovalRejectedV1> envelope, CancellationToken cancellationToken = default);
}
