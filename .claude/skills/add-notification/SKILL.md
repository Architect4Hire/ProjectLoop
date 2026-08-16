# Add Notification

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Trigger from an integration fact, not a synchronous domain dependency.
2. Model recipient/template/delivery attempt.
3. Make delivery retryable and idempotent.
4. Do not roll back source business transaction on failure.
5. Instrument outcomes and dead-letter handling.
6. Test duplicate event delivery.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
