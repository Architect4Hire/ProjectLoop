namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Approvals.Core contains approvals domain contracts and business logic.
///
/// Layering:
/// - Facade: use-case validation, orchestration, authorization
/// - Business: domain rules, state transitions, decisions
/// - Data: transaction boundaries, repository composition
/// - Repository: persistence operations
/// - DbContext: EF mapping
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ApprovalsCoreNamespace { }
