# Solution Structure Rule

Keep deployable service hosts thin, service Core projects domain/application focused, Functions/consumers transport-focused, shared libraries mechanism-only, and AppHost orchestration-only.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
