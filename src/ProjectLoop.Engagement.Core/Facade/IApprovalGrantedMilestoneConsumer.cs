using ProjectLoop.Contracts;

namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Use-case boundary the ApprovalGranted Service Bus trigger delegates
/// into. There is no authenticated end user or tenant-context accessor here
/// — the caller is the Approvals service via an asynchronous message, so
/// tenant scoping comes from the event's own TenantId.
/// </summary>
public interface IApprovalGrantedMilestoneConsumer
{
    Task ConsumeAsync(IntegrationEventEnvelope<ApprovalGrantedV1> envelope, CancellationToken cancellationToken = default);
}
