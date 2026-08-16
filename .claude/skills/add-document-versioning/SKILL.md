# Add Document Versioning

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Load document under tenant scope.
2. Create a new immutable version.
3. Never overwrite an approved/published version.
4. Update CurrentVersion pointer transactionally.
5. Preserve hashes/storage refs/history.
6. Ensure prior approvals remain bound to their exact versions.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
