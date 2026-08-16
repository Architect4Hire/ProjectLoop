# Add Audit Record

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Identify business/security fact.
2. Capture actor, tenant, resource, action, UTC time and correlation.
3. Avoid sensitive payload dumping.
4. Make history append-oriented.
5. Persist independently from diagnostic logs.
6. Test privileged/cross-tenant actions.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
