namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Engagement.Core contains engagement domain contracts and business logic.
///
/// Layering:
/// - Facade: use-case validation, orchestration, authorization
/// - Business: domain rules, state transitions, decisions
/// - Data: transaction boundaries, repository composition
/// - Repository: persistence operations
/// - DbContext: EF mapping
/// </summary>
[ExcludeFromCodeCoverage]
internal static class EngagementCoreNamespace { }
