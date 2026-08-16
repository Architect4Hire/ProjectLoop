# Add Document Upload

Use this skill when implementing this concern in Project Loop. Read `CLAUDE.md` and the relevant `.claude/rules/` files first.

## Procedure
1. Authorize tenant/project before accepting content.
2. Validate size/type and create upload intent.
3. Store binary in private Blob Storage using opaque key.
4. Record metadata/hash/version in SQL.
5. Include quarantine/scanning state where configured.
6. Publish lifecycle event through outbox and test cross-tenant denial.

## Completion criteria
- Architecture invariants remain intact.
- Tests cover success and meaningful failure/security paths.
- Observability is preserved.
- Documentation/event catalog/ADR is updated when the change alters a contract or convention.
