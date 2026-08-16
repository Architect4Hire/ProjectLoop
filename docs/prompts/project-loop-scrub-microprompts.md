# Project Loop SCRUB Microprompts

These prompts are deliberately seam-by-seam. Each prompt does one thing and points back to the controlling requirement/ADR.

## SCRUB-001 — Create solution skeleton
**Scope:** Create the .NET 10 solution folders/projects for YARP, Aspire AppHost, Angular web shell and proposed service placeholders only. Do not implement domain behavior.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-001`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-002 — Wire Aspire infrastructure
**Scope:** Add SQL, Redis, Service Bus and Blob emulator/configuration references to AppHost using service discovery.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-001`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-003 — Bootstrap Angular 22 shell
**Scope:** Create the Angular 22 application shell with lazy route placeholders and strict settings.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-002`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-004 — Install local design system
**Scope:** Place the approved design system under `src/web/design-system/` and make the shell consume it.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-002`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-005 — Create Identity persistence
**Scope:** Configure ASP.NET Core Identity persistence in the Identity/Tenant service database.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-006`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-006 — Add tenant entity
**Scope:** Add tenant/domain persistence and migrations without project membership yet.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-005`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-007 — Add tenant membership
**Scope:** Add user-to-tenant membership and server-side tenant resolution.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-005`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-008 — Add client invitation
**Scope:** Implement invitation creation/acceptance as an Identity/Tenant use case.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-001`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-009 — Add tenant authorization tests
**Scope:** Add cross-tenant positive/negative tests before feature services use tenant data.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-001`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-010 — Create Engagement service
**Scope:** Create thin API/Core persistence structure for projects only.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-001`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-011 — Add Project aggregate
**Scope:** Implement tenant-owned Project model and persistence.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-002`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-012 — Add project membership/authorization
**Scope:** Authorize project access through tenant/project membership.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-002`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-013 — Add project health endpoint
**Scope:** Add one read endpoint for project health using HTTP request/response.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-002`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-014 — Add milestone model
**Scope:** Add tenant/project-scoped milestone persistence and state rules.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-002`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-015 — Create Documents service
**Scope:** Create Documents API/Core/persistence and Blob infrastructure integration without upload behavior.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-007`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-016 — Add Document metadata model
**Scope:** Add Document metadata entity/domain model with tenant/project/type/status/visibility/current version.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-003`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-017 — Add DocumentVersion model
**Scope:** Add immutable version metadata, hash and blob object key model.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-009`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-018 — Add private blob abstraction
**Scope:** Implement service-owned Blob storage abstraction with private access only.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-007`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-019 — Add document upload authorization
**Scope:** Implement tenant/project authorization for upload intent before binary handling.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-004`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-020 — Add document binary upload
**Scope:** Store validated binary under opaque blob key and record hash.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-004`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-021 — Persist initial document version
**Scope:** Persist Document + first DocumentVersion in SQL after upload according to lifecycle design.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-005`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-022 — Add DocumentUploaded outbox
**Scope:** Persist DocumentUploaded in transactional outbox with document state.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-003`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-023 — Add document listing endpoint
**Scope:** Return tenant/project-scoped document metadata over HTTP.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-003`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-024 — Add authorized download
**Scope:** Authorize exact document/version before Blob stream/read access.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-007`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-025 — Add second-version workflow
**Scope:** Create a new immutable DocumentVersion without overwriting v1.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-009`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-026 — Add publication transition
**Scope:** Publish an eligible version and freeze it as immutable.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-006`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-027 — Add DocumentPublished outbox
**Scope:** Persist DocumentPublished with publication state in one transaction.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-003`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-028 — Create Approvals service
**Scope:** Create Approvals API/Core/persistence structure without workflow behavior.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-001`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-029 — Add ApprovalRequest model
**Scope:** Model request target, exact version, state, requested-by/at and tenant context.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-010`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-030 — Consume DocumentPublished
**Scope:** Create idempotent consumer that can create an approval request when policy requires.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-004`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-031 — Persist ApprovalRequested outbox
**Scope:** Create ApprovalRequested fact atomically with approval request creation.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-003`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-032 — Add approval read endpoint
**Scope:** Return approval + target version metadata required for client review.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-007`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-033 — Add approve command
**Scope:** Implement authenticated version-bound approve transition over HTTP.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-007`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-034 — Persist immutable decision
**Scope:** Write ApprovalDecision history without destructive update.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-008`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-035 — Publish ApprovalGranted
**Scope:** Persist ApprovalGranted via outbox with the decision transaction.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-003`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-036 — Add reject command
**Scope:** Implement authenticated version-bound reject transition and ApprovalRejected event.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-008`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-037 — Test stale-version approval
**Scope:** Prove approval for v3 never approves newly created v4.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-009`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-038 — Create Notification consumer
**Scope:** Consume ApprovalRequested and own email delivery attempts independently.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-009`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-039 — Make notification idempotent
**Scope:** Prevent duplicate message delivery from sending duplicate notifications beyond defined retry semantics.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-004`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-040 — Consume approval outcome in Engagement
**Scope:** Update milestone/project state idempotently from ApprovalGranted/Rejected where configured.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-010`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-041 — Create Audit service
**Scope:** Create append-oriented audit persistence and event consumers.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-011`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-042 — Audit document lifecycle
**Scope:** Consume document lifecycle facts into explicit audit records.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-011`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-043 — Audit approval lifecycle
**Scope:** Consume approval lifecycle facts into explicit audit records.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-011`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-044 — Build document list UI
**Scope:** Build Angular document list using only Project Loop design-system primitives.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-003`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-045 — Build document version UI
**Scope:** Show version history and immutable/published/approved state.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-005`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-046 — Build approval queue UI
**Scope:** Show outstanding client approvals with target resource/version.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-007`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-047 — Build approval review UI
**Scope:** Display document/version details and approve/reject actions with accessible confirmation.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-008`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-048 — Build dashboard project health
**Scope:** Render project health and milestones from HTTP read APIs.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-002`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-049 — Add commercial read model contract
**Scope:** Define invoice/hour/retainer portal contract without making Project Loop the source accounting system.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-011`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-050 — Add distributed tracing verification
**Scope:** Trace one approval workflow from browser HTTP through outbox/Service Bus consumers.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `LOOP-012`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-051 — Add Redis cache to one measured read
**Scope:** Add tenant-aware cache-aside behavior only to a justified expensive read.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `ADR-012`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.

## SCRUB-052 — Run architecture quality gate
**Scope:** Run tests/reviews for tenant isolation, authorization, HTTP-vs-event choices, document immutability and event idempotency.
**Constraints:** Follow `CLAUDE.md`, the matching `.claude/rules/` and `.claude/skills/`, and `CLAUDE.md`.
**Restrictions:** Change only this seam. Do not invent new service boundaries, shared databases, direct Service Bus publishing, public Blob access, or duplicate design-system components.
**Usage:** Preserve Controller/Trigger -> Facade -> Business -> Data -> Repository -> DbContext and owning-service boundaries.
**Behavior:** Add the smallest tests that prove the seam, including tenant/authorization/idempotency behavior where applicable.
