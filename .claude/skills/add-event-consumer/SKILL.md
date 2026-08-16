# Add Event Consumer

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Define why async delivery is correct.
2. Make handling idempotent.
3. Use inbox when durable side effects need duplicate protection.
4. Keep trigger thin and delegate inward.
5. Handle retry/dead-letter/observability.
6. Add duplicate-delivery tests.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
