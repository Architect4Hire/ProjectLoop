using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class InboxMessageTests
{
    [Fact]
    public void InboxMessage_Properties_Are_Init_Only()
    {
        // Every property is `init`, not `set` — a processed-message record can
        // only ever be inserted, never mutated back into an unprocessed state.
        var isInitOnly = typeof(InboxMessage).GetProperties()
            .Where(p => p.CanWrite)
            .All(p => p.SetMethod!.ReturnParameter
                .GetRequiredCustomModifiers()
                .Any(m => m == typeof(System.Runtime.CompilerServices.IsExternalInit)));

        Assert.True(isInitOnly);
    }

    [Fact]
    public void InboxMessage_Can_Be_Constructed_With_Required_Members()
    {
        var message = new InboxMessage
        {
            Id = Guid.NewGuid(),
            MessageId = Guid.NewGuid(),
            EventType = "DocumentPublished",
            CorrelationId = "correlation-1",
            ProcessedAtUtc = DateTimeOffset.UtcNow,
        };

        Assert.Equal("DocumentPublished", message.EventType);
    }
}
