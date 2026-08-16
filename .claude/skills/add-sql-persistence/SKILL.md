# Add SQL Persistence

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Confirm owning service database.
2. Model tenant key and indexes.
3. Create EF mapping/migration.
4. Keep transaction local.
5. Add repository/data-layer tests.
6. Never introduce cross-service joins.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
