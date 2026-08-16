# Add Tenant-Aware Feature

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Resolve tenant from authenticated membership, not caller claim alone.
2. Scope every data access.
3. Namespace cache/blob/event/audit data.
4. Test cross-tenant read/write denial.
5. Review admin bypass paths explicitly.
6. Run tenant-boundary reviewer.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
