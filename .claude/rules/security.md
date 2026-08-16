# Security Rule

Default deny. Validate input and file types, use managed identity/Key Vault in Azure, private blob access, least privilege, no secrets in source, and explicit tenant authorization.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
