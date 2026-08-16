# Add Document Type

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Define business meaning, allowed MIME/content policy, visibility defaults and lifecycle.
2. Add metadata classification without coupling to blob path.
3. Define approval eligibility if applicable.
4. Update authorization and retention rules.
5. Add UI presentation through design system.
6. Document type in requirements/design docs.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
