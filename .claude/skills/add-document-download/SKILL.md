# Add Document Download

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Resolve authenticated tenant/project membership.
2. Authorize document/version visibility before storage access.
3. Resolve blob key from SQL metadata.
4. Stream or issue short-lived access according to design.
5. Audit meaningful access as required.
6. Never expose permanent public blob URLs.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
