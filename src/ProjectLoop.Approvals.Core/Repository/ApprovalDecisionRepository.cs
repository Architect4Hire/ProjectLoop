namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalDecisionRepository : IApprovalDecisionRepository
{
    private readonly ApprovalsDbContext _dbContext;

    public ApprovalDecisionRepository(ApprovalsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(ApprovalDecision decision, CancellationToken cancellationToken = default)
    {
        _dbContext.ApprovalDecisions.Add(decision);
        return Task.CompletedTask;
    }
}
