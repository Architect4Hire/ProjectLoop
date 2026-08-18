using System.Diagnostics.CodeAnalysis;

namespace ProjectLoop.Contracts;

/// <summary>
/// ProjectLoop.Contracts holds cross-service integration-event contracts
/// only: the shared envelope and versioned event payloads. It never
/// references EF entities or any bounded service's internal types.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ContractsNamespace { }
