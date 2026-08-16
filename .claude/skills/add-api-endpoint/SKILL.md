# Add API Endpoint

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Identify owning service and use case.
2. Define request/response contracts without exposing persistence models.
3. Enforce authentication, tenant context and authorization.
4. Controller delegates to Facade; preserve onion direction.
5. Use ProblemDetails and cancellation.
6. Add unit/integration tests and OpenAPI coverage.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
