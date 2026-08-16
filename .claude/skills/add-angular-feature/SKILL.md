# Add Angular Feature

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Locate the owning route/feature boundary.
2. Inventory existing design-system primitives before writing markup.
3. Model typed API/view state using signals and typed forms as appropriate.
4. Implement lazy route/component boundaries and accessible states.
5. Add tests for loading, error, empty, success and authorization-sensitive behavior.
6. Run design-system and Angular review.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
