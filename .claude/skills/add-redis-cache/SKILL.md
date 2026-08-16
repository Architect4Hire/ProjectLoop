# Add Redis Cache

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Prove caching is useful.
2. Choose owning domain and tenant-aware key.
3. Define TTL and invalidation.
4. Keep source of truth in SQL/service.
5. Handle miss/eviction safely.
6. Instrument hit/miss and test stale data behavior.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
