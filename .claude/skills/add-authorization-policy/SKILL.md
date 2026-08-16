# Add Authorization Policy

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Define resource/action and required memberships/roles/state.
2. Implement server-side policy/handler.
3. Avoid data-fetch duplication where possible.
4. Add positive and negative tests including cross-tenant cases.
5. Expose UI affordances separately from enforcement.
6. Audit privileged operations.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
