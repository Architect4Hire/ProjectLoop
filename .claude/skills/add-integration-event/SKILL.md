# Add Integration Event

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Name a completed business fact in past tense.
2. Define owner, business key, tenant, event ID, version, occurred-at, correlation/causation.
3. Keep contract independent of EF/internal types.
4. Persist via outbox when coupled to state change.
5. Document in event catalog and add contract tests.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
