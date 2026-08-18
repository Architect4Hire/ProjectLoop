namespace ProjectLoop.Engagement.Core;

public static class MilestoneMapper
{
    public static IReadOnlyList<MilestoneSummaryResponse> ToSummaryResponses(IEnumerable<Milestone> milestones) =>
        milestones
            .OrderBy(m => m.CreatedAtUtc)
            .Select(ToSummaryResponse)
            .ToList();

    private static MilestoneSummaryResponse ToSummaryResponse(Milestone milestone) =>
        new()
        {
            Id = milestone.Id,
            Name = milestone.Name,
            Status = milestone.Status,
            UpdatedAtUtc = milestone.UpdatedAtUtc,
        };
}
