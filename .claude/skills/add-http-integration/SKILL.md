# Add HTTP Integration

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Confirm caller truly needs immediate result.
2. Define stable consumer-facing contract.
3. Use typed HttpClient and service discovery.
4. Set timeout/cancellation/tracing.
5. Retry only safe operations.
6. Avoid sync chains and document failure behavior.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
