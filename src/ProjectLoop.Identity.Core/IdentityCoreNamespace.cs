namespace ProjectLoop.Identity.Core;

/// <summary>
/// Identity.Core contains identity domain contracts and business logic.
///
/// Layering:
/// - Facade: use-case validation, orchestration, authorization
/// - Business: domain rules, state transitions, decisions
/// - Data: transaction boundaries, repository composition
/// - Repository: persistence operations
/// - DbContext: EF mapping
/// </summary>
[ExcludeFromCodeCoverage]
internal static class IdentityCoreNamespace { }
