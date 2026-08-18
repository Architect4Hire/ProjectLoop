using System.Text.Json;
using ProjectLoop.Approvals.Core;
using ProjectLoop.Approvals.Functions;
using ProjectLoop.Contracts;
using Xunit;

namespace ProjectLoop.Approvals.Functions.Tests;

public class DocumentPublishedTriggerTests
{
    private sealed class FakeDocumentPublishedConsumer : IDocumentPublishedConsumer
    {
        public IntegrationEventEnvelope<DocumentPublishedV1>? ReceivedEnvelope { get; private set; }

        public Task ConsumeAsync(IntegrationEventEnvelope<DocumentPublishedV1> envelope, CancellationToken cancellationToken = default)
        {
            ReceivedEnvelope = envelope;
            return Task.CompletedTask;
        }
    }

    private static IntegrationEventEnvelope<DocumentPublishedV1> CreateEnvelope() => new(
        EventId: Guid.NewGuid(),
        EventType: "DocumentPublished",
        EventVersion: 1,
        OccurredAtUtc: DateTimeOffset.UtcNow,
        TenantId: Guid.NewGuid(),
        CorrelationId: "correlation-1",
        CausationId: null,
        TraceParent: null,
        Data: new DocumentPublishedV1(
            DocumentId: Guid.NewGuid(),
            DocumentVersionId: Guid.NewGuid(),
            VersionNumber: 1,
            ProjectId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            Category: "Contract",
            Visibility: DocumentPublishedVisibility.Client,
            PublishedAtUtc: DateTimeOffset.UtcNow));

    [Fact]
    public async Task RunAsync_Deserializes_The_Envelope_And_Delegates_To_The_Injected_Consumer()
    {
        var consumer = new FakeDocumentPublishedConsumer();
        var trigger = new DocumentPublishedTrigger(consumer);
        var envelope = CreateEnvelope();
        var message = JsonSerializer.Serialize(envelope);

        await trigger.RunAsync(message, cancellationToken: default);

        Assert.NotNull(consumer.ReceivedEnvelope);
        Assert.Equal(envelope, consumer.ReceivedEnvelope);
    }

    [Fact]
    public async Task RunAsync_Throws_On_Unparseable_Payload_Rather_Than_Silently_Dropping_It()
    {
        var consumer = new FakeDocumentPublishedConsumer();
        var trigger = new DocumentPublishedTrigger(consumer);

        await Assert.ThrowsAsync<JsonException>(() => trigger.RunAsync("not-json", cancellationToken: default));
    }
}
