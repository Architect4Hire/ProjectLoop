# Add Inbox Consumer

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Choose stable message identity.
2. Check/process inbox in the same transaction as durable side effects.
3. Do not mark processed before effects commit.
4. Return safely on duplicates.
5. Expose metrics for duplicate/failed processing.
6. Test re-delivery.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
