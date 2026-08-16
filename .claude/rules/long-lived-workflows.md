# Long-Lived Workflows Rule

Model durable process state explicitly. Start synchronously, persist intent/outbox, return 202/status URI where appropriate, progress asynchronously, expose status over HTTP.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
