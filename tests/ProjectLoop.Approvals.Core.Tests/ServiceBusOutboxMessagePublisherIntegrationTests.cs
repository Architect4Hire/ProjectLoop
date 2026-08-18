using Azure.Messaging.ServiceBus;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

/// <summary>
/// Exercises ServiceBusOutboxMessagePublisher against a real Service
/// Bus/Aspire-emulator endpoint. Skipped when no endpoint is configured,
/// since it depends on infrastructure external to the unit test suite.
/// </summary>
public class ServiceBusOutboxMessagePublisherIntegrationTests
{
    private const string ConnectionStringEnvironmentVariable = "APPROVALS_SERVICEBUS_TEST_CONNECTION_STRING";
    private const string EntityNameEnvironmentVariable = "APPROVALS_SERVICEBUS_TEST_ENTITY_NAME";

    private static bool TryGetConnection(out string connectionString, out string entityName)
    {
        connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable) ?? string.Empty;
        entityName = Environment.GetEnvironmentVariable(EntityNameEnvironmentVariable) ?? string.Empty;
        return !string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(entityName);
    }

    [Fact]
    public async Task PublishAsync_Sends_The_Outbox_Row_To_The_Configured_Entity()
    {
        if (!TryGetConnection(out var connectionString, out var entityName))
        {
            return;
        }

        await using var client = new ServiceBusClient(connectionString);
        await using var sender = client.CreateSender(entityName);
        var publisher = new ServiceBusOutboxMessagePublisher(sender);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventId = Guid.NewGuid(),
            EventType = "ApprovalRequested",
            EventVersion = 1,
            Payload = "{\"eventType\":\"ApprovalRequested\"}",
            Status = OutboxMessageStatus.Pending,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        };

        await publisher.PublishAsync(outboxMessage);
    }
}
