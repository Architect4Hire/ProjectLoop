# Trace Distributed Request

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Start from correlation/trace ID.
2. Follow Angular->YARP->service spans.
3. Inspect SQL/Redis/Blob spans.
4. Follow outbox publication and Service Bus message metadata.
5. Follow consumer/inbox spans.
6. Identify missing propagation or sensitive logging.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
