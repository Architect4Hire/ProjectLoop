# Outbox/Inbox Rule

State+event publication uses a transactional outbox. All consumers are idempotent. Use an inbox when durable side effects must be protected from duplicate delivery.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
