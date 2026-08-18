namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalDecisionTransaction : IApprovalDecisionTransaction
{
    private readonly ApprovalsDbContext _dbContext;
    private readonly IApprovalRequestRepository _approvalRequestRepository;
    private readonly IApprovalDecisionRepository _approvalDecisionRepository;

    public ApprovalDecisionTransaction(
        ApprovalsDbContext dbContext,
        IApprovalRequestRepository approvalRequestRepository,
        IApprovalDecisionRepository approvalDecisionRepository)
    {
        _dbContext = dbContext;
        _approvalRequestRepository = approvalRequestRepository;
        _approvalDecisionRepository = approvalDecisionRepository;
    }

    public async Task<ApprovalDecision> ExecuteAsync(
        ApprovalRequest request,
        ApprovalRequestStatus outcome,
        string approverUserId,
        string? comments,
        DateTimeOffset decidedAtUtc,
        CancellationToken cancellationToken = default)
    {
        _approvalRequestRepository.ApplyDecision(request, outcome);

        var decision = new ApprovalDecision
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            ApprovalRequestId = request.Id,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            TargetVersionId = request.TargetVersionId,
            ApproverUserId = approverUserId,
            Decision = outcome,
            Comments = comments,
            DecidedAtUtc = decidedAtUtc,
            CorrelationId = request.CorrelationId,
        };

        await _approvalDecisionRepository.AddAsync(decision, cancellationToken);

        // A single SaveChangesAsync call commits the request's terminal
        // state and the appended decision row as one atomic unit of work.
        // No ApprovalGranted/ApprovalRejected outbox event is raised here —
        // that publication is added by a later, separately scoped microstep.
        await _dbContext.SaveChangesAsync(cancellationToken);

        return decision;
    }
}
