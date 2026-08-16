# Add Outbox Publisher

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Write business state and outbox in one transaction.
2. Store required envelope metadata.
3. Publish only from relay/dispatcher.
4. Mark dispatched after broker acknowledgement.
5. Make relay retries safe and observable.
6. Test failure between commit and publication.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
