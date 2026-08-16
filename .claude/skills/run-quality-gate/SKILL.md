# Run Quality Gate

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Build all affected .NET and Angular projects.
2. Run unit/integration/component tests.
3. Run format/lint/type-check.
4. Review tenant/authorization boundaries.
5. Review HTTP-vs-event choice and idempotency.
6. Verify docs/ADR consistency for architecture-impacting changes.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
