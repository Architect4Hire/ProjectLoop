namespace ProjectLoop.Audit.Functions;

/// <summary>
/// Audit.Functions hosts Service Bus and timer-based triggers for Audit service.
///
/// Responsibilities:
/// - Transport binding for asynchronous workloads
/// - Deserialization and error handling
/// - Delegation to Core Facade boundary
/// - No domain logic; all business rules in Core
/// </summary>
[ExcludeFromCodeCoverage]
internal static class AuditFunctionsNamespace { }
