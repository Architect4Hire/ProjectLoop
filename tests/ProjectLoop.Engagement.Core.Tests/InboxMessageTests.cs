using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class InboxMessageTests
{
    [Fact]
    public void New_InboxMessage_Carries_The_Producers_Stable_EventId_As_MessageId()
    {
        var messageId = Guid.NewGuid();

        var inboxMessage = new InboxMessage
        {
            Id = Guid.NewGuid(),
            MessageId = messageId,
            EventType = "ApprovalGranted",
            ProcessedAtUtc = DateTimeOffset.UtcNow,
        };

        Assert.Equal(messageId, inboxMessage.MessageId);
        Assert.Equal("ApprovalGranted", inboxMessage.EventType);
    }
}
