# Add Approval Workflow

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Identify exact target resource/version.
2. Create explicit request state and allowed transitions.
3. Persist request/decision immutably.
4. Use outbox for ApprovalRequested/Granted/Rejected facts.
5. Keep email and milestone updates asynchronous.
6. Test duplicate submission, stale version and unauthorized approver.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
