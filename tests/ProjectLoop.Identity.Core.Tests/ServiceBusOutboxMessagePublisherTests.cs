using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class ServiceBusOutboxMessagePublisherTests
{
    [Fact]
    public void BuildMessage_Maps_Outbox_Row_Identity_And_Body_Onto_The_Service_Bus_Message()
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventId = Guid.NewGuid(),
            EventType = "ClientUserInvited",
            EventVersion = 1,
            Payload = "{\"eventType\":\"ClientUserInvited\"}",
            CorrelationId = "correlation-1",
            Status = OutboxMessageStatus.Pending,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        };

        var sbMessage = ServiceBusOutboxMessagePublisher.BuildMessage(outboxMessage);

        Assert.Equal(outboxMessage.EventId.ToString(), sbMessage.MessageId);
        Assert.Equal(outboxMessage.EventType, sbMessage.Subject);
        Assert.Equal("application/json", sbMessage.ContentType);
        Assert.Equal(outboxMessage.CorrelationId, sbMessage.CorrelationId);
        Assert.Equal(outboxMessage.Payload, sbMessage.Body.ToString());
    }
}
