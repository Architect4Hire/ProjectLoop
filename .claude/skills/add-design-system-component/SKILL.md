# Add Design System Component

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Prove an existing component/recipe cannot satisfy the use case.
2. Define semantic API, tokens and accessibility behavior.
3. Implement without feature-domain coupling.
4. Document variants/states and add examples/tests.
5. Replace duplicated feature styling with the new primitive only when appropriate.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
