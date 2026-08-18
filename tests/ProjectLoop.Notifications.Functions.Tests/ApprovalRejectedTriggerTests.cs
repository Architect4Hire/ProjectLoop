using System.Text.Json;
using ProjectLoop.Contracts;
using ProjectLoop.Notifications.Core;
using ProjectLoop.Notifications.Functions;
using Xunit;

namespace ProjectLoop.Notifications.Functions.Tests;

public class ApprovalRejectedTriggerTests
{
    private sealed class FakeApprovalRejectedNotificationConsumer : IApprovalRejectedNotificationConsumer
    {
        public IntegrationEventEnvelope<ApprovalRejectedV1>? ReceivedEnvelope { get; private set; }

        public Task ConsumeAsync(IntegrationEventEnvelope<ApprovalRejectedV1> envelope, CancellationToken cancellationToken = default)
        {
            ReceivedEnvelope = envelope;
            return Task.CompletedTask;
        }
    }

    private static IntegrationEventEnvelope<ApprovalRejectedV1> CreateEnvelope() => new(
        EventId: Guid.NewGuid(),
        EventType: "ApprovalRejected",
        EventVersion: 1,
        OccurredAtUtc: DateTimeOffset.UtcNow,
        TenantId: Guid.NewGuid(),
        CorrelationId: "correlation-1",
        CausationId: null,
        TraceParent: null,
        Data: new ApprovalRejectedV1(
            ApprovalDecisionId: Guid.NewGuid(),
            ApprovalRequestId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid(),
            TargetType: "DocumentVersion",
            TargetId: Guid.NewGuid(),
            TargetVersionId: Guid.NewGuid(),
            ApproverUserId: "approver-1",
            Comments: "Needs revision",
            DecidedAtUtc: DateTimeOffset.UtcNow));

    [Fact]
    public async Task RunAsync_Deserializes_The_Envelope_And_Delegates_To_The_Injected_Consumer()
    {
        var consumer = new FakeApprovalRejectedNotificationConsumer();
        var trigger = new ApprovalRejectedTrigger(consumer);
        var envelope = CreateEnvelope();
        var message = JsonSerializer.Serialize(envelope);

        await trigger.RunAsync(message, cancellationToken: default);

        Assert.NotNull(consumer.ReceivedEnvelope);
        Assert.Equal(envelope, consumer.ReceivedEnvelope);
    }

    [Fact]
    public async Task RunAsync_Throws_On_Unparseable_Payload_Rather_Than_Silently_Dropping_It()
    {
        var consumer = new FakeApprovalRejectedNotificationConsumer();
        var trigger = new ApprovalRejectedTrigger(consumer);

        await Assert.ThrowsAsync<JsonException>(() => trigger.RunAsync("not-json", cancellationToken: default));
    }
}
