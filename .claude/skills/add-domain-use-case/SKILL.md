# Add Domain Use Case

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Define invariant/state transition in Business.
2. Keep transport, EF, Redis and Service Bus concerns outside Business.
3. Have Facade orchestrate authorization/cache/use-case flow.
4. Have Data own transaction and repositories.
5. Emit integration fact decisions without publishing directly.
6. Add domain tests.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
