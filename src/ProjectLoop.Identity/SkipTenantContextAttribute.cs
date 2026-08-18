namespace ProjectLoop.Identity;

/// <summary>
/// Marks an endpoint that authenticates the caller but does not require an
/// established tenant context — e.g. invitation acceptance, where accepting
/// is exactly what creates the caller's first membership.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class SkipTenantContextAttribute : Attribute
{
}
