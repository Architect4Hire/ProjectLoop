# Add Document Publication

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Validate publishable state and authorization.
2. Freeze target version.
3. Persist publication state + outbox atomically.
4. Emit DocumentPublished.
5. Trigger approvals/notifications asynchronously where policy requires.
6. Test retry/idempotency and supersession behavior.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
