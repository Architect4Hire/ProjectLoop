using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class MilestoneSummaryResponseTests
{
    [Fact]
    public void MilestoneSummaryResponse_Can_Be_Created_With_Required_Members()
    {
        var updatedAtUtc = DateTimeOffset.UtcNow;
        var id = Guid.NewGuid();

        var response = new MilestoneSummaryResponse
        {
            Id = id,
            Name = "Discovery Complete",
            Status = MilestoneStatus.Completed,
            UpdatedAtUtc = updatedAtUtc,
        };

        Assert.Equal(id, response.Id);
        Assert.Equal("Discovery Complete", response.Name);
        Assert.Equal(MilestoneStatus.Completed, response.Status);
        Assert.Equal(updatedAtUtc, response.UpdatedAtUtc);
    }
}
