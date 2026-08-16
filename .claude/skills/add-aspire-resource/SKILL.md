# Add Aspire Resource

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Add resource/reference in AppHost only.
2. Use configuration/service discovery.
3. Wire health/dependency ordering.
4. Keep secrets externalized.
5. Update local developer docs.
6. Do not put application logic in AppHost.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
