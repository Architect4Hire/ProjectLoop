# Project Loop — SCRUB Atomic Microstep Implementation Prompts v2

> Replacement for the original high-level Project Loop implementation sequence. This library is intentionally atomic: **one prompt = one primary change**.

> **Artifact identity:** This is the corrected atomic library. It contains 383 separately executable prompts (`000`–`382`). It replaces the earlier 52-prompt coarse implementation plan.

Project Loop is a multi-tenant consulting client portal centered on governed documents, version-bound approvals, and a deliberate mixture of synchronous HTTP and event-driven integration.

## Why this sequence is intentionally granular

Every implementation prompt:

- advances one narrow seam only;
- identifies the Project Loop requirement IDs it advances;
- requires Claude to read the canonical requirement text before coding;
- names the architectural rules and skills that govern the seam;
- explicitly forbids adjacent work;
- uses the smallest meaningful verification for that seam;
- ends after reporting verification results;
- never treats convenience as permission to implement the next layer.

A microstep may touch more than one file **only when those files are indivisible parts of the same seam**. An interface plus its focused implementation is one seam. An entity plus its EF mapping plus repository plus API endpoint is **not** one seam.

## Architecture this sequence preserves

```text
Browser / Angular 22
        |
        v
YARP Gateway                       # only browser-facing backend edge
        |
        +--------------------------+
        |                          |
        v                          v
ProjectLoop.<Service> API      typed HTTP to owning service
        |
        v
Controller -> Facade -> Business -> Data -> Repository -> DbContext

ProjectLoop.<Service>.Functions
        |                         |
ServiceBusTrigger            TimerTrigger
        |                         |
        v                         v
Facade -> Business...        Outbox Relay -> Azure Service Bus
```

Per bounded service:

```text
ProjectLoop.<Service>/
ProjectLoop.<Service>.Core/
ProjectLoop.<Service>.Functions/
```

Infrastructure baseline:

- .NET 10
- Angular 22
- local Project Loop design system under `src/web/design-system/`
- YARP gateway
- .NET Aspire
- ASP.NET Core Identity
- Microsoft SQL Server / Azure SQL
- one database per bounded service
- Azure Blob Storage for document binaries
- Azure Service Bus
- Redis
- Azure Functions isolated worker / Flex Consumption production target
- OpenTelemetry
- Azure Monitor / Application Insights

## Canonical domain boundaries

The initial approved boundaries are:

- **Identity/Tenant** — ASP.NET Core Identity, tenants, memberships, invitations and authorization context.
- **Engagement** — projects, project membership, milestones, meetings, decisions and project health.
- **Documents** — document metadata, immutable versions, Blob references, publication and client visibility.
- **Approvals** — approval requests, exact-version targets, decisions and immutable approval history.
- **Notifications** — asynchronous notification delivery and delivery history.
- **Audit** — append-only business/security audit history.
- **Commercial Read Model** — externally sourced invoice/hour/retainer summary data exposed to the portal.

Do not add a separate Dashboard bounded service unless a later ADR explicitly introduces one.

## Integration decision rule

Use **HTTP** when the caller requires the answer before it can continue. Use **events** for completed business facts that require propagation, fan-out, independent retry, temporal decoupling, or long-running work.

State change plus required event publication uses a **transactional outbox**. All consumers are idempotent. Use a transactional inbox when duplicate processing could incorrectly mutate durable state or repeat a non-idempotent business side effect.

## Document/approval invariants

1. SQL owns document metadata and lifecycle; Blob Storage owns binary content.
2. A Blob URL is never an authorization boundary.
3. Published or approved document versions are immutable.
4. Replacing content creates a new `DocumentVersion`.
5. An approval targets an exact immutable version or exact non-document target identity.
6. Approval of v3 never implies approval of v4.
7. Approval history is append-only business evidence.
8. Operational logs are not the authoritative audit store.

## Mandatory execution contract for every prompt

Before changing code, Claude must:

1. Read root `CLAUDE.md`.
2. Read the prompt's `REQUIREMENTS` callout and both canonical requirement files.
3. Read every `.claude/rules/*.md` named by `USAGE`.
4. Use the matching `.claude/skills/*/SKILL.md` named by `USAGE` when present.
5. Inspect existing code before deciding an artifact is missing.
6. State the **single primary change** before editing.
7. List the exact files expected to change.
8. Stop without editing if the step requires an unresolved architecture, security, persistence-ownership, public-contract, storage, or deployment decision.
9. Make only the primary change.
10. Run the smallest meaningful verification.
11. Report files changed, verification command(s), pass/fail, requirement IDs advanced, and the next prompt number.
12. **STOP. Do not continue into the next prompt.**

## Compact requirement callout contract

Every prompt contains:

```text
REQUIREMENTS:
  TRACEABILITY: <LOOP requirement IDs or architecture-governance marker>
  REQUIREMENT LINKS: <canonical requirements + matrix>
  REQUIREMENT INTENT: <short statement of applicable behavior>
  SOURCE OF TRUTH: Read the linked requirements before coding; stop on drift.

SCOPE:       one concrete action and exact boundary
CONSTRAINT:  rules that must remain true while making that action
RESTRICTION: adjacent work that is forbidden
USAGE:       exact rules / skills / verification tools to use
BEHAVIOR:    inspect -> change one thing -> verify -> report -> STOP
```

Canonical requirement links used below:

- [Project Loop requirements](../requirements/requirements.md)
- [Project Loop requirements matrix](../requirements/requirements-matrix.md)

---

# Part 0 — Repository discovery and architecture gates

## Prompt 000 — Inventory the repository without changing it

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Inspect the current Project Loop repository and report what exists versus what is only documented.

CONSTRAINT: Read `CLAUDE.md`, all `.claude` rules/skills/agents, docs, solution/project/package files, and git status.

RESTRICTION: Do not create, edit, rename, delete, restore, install, scaffold, migrate, or format anything.

USAGE: Read-only repository tools only.

BEHAVIOR: Report repository tree, buildable projects, missing expected artifacts, unresolved decisions and current verification commands. Prove `git status` is unchanged. STOP.
```

## Prompt 001 — Bind the canonical requirements sources

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Locate the authoritative Project Loop requirements and requirement matrix only.

CONSTRAINT: The expected canonical files are `docs/requirements/requirements.md` and `docs/requirements/requirements-matrix.md`; verify rather than assume.

RESTRICTION: Do not modify requirements or infer new requirements.

USAGE: Read-only search.

BEHAVIOR: Report the exact paths, requirement IDs found, missing/duplicate IDs and any drift between the two files. STOP.
```

## Prompt 002 — Verify the approved bounded-context catalog

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Compare the documented service catalog with the approved Project Loop boundaries only.

CONSTRAINT: Identity/Tenant, Engagement, Documents, Approvals, Notifications, Audit and Commercial Read Model are the approved starting boundaries.

RESTRICTION: Do not scaffold projects or add/remove boundaries.

USAGE: Read `CLAUDE.md`, `.claude/rules/domain-boundaries.md`, `docs/adr/ADR-001-service-boundaries.md`.

BEHAVIOR: Report agreement or drift. If drift exists, stop for documentation correction; otherwise STOP.
```

## Prompt 003 — Verify HTTP-versus-event architecture decision

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Check that the repository documentation consistently expresses the HTTP-versus-event decision rule.

CONSTRAINT: HTTP is for immediate request/response; events are for completed facts requiring temporal decoupling, retries, fan-out or long-running propagation.

RESTRICTION: Do not change code or broker topology.

USAGE: Read integration ADRs/rules/design docs only.

BEHAVIOR: Report inconsistencies by file and line/section. STOP.
```

## Prompt 004 — Verify the document storage decision

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Check that SQL-metadata plus Blob-binary ownership is consistent across architecture documents.

CONSTRAINT: Documents owns metadata/lifecycle; Blob Storage owns binaries; Blob addresses are not auth boundaries.

RESTRICTION: Do not provision Blob containers or write storage code.

USAGE: Read document/storage ADRs and rules.

BEHAVIOR: Report drift only. STOP.
```

## Prompt 005 — Verify the approval version-binding decision

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Check that every approval design binds document approvals to an exact immutable version.

CONSTRAINT: Approval of one version never approves a later version.

RESTRICTION: Do not implement approval code.

USAGE: Read approval/versioning ADRs, requirements and rules.

BEHAVIOR: Report any language that ambiguously targets a document without version identity. STOP.
```

# Part 1 — Solution and local development skeleton

## Prompt 006 — Create the root .NET solution file

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the root `.sln`/solution container if it does not already exist.

CONSTRAINT: .NET 10 naming must use `ProjectLoop` namespace conventions.

RESTRICTION: Do not create any projects yet.

USAGE: Use repository naming rules; run solution-list command.

BEHAVIOR: Verify the solution opens/lists successfully and contains no unintended projects. STOP.
```

## Prompt 007 — Create Aspire AppHost project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.AppHost` with the minimum .NET Aspire AppHost project files.

CONSTRAINT: Use .NET 10 and the existing project naming/layout rules.

RESTRICTION: Do not add domain services, infrastructure resources, routes, auth or business code.

USAGE: Read `.claude/rules/solution-structure.md`, `.claude/rules/aspire.md`; use `add-aspire-resource` only when relevant.

BEHAVIOR: Restore/build only the affected project or solution slice; report and STOP.
```

## Prompt 008 — Create ServiceDefaults project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the shared Aspire `ProjectLoop.ServiceDefaults` project.

CONSTRAINT: Use .NET 10 and the existing project naming/layout rules.

RESTRICTION: Do not add domain services, infrastructure resources, routes, auth or business code.

USAGE: Read `.claude/rules/solution-structure.md`, `.claude/rules/aspire.md`; use `add-aspire-resource` only when relevant.

BEHAVIOR: Restore/build only the affected project or solution slice; report and STOP.
```

## Prompt 009 — Reference ServiceDefaults from AppHost

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the AppHost -> ServiceDefaults project reference and required bootstrap call.

CONSTRAINT: Use .NET 10 and the existing project naming/layout rules.

RESTRICTION: Do not add domain services, infrastructure resources, routes, auth or business code.

USAGE: Read `.claude/rules/solution-structure.md`, `.claude/rules/aspire.md`; use `add-aspire-resource` only when relevant.

BEHAVIOR: Restore/build only the affected project or solution slice; report and STOP.
```

## Prompt 010 — Create YARP Gateway project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the `ProjectLoop.Gateway` ASP.NET Core project without routes.

CONSTRAINT: Use .NET 10 and the existing project naming/layout rules.

RESTRICTION: Do not add domain services, infrastructure resources, routes, auth or business code.

USAGE: Read `.claude/rules/solution-structure.md`, `.claude/rules/aspire.md`; use `add-aspire-resource` only when relevant.

BEHAVIOR: Restore/build only the affected project or solution slice; report and STOP.
```

## Prompt 011 — Create Identity API host project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Identity` ASP.NET Core API host in the approved service folder.

CONSTRAINT: Use .NET 10; host remains thin and must reference no other service persistence project.

RESTRICTION: Do not create `Identity.Core`, Functions, entities, DbContext, controllers or routes.

USAGE: Read solution-structure/architecture/dotnet/domain-boundaries rules.

BEHAVIOR: Build the new API project only. STOP.
```

## Prompt 012 — Create Identity.Core project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Identity.Core` class library.

CONSTRAINT: Core will later contain Facade/Business/Data/Repository contracts/implementations according to Project Loop layering.

RESTRICTION: Do not add entities, EF packages, persistence, business behavior or API references.

USAGE: Read solution-structure/architecture/dotnet rules.

BEHAVIOR: Build the Core project only. STOP.
```

## Prompt 013 — Create Identity.Functions project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Identity.Functions` isolated-worker Functions project.

CONSTRAINT: Functions will later host Service Bus/timer triggers and call the same Core facade boundary.

RESTRICTION: Do not add triggers, Service Bus packages beyond template necessities, handlers or domain behavior.

USAGE: Read dotnet/messaging/solution-structure rules.

BEHAVIOR: Build the Functions project only. STOP.
```

## Prompt 014 — Create Engagement API host project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Engagement` ASP.NET Core API host in the approved service folder.

CONSTRAINT: Use .NET 10; host remains thin and must reference no other service persistence project.

RESTRICTION: Do not create `Engagement.Core`, Functions, entities, DbContext, controllers or routes.

USAGE: Read solution-structure/architecture/dotnet/domain-boundaries rules.

BEHAVIOR: Build the new API project only. STOP.
```

## Prompt 015 — Create Engagement.Core project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Engagement.Core` class library.

CONSTRAINT: Core will later contain Facade/Business/Data/Repository contracts/implementations according to Project Loop layering.

RESTRICTION: Do not add entities, EF packages, persistence, business behavior or API references.

USAGE: Read solution-structure/architecture/dotnet rules.

BEHAVIOR: Build the Core project only. STOP.
```

## Prompt 016 — Create Engagement.Functions project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Engagement.Functions` isolated-worker Functions project.

CONSTRAINT: Functions will later host Service Bus/timer triggers and call the same Core facade boundary.

RESTRICTION: Do not add triggers, Service Bus packages beyond template necessities, handlers or domain behavior.

USAGE: Read dotnet/messaging/solution-structure rules.

BEHAVIOR: Build the Functions project only. STOP.
```

## Prompt 017 — Create Documents API host project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Documents` ASP.NET Core API host in the approved service folder.

CONSTRAINT: Use .NET 10; host remains thin and must reference no other service persistence project.

RESTRICTION: Do not create `Documents.Core`, Functions, entities, DbContext, controllers or routes.

USAGE: Read solution-structure/architecture/dotnet/domain-boundaries rules.

BEHAVIOR: Build the new API project only. STOP.
```

## Prompt 018 — Create Documents.Core project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Documents.Core` class library.

CONSTRAINT: Core will later contain Facade/Business/Data/Repository contracts/implementations according to Project Loop layering.

RESTRICTION: Do not add entities, EF packages, persistence, business behavior or API references.

USAGE: Read solution-structure/architecture/dotnet rules.

BEHAVIOR: Build the Core project only. STOP.
```

## Prompt 019 — Create Documents.Functions project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Documents.Functions` isolated-worker Functions project.

CONSTRAINT: Functions will later host Service Bus/timer triggers and call the same Core facade boundary.

RESTRICTION: Do not add triggers, Service Bus packages beyond template necessities, handlers or domain behavior.

USAGE: Read dotnet/messaging/solution-structure rules.

BEHAVIOR: Build the Functions project only. STOP.
```

## Prompt 020 — Create Approvals API host project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Approvals` ASP.NET Core API host in the approved service folder.

CONSTRAINT: Use .NET 10; host remains thin and must reference no other service persistence project.

RESTRICTION: Do not create `Approvals.Core`, Functions, entities, DbContext, controllers or routes.

USAGE: Read solution-structure/architecture/dotnet/domain-boundaries rules.

BEHAVIOR: Build the new API project only. STOP.
```

## Prompt 021 — Create Approvals.Core project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Approvals.Core` class library.

CONSTRAINT: Core will later contain Facade/Business/Data/Repository contracts/implementations according to Project Loop layering.

RESTRICTION: Do not add entities, EF packages, persistence, business behavior or API references.

USAGE: Read solution-structure/architecture/dotnet rules.

BEHAVIOR: Build the Core project only. STOP.
```

## Prompt 022 — Create Approvals.Functions project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Approvals.Functions` isolated-worker Functions project.

CONSTRAINT: Functions will later host Service Bus/timer triggers and call the same Core facade boundary.

RESTRICTION: Do not add triggers, Service Bus packages beyond template necessities, handlers or domain behavior.

USAGE: Read dotnet/messaging/solution-structure rules.

BEHAVIOR: Build the Functions project only. STOP.
```

## Prompt 023 — Create Notifications API host project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Notifications` ASP.NET Core API host in the approved service folder.

CONSTRAINT: Use .NET 10; host remains thin and must reference no other service persistence project.

RESTRICTION: Do not create `Notifications.Core`, Functions, entities, DbContext, controllers or routes.

USAGE: Read solution-structure/architecture/dotnet/domain-boundaries rules.

BEHAVIOR: Build the new API project only. STOP.
```

## Prompt 024 — Create Notifications.Core project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Notifications.Core` class library.

CONSTRAINT: Core will later contain Facade/Business/Data/Repository contracts/implementations according to Project Loop layering.

RESTRICTION: Do not add entities, EF packages, persistence, business behavior or API references.

USAGE: Read solution-structure/architecture/dotnet rules.

BEHAVIOR: Build the Core project only. STOP.
```

## Prompt 025 — Create Notifications.Functions project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Notifications.Functions` isolated-worker Functions project.

CONSTRAINT: Functions will later host Service Bus/timer triggers and call the same Core facade boundary.

RESTRICTION: Do not add triggers, Service Bus packages beyond template necessities, handlers or domain behavior.

USAGE: Read dotnet/messaging/solution-structure rules.

BEHAVIOR: Build the Functions project only. STOP.
```

## Prompt 026 — Create Audit API host project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Audit` ASP.NET Core API host in the approved service folder.

CONSTRAINT: Use .NET 10; host remains thin and must reference no other service persistence project.

RESTRICTION: Do not create `Audit.Core`, Functions, entities, DbContext, controllers or routes.

USAGE: Read solution-structure/architecture/dotnet/domain-boundaries rules.

BEHAVIOR: Build the new API project only. STOP.
```

## Prompt 027 — Create Audit.Core project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Audit.Core` class library.

CONSTRAINT: Core will later contain Facade/Business/Data/Repository contracts/implementations according to Project Loop layering.

RESTRICTION: Do not add entities, EF packages, persistence, business behavior or API references.

USAGE: Read solution-structure/architecture/dotnet rules.

BEHAVIOR: Build the Core project only. STOP.
```

## Prompt 028 — Create Audit.Functions project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Audit.Functions` isolated-worker Functions project.

CONSTRAINT: Functions will later host Service Bus/timer triggers and call the same Core facade boundary.

RESTRICTION: Do not add triggers, Service Bus packages beyond template necessities, handlers or domain behavior.

USAGE: Read dotnet/messaging/solution-structure rules.

BEHAVIOR: Build the Functions project only. STOP.
```

## Prompt 029 — Create Commercial API host project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Commercial` ASP.NET Core API host in the approved service folder.

CONSTRAINT: Use .NET 10; host remains thin and must reference no other service persistence project.

RESTRICTION: Do not create `Commercial.Core`, Functions, entities, DbContext, controllers or routes.

USAGE: Read solution-structure/architecture/dotnet/domain-boundaries rules.

BEHAVIOR: Build the new API project only. STOP.
```

## Prompt 030 — Create Commercial.Core project

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only `ProjectLoop.Commercial.Core` class library.

CONSTRAINT: Core will later contain Facade/Business/Data/Repository contracts/implementations according to Project Loop layering.

RESTRICTION: Do not add entities, EF packages, persistence, business behavior or API references.

USAGE: Read solution-structure/architecture/dotnet rules.

BEHAVIOR: Build the Core project only. STOP.
```

## Prompt 031 — Reference Identity.Core from Identity API

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Identity` to `ProjectLoop.Identity.Core`.

CONSTRAINT: Preserve one-way host -> Core dependency.

RESTRICTION: Do not register services or add behavior.

USAGE: Read architecture/solution-structure rules.

BEHAVIOR: Build the affected API project. STOP.
```

## Prompt 032 — Reference Identity.Core from Identity.Functions

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Identity.Functions` to `ProjectLoop.Identity.Core`.

CONSTRAINT: Preserve trigger -> facade use of Core; Functions must not reference API host.

RESTRICTION: Do not add triggers or DI registration.

USAGE: Read architecture/functions rules.

BEHAVIOR: Build the affected Functions project. STOP.
```

## Prompt 033 — Reference Engagement.Core from Engagement API

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Engagement` to `ProjectLoop.Engagement.Core`.

CONSTRAINT: Preserve one-way host -> Core dependency.

RESTRICTION: Do not register services or add behavior.

USAGE: Read architecture/solution-structure rules.

BEHAVIOR: Build the affected API project. STOP.
```

## Prompt 034 — Reference Engagement.Core from Engagement.Functions

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Engagement.Functions` to `ProjectLoop.Engagement.Core`.

CONSTRAINT: Preserve trigger -> facade use of Core; Functions must not reference API host.

RESTRICTION: Do not add triggers or DI registration.

USAGE: Read architecture/functions rules.

BEHAVIOR: Build the affected Functions project. STOP.
```

## Prompt 035 — Reference Documents.Core from Documents API

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Documents` to `ProjectLoop.Documents.Core`.

CONSTRAINT: Preserve one-way host -> Core dependency.

RESTRICTION: Do not register services or add behavior.

USAGE: Read architecture/solution-structure rules.

BEHAVIOR: Build the affected API project. STOP.
```

## Prompt 036 — Reference Documents.Core from Documents.Functions

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Documents.Functions` to `ProjectLoop.Documents.Core`.

CONSTRAINT: Preserve trigger -> facade use of Core; Functions must not reference API host.

RESTRICTION: Do not add triggers or DI registration.

USAGE: Read architecture/functions rules.

BEHAVIOR: Build the affected Functions project. STOP.
```

## Prompt 037 — Reference Approvals.Core from Approvals API

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Approvals` to `ProjectLoop.Approvals.Core`.

CONSTRAINT: Preserve one-way host -> Core dependency.

RESTRICTION: Do not register services or add behavior.

USAGE: Read architecture/solution-structure rules.

BEHAVIOR: Build the affected API project. STOP.
```

## Prompt 038 — Reference Approvals.Core from Approvals.Functions

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Approvals.Functions` to `ProjectLoop.Approvals.Core`.

CONSTRAINT: Preserve trigger -> facade use of Core; Functions must not reference API host.

RESTRICTION: Do not add triggers or DI registration.

USAGE: Read architecture/functions rules.

BEHAVIOR: Build the affected Functions project. STOP.
```

## Prompt 039 — Reference Notifications.Core from Notifications API

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Notifications` to `ProjectLoop.Notifications.Core`.

CONSTRAINT: Preserve one-way host -> Core dependency.

RESTRICTION: Do not register services or add behavior.

USAGE: Read architecture/solution-structure rules.

BEHAVIOR: Build the affected API project. STOP.
```

## Prompt 040 — Reference Notifications.Core from Notifications.Functions

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Notifications.Functions` to `ProjectLoop.Notifications.Core`.

CONSTRAINT: Preserve trigger -> facade use of Core; Functions must not reference API host.

RESTRICTION: Do not add triggers or DI registration.

USAGE: Read architecture/functions rules.

BEHAVIOR: Build the affected Functions project. STOP.
```

## Prompt 041 — Reference Audit.Core from Audit API

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Audit` to `ProjectLoop.Audit.Core`.

CONSTRAINT: Preserve one-way host -> Core dependency.

RESTRICTION: Do not register services or add behavior.

USAGE: Read architecture/solution-structure rules.

BEHAVIOR: Build the affected API project. STOP.
```

## Prompt 042 — Reference Audit.Core from Audit.Functions

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Audit.Functions` to `ProjectLoop.Audit.Core`.

CONSTRAINT: Preserve trigger -> facade use of Core; Functions must not reference API host.

RESTRICTION: Do not add triggers or DI registration.

USAGE: Read architecture/functions rules.

BEHAVIOR: Build the affected Functions project. STOP.
```

## Prompt 043 — Reference Commercial.Core from Commercial API

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the project reference from `ProjectLoop.Commercial` to `ProjectLoop.Commercial.Core`.

CONSTRAINT: Preserve one-way host -> Core dependency.

RESTRICTION: Do not register services or add behavior.

USAGE: Read architecture/solution-structure rules.

BEHAVIOR: Build the affected API project. STOP.
```

## Prompt 044 — Add SQL Server resource to Aspire

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the SQL Server/Azure SQL-compatible local resource to AppHost.

CONSTRAINT: Use Aspire-native configuration/service discovery; no secrets in source.

RESTRICTION: Do not add service databases or references yet.

USAGE: Read aspire/security rules and `add-aspire-resource` skill.

BEHAVIOR: Build AppHost and inspect generated resource model if test support exists. STOP.
```

## Prompt 045 — Add Redis resource to Aspire

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the Redis resource to AppHost.

CONSTRAINT: Use Aspire-native configuration/service discovery; no secrets in source.

RESTRICTION: Do not add cache consumers.

USAGE: Read aspire/security rules and `add-aspire-resource` skill.

BEHAVIOR: Build AppHost and inspect generated resource model if test support exists. STOP.
```

## Prompt 046 — Add Azure Service Bus resource to Aspire

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the Azure Service Bus emulator/resource definition supported by the chosen Aspire baseline.

CONSTRAINT: Use Aspire-native configuration/service discovery; no secrets in source.

RESTRICTION: Do not define topics, subscriptions or consumers.

USAGE: Read aspire/security rules and `add-aspire-resource` skill.

BEHAVIOR: Build AppHost and inspect generated resource model if test support exists. STOP.
```

## Prompt 047 — Add Blob Storage resource to Aspire

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the Azure Storage/Blob local resource to AppHost.

CONSTRAINT: Use Aspire-native configuration/service discovery; no secrets in source.

RESTRICTION: Do not create document containers or storage code.

USAGE: Read aspire/security rules and `add-aspire-resource` skill.

BEHAVIOR: Build AppHost and inspect generated resource model if test support exists. STOP.
```

# Part 2 — Angular 22 shell and design-system integration

## Prompt 048 — Create the Angular 22 workspace

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the Angular 22 application workspace under `src/web` with strict TypeScript and routing enabled.

CONSTRAINT: Use Angular 22 standalone APIs and strict compiler settings.

RESTRICTION: Do not create feature pages, design-system copies, auth, API clients or business UI.

USAGE: Read angular/design-system rules and `add-angular-feature` skill.

BEHAVIOR: Run Angular build only. STOP.
```

## Prompt 049 — Create the design-system source directory contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the expected `src/web/design-system/` directory/README contract for the owner-supplied design-system drop-in.

CONSTRAINT: The directory is authoritative local source and is not synthesized by Claude.

RESTRICTION: Do not fetch, generate, copy or modify the actual design system.

USAGE: Read design-system rule.

BEHAVIOR: Verify directory/README only and STOP.
```

## Prompt 050 — Wire the dropped design system into Angular

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: After the owner-supplied design system exists, add only the Angular/TypeScript path/import configuration needed to consume it.

CONSTRAINT: Preserve the design-system source tree unchanged.

RESTRICTION: Do not build application pages or recreate recipes.

USAGE: Read design-system/angular rules and `add-design-system-component` skill.

BEHAVIOR: Build Angular and prove one compile-time import resolves. STOP.
```

## Prompt 051 — Render the Project Loop application shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the authenticated application-shell component using the design-system shell/layout primitives.

CONSTRAINT: Use existing design-system tokens/primitives; page outlet only, no feature content.

RESTRICTION: Do not create dashboard widgets, navigation authorization, login or project UI.

USAGE: Read angular/design-system rules.

BEHAVIOR: Run shell component test/build. STOP.
```

## Prompt 052 — Add top-level lazy route placeholders

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only lazy route placeholders for dashboard, projects, documents, approvals and account.

CONSTRAINT: Use standalone lazy loading; placeholders contain no business behavior.

RESTRICTION: Do not implement pages, resolvers, guards or API calls.

USAGE: Read angular rule.

BEHAVIOR: Run router configuration/unit test. STOP.
```

# Part 3 — Identity, tenant membership and authorization context

## Prompt 053 — Add ASP.NET Core Identity packages to Identity service

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the approved ASP.NET Core Identity/EF Core package references to Identity projects.

CONSTRAINT: Use .NET 10-compatible packages and SQL Server provider.

RESTRICTION: Do not create user types, DbContext, migrations or endpoints.

USAGE: Read identity/database rules and `add-sql-persistence` skill.

BEHAVIOR: Restore/build Identity projects. STOP.
```

## Prompt 054 — Create ProjectLoopUser identity type

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the application Identity user type derived from the chosen ASP.NET Core Identity base type.

CONSTRAINT: Keep business tenant/project membership out of the Identity user row.

RESTRICTION: Do not add tenant navigation, roles, DbContext or endpoints.

USAGE: Read identity/multi-tenancy rules.

BEHAVIOR: Run focused compile/unit test. STOP.
```

## Prompt 055 — Create IdentityDbContext

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the Identity EF DbContext type using `ProjectLoopUser`.

CONSTRAINT: Identity DB is owned exclusively by Identity/Tenant boundary.

RESTRICTION: Do not add Tenant entities or migrations yet.

USAGE: Read identity/database rules.

BEHAVIOR: Compile Core only. STOP.
```

## Prompt 056 — Configure IdentityDbContext SQL mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only SQL Server-compatible Identity model configuration required by the DbContext.

CONSTRAINT: Use UTC-safe conventions and service-owned schema rules.

RESTRICTION: Do not create migration or seed users.

USAGE: Read database/identity rules.

BEHAVIOR: Run model-build focused test. STOP.
```

## Prompt 057 — Register IdentityDbContext with DI

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only IdentityDbContext registration to the Identity API host.

CONSTRAINT: Connection comes from configuration/Aspire reference; no literal secrets.

RESTRICTION: Do not configure login endpoints or auth cookies/tokens.

USAGE: Read identity/aspire/security rules.

BEHAVIOR: Build API and run DI resolution test if available. STOP.
```

## Prompt 058 — Create initial Identity migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the initial Identity database migration for currently modeled Identity tables.

CONSTRAINT: Migration must be deterministic and scoped to Identity DB.

RESTRICTION: Do not apply it to shared/other service databases or add tenant tables yet.

USAGE: Read database rule.

BEHAVIOR: Generate migration and inspect it; do not add unrelated schema. STOP.
```

## Prompt 059 — Create Tenant entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the Tenant domain/persistence entity with stable ID, name, status and timestamps required by current requirements.

CONSTRAINT: Tenant is owned by Identity/Tenant boundary.

RESTRICTION: Do not create membership, EF mapping, repository or endpoint.

USAGE: Read multi-tenancy/database rules.

BEHAVIOR: Run focused entity unit tests only. STOP.
```

## Prompt 060 — Add Tenant EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF Core mapping/configuration for Tenant.

CONSTRAINT: Define keys, lengths, indexes/concurrency according to database rules.

RESTRICTION: Do not add repository or migration in this prompt.

USAGE: Read database/multi-tenancy rules.

BEHAVIOR: Run EF model test. STOP.
```

## Prompt 061 — Add Tenant DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the Tenant DbSet/model registration to IdentityDbContext.

CONSTRAINT: No cross-service entities.

RESTRICTION: Do not generate migration.

USAGE: Read database rule.

BEHAVIOR: Build Core/model test. STOP.
```

## Prompt 062 — Create Tenant schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the migration produced by adding Tenant to IdentityDbContext.

CONSTRAINT: Migration contains only Tenant-related schema delta.

RESTRICTION: Do not add membership schema or seed data.

USAGE: Read database rule.

BEHAVIOR: Inspect migration and run migration test against disposable SQL if available. STOP.
```

## Prompt 063 — Create TenantMembership entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only user-to-tenant membership entity with user ID, tenant ID, role/access state and timestamps.

CONSTRAINT: Membership is server-owned authorization data; reference user by stable Identity key.

RESTRICTION: Do not add EF mapping, resolver or endpoints.

USAGE: Read identity/multi-tenancy/authorization rules.

BEHAVIOR: Run entity tests only. STOP.
```

## Prompt 064 — Add TenantMembership EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes/constraints for TenantMembership.

CONSTRAINT: Prevent duplicate active membership tuple according to chosen data rule.

RESTRICTION: Do not add DbSet/migration yet.

USAGE: Read database/multi-tenancy rules.

BEHAVIOR: Run model configuration test. STOP.
```

## Prompt 065 — Add TenantMembership DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only TenantMembership in IdentityDbContext.

CONSTRAINT: No other model changes.

RESTRICTION: Do not generate migration.

USAGE: Read database rule.

BEHAVIOR: Build/model test. STOP.
```

## Prompt 066 — Create TenantMembership schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only the membership schema migration.

CONSTRAINT: Migration must contain the expected FK/index/constraint delta only.

RESTRICTION: Do not add invitation schema.

USAGE: Read database rule.

BEHAVIOR: Inspect and integration-test migration. STOP.
```

## Prompt 067 — Define ITenantContext contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the server-side tenant-context abstraction exposed to authorized application seams.

CONSTRAINT: Context represents authenticated/authorized tenant identity; request payload values do not establish it.

RESTRICTION: Do not implement resolver or middleware.

USAGE: Read multi-tenancy/authorization rules and `add-tenant-aware-feature` skill.

BEHAVIOR: Compile contract only. STOP.
```

## Prompt 068 — Implement tenant-context resolver from authenticated user membership

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only the component that resolves an allowed tenant context from authenticated identity plus persisted membership.

CONSTRAINT: Reject absent/disabled/non-member tenant selection; no trust in arbitrary tenant claim/request value unless validated against membership.

RESTRICTION: Do not add middleware, endpoints or project authorization.

USAGE: Read identity/multi-tenancy/authorization rules.

BEHAVIOR: Focused positive/negative unit tests. STOP.
```

## Prompt 069 — Add tenant-context request middleware

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only middleware/endpoint plumbing that establishes ITenantContext using the resolver for authenticated requests.

CONSTRAINT: Return correct unauthorized/forbidden semantics; never silently fall back to another tenant.

RESTRICTION: Do not add project-level policies or feature endpoints.

USAGE: Read api-design/identity/authorization rules.

BEHAVIOR: Add focused middleware tests. STOP.
```

## Prompt 070 — Add cross-tenant denial integration test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add one integration test proving a user authorized for Tenant A cannot establish Tenant B context.

CONSTRAINT: Use real Identity/Tenant persistence test fixture where practical.

RESTRICTION: Do not add new production behavior unless the test exposes a defect in the immediately preceding tenant-context seam.

USAGE: Read testing/multi-tenancy rules.

BEHAVIOR: Run the single test and report. STOP.
```

## Prompt 071 — Create ClientInvitation entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the invitation persistence entity containing tenant, email, token hash/reference, expiry, state, inviter and timestamps.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not map, persist, send email or accept invitations.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 072 — Add ClientInvitation EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes for ClientInvitation.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not add DbSet or migration.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 073 — Add ClientInvitation DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only ClientInvitation with IdentityDbContext.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not generate migration.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 074 — Create ClientInvitation schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only the invitation schema migration.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not implement invitation behavior.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 075 — Define invitation creation request contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the API request DTO/contract for creating an invitation.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not add controller action or business logic.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 076 — Implement invitation token generator

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only secure invitation token generation/hash behavior.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not persist or send invitations.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 077 — Implement invitation creation repository operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository persistence needed to create an invitation record.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not expose facade/controller or publish events.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 078 — Implement invitation creation business operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only business-layer creation logic using token generator and repository.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not add facade/controller/email.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 079 — Implement invitation creation facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only facade validation/authorization orchestration for invitation creation.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not add controller route or notification.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 080 — Add invitation creation HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the controller action that exposes the existing invitation facade operation.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not implement acceptance or email delivery.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 081 — Add invitation creation HTTP contract test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the focused authorized/forbidden HTTP contract tests for invitation creation.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not implement new invitation behavior beyond fixing defects in this seam.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 082 — Define invitation acceptance request contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the API contract for accepting an invitation token.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not add controller/business behavior.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 083 — Implement invitation lookup-by-token repository operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only secure lookup of a valid invitation by token/hash.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not mutate invitation or membership.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 084 — Implement invitation acceptance business operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only business logic that validates invitation state/expiry and creates membership atomically as designed.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not add facade/controller/email or unrelated account creation.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 085 — Implement invitation acceptance facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only facade orchestration for the existing acceptance business operation.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not add route or notification.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 086 — Add invitation acceptance HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the acceptance controller action.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not add UI.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

## Prompt 087 — Add invitation acceptance integration test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only integration tests for valid, expired, reused and wrong-email/wrong-user invitation semantics required by the current design.

CONSTRAINT: Invitation handling stays in Identity/Tenant and must preserve tenant isolation and least privilege.

RESTRICTION: Do not add unrelated login tests.

USAGE: Read identity/multi-tenancy/authorization/database rules; use tenant-aware feature skill where relevant.

BEHAVIOR: Run only the smallest focused unit/integration/HTTP verification for this seam. Report and STOP.
```

# Part 4 — Engagement project and milestone seams

## Prompt 088 — Create Project entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the tenant-owned Project entity with identifiers and basic status/health fields required for portal display.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add EF mapping, milestones, repository or API.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 089 — Add Project EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes/concurrency configuration for Project.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add DbSet or migration.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 090 — Create EngagementDbContext

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the Engagement service DbContext shell.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not register Project DbSet yet.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 091 — Add Project DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only Project with EngagementDbContext.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not generate migration.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 092 — Create initial Engagement Project migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only the schema migration for current Project model.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add milestones.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 093 — Define IProjectRepository read contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository contract for tenant-scoped project lookup by ID.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not implement it or add list/create methods.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 094 — Implement tenant-scoped project lookup repository

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only repository lookup by tenant + project ID.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add business/facade/API.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 095 — Define project detail response contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the public/internal DTO returned for project detail.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add mapping or route.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 096 — Implement project detail business mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only business-layer mapping from Project persistence/domain model to project detail contract.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add facade/controller.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 097 — Implement project detail facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only facade tenant/authorization orchestration for project detail lookup.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add controller route.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 098 — Add project detail HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only one GET endpoint for authorized project detail.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add list/dashboard/milestones.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 099 — Add cross-tenant project-detail HTTP test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only a test proving Tenant A cannot fetch Tenant B project detail.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add broader auth test suite.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 100 — Create Milestone entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tenant/project-scoped Milestone entity and state fields.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not map or expose it.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 101 — Add Milestone EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes/FK constraints for Milestone.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 102 — Add Milestone DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only Milestone with EngagementDbContext.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not generate migration.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 103 — Create Milestone schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only milestone-related schema delta.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add API behavior.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 104 — Define project milestone-list response contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only DTO contracts for milestone list display.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not query data.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 105 — Implement milestone list repository query

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tenant/project-scoped milestone list query.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add business/facade/controller.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 106 — Implement milestone list business mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only mapping/sorting behavior for milestone list.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add facade/controller.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 107 — Implement milestone list facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authorization orchestration for milestone list.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add route.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 108 — Add milestone list HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only GET endpoint for project milestones.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add mutation endpoints.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 109 — Add milestone list tenant-isolation test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only cross-tenant negative test for milestone list.

CONSTRAINT: All Engagement data access is tenant-scoped and follows Controller -> Facade -> Business -> Data -> Repository -> DbContext.

RESTRICTION: Do not add unrelated milestone tests.

USAGE: Read architecture/dotnet/database/multi-tenancy/authorization rules and matching endpoint/persistence skills.

BEHAVIOR: Run the smallest focused compile/unit/integration/HTTP test for the seam. STOP.
```

## Prompt 110 — Define project-health response contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the DTO/contract for project health displayed by the portal.

CONSTRAINT: Project-health is synchronous HTTP because the browser requires an immediate answer.

RESTRICTION: Do not add query logic.

USAGE: Read api-design/architecture/dotnet/multi-tenancy/authorization rules.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 111 — Implement project-health repository query

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tenant/project-scoped data query required for the current health fields.

CONSTRAINT: Project-health is synchronous HTTP because the browser requires an immediate answer.

RESTRICTION: Do not map or expose HTTP.

USAGE: Read api-design/architecture/dotnet/multi-tenancy/authorization rules.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 112 — Implement project-health business mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only business mapping/calculation for the existing project-health contract.

CONSTRAINT: Project-health is synchronous HTTP because the browser requires an immediate answer.

RESTRICTION: Do not add facade or route.

USAGE: Read api-design/architecture/dotnet/multi-tenancy/authorization rules.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 113 — Implement project-health facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authorization/orchestration for project-health read.

CONSTRAINT: Project-health is synchronous HTTP because the browser requires an immediate answer.

RESTRICTION: Do not add route.

USAGE: Read api-design/architecture/dotnet/multi-tenancy/authorization rules.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 114 — Add project-health HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only one GET route returning project health.

CONSTRAINT: Project-health is synchronous HTTP because the browser requires an immediate answer.

RESTRICTION: Do not build dashboard aggregation.

USAGE: Read api-design/architecture/dotnet/multi-tenancy/authorization rules.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 115 — Add project-health HTTP contract test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only success/not-found/forbidden tests for the health endpoint.

CONSTRAINT: Project-health is synchronous HTTP because the browser requires an immediate answer.

RESTRICTION: Do not create Angular UI.

USAGE: Read api-design/architecture/dotnet/multi-tenancy/authorization rules.

BEHAVIOR: Run focused verification and STOP.
```

# Part 5 — Document metadata and storage foundation

## Prompt 116 — Create Document entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the Document aggregate root metadata entity: IDs, TenantId, ProjectId, type/category/title/status/visibility/current-version reference and timestamps as required.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not add version entity, EF mapping or Blob code.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 117 — Add Document EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes/concurrency configuration for Document.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not add DbSet/migration/version schema.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 118 — Create DocumentsDbContext

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the Documents service DbContext shell.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not register entities yet.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 119 — Add Document DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only Document in DocumentsDbContext.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not generate migration.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 120 — Create initial Document metadata migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only Document metadata schema.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not add DocumentVersion yet.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 121 — Create DocumentVersion entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only immutable DocumentVersion metadata fields including version ID/number, document ID, Blob object key, MIME type, size, content hash, uploader and timestamp.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not map/persist/upload.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 122 — Add DocumentVersion EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping, uniqueness and FK/index constraints for DocumentVersion.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 123 — Add DocumentVersion DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only DocumentVersion with DocumentsDbContext.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not generate migration.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 124 — Create DocumentVersion schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only the DocumentVersion schema delta.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not implement upload.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 125 — Define document visibility enum/value object

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the approved internal/client visibility representation and invariants.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not change publication behavior.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 126 — Define document lifecycle status enum/value object

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only document/version lifecycle states required by current design.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not implement transitions.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 127 — Define IBlobDocumentStore contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the storage abstraction for binary put/open/delete-if-orphan operations needed by Documents.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not implement Azure Blob calls.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 128 — Add Azure Blob SDK package to Documents infrastructure

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the approved Azure Blob SDK package references.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not configure clients or containers.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 129 — Register BlobServiceClient from Aspire/configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only DI/configuration for Blob client using service discovery/configuration.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not create container or document methods.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 130 — Define the private document Blob container name in configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the container-name configuration/options contract and validation.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not create public access or storage methods.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 131 — Implement Blob container bootstrap with private access

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only container existence/bootstrap behavior with private access.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not upload files.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 132 — Implement Blob put operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only binary upload to the private container using an opaque service-generated object key.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not persist SQL metadata or expose URL.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 133 — Implement Blob open-read operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only authorized-internal storage abstraction method to open/read by object key.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not add HTTP download endpoint.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

## Prompt 134 — Add Blob storage integration test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Azurite/Aspire-compatible integration tests proving put/read and private-container behavior.

CONSTRAINT: Documents owns metadata; Blob owns binaries; object keys are opaque and never authorization grants.

RESTRICTION: Do not involve SQL metadata.

USAGE: Read document-management/blob-storage/database/security rules and relevant document/persistence skills.

BEHAVIOR: Run only the smallest model/storage verification for this seam. STOP.
```

# Part 6 — Document upload and catalog seams

## Prompt 135 — Define document-upload request metadata contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only non-binary upload metadata contract fields such as project, title, category/type and visibility intent.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not add controller or multipart handling.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 136 — Define document-upload result contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only response contract containing document/version identity and safe metadata.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not expose Blob URL/object key.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 137 — Implement upload file-name normalization helper

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only safe display-file-name normalization/validation behavior.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not upload or persist.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 138 — Implement upload MIME-type validation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only allowed MIME-type validation policy required by current requirements/security rules.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not inspect file content beyond this seam.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 139 — Implement upload size-limit validation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only configured upload-size validation.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not upload or persist.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 140 — Implement document content hash calculation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only streaming hash calculation used to record content integrity.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not deduplicate or persist.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 141 — Implement Document repository create operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only SQL repository persistence for a new Document metadata record.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not create version or Blob object.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 142 — Implement DocumentVersion repository create operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only SQL repository persistence for one DocumentVersion.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not update current version pointer.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 143 — Implement current-version pointer update repository operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository operation that sets Document.CurrentVersionId with concurrency protection.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not upload or publish.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 144 — Implement upload business transaction boundary

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only SQL transaction orchestration that creates Document + first DocumentVersion + current-version pointer using already-stored Blob object metadata.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not add Blob upload, facade or controller.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 145 — Implement upload facade orchestration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only facade orchestration that authorizes tenant/project, validates metadata/file, stores Blob, then invokes the existing SQL business transaction and cleans up orphan Blob on SQL failure according to design.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not add controller endpoint or publication.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 146 — Add document upload HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the multipart/streaming HTTP endpoint that calls the existing upload facade operation.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not add listing/download/version-2 behavior.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 147 — Add successful upload integration test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only an end-to-end service test proving one upload creates one private Blob plus one Document plus one version with matching hash/size.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not test publication.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 148 — Add failed-SQL orphan-cleanup test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the focused test for Blob cleanup/compensation when metadata persistence fails after Blob put.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not introduce distributed transactions.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 149 — Add cross-tenant upload denial test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving a user cannot upload into a project outside authorized tenant/project membership.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not add download tests.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 150 — Define document-list query contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only query parameters for project/category/status/visibility pagination/filtering.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not query SQL.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 151 — Define document-list response contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only safe metadata DTO for list results.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not expose Blob object keys.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 152 — Implement document-list repository query

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tenant/project-scoped SQL query with approved filters and pagination.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not add business/facade/controller.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 153 — Implement document-list business mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only mapping from repository rows/entities to list DTO.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not add facade/controller.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 154 — Implement document-list facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authorization and orchestration for listing documents.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not add endpoint.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 155 — Add document-list HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only GET endpoint for authorized project document catalog.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not return binary content.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

## Prompt 156 — Add document-list cross-tenant test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-003, LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only negative test proving cross-tenant document metadata cannot be enumerated.

CONSTRAINT: Document API never reveals raw Blob credentials/URLs and every project-scoped operation derives tenant authorization server-side.

RESTRICTION: Do not add UI.

USAGE: Read document-management/blob-storage/multi-tenancy/api rules and upload/list skills.

BEHAVIOR: Run the narrowest unit/integration/HTTP verification and STOP.
```

# Part 7 — Document versioning, publication and authorized download

## Prompt 157 — Define add-document-version request contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only metadata/binary request contract for adding a new version to an existing document.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not implement behavior.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 158 — Implement next-version-number repository query

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only concurrency-safe repository behavior needed to determine/allocate the next version sequence.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not create version.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 159 — Implement add-version SQL transaction

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only transaction that inserts a new DocumentVersion and updates current-version pointer without modifying prior versions.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not upload Blob or publish.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 160 — Implement add-version facade orchestration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authorization/validation/Blob-put orchestration around the existing add-version transaction.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not add HTTP endpoint or publication.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 161 — Add add-document-version HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only POST endpoint for creating a new version of an existing document.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not add publish action.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 162 — Add immutable-v1/v2 integration test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-005
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document content changes create immutable versions. Existing published or approved versions are never overwritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving creating v2 leaves all v1 persisted metadata/content identifiers unchanged.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not test approvals.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 163 — Define publish-document-version command contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only command/route contract identifying the exact document and version to publish.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not implement transition.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 164 — Implement publication eligibility business rule

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only pure business rule validating that the target version is eligible for publication.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not persist or emit events.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 165 — Implement publish-version repository transition

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only concurrency-safe persistence transition for publication metadata on the exact version/document.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not create outbox event yet.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 166 — Implement publish-version facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authorization/orchestration for the existing publication transition.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not add route or outbox.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 167 — Add publish-document-version HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authenticated internal-user HTTP action to publish one exact version.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not emit Service Bus directly.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 168 — Add client-visible publication authorization test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tests proving unpublished/internal versions are hidden and published client-visible versions are visible to authorized client users.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not test binary download yet.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 169 — Define document-download response behavior

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the HTTP/content-disposition contract for downloading an exact document version.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not open Blob yet.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 170 — Implement authorized version lookup for download

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository/business lookup that returns Blob object key after tenant/project/document/version visibility authorization has succeeded.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not stream data.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 171 — Implement download facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only facade authorization/orchestration that resolves the exact allowed version then opens Blob stream through IBlobDocumentStore.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not add endpoint.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 172 — Add authorized document download endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only GET/stream endpoint for exact authorized version content.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not return durable/public Blob URL.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 173 — Add unpublished-version download denial test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only client test proving an authorized project member still cannot download an unpublished/internal-only version.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not add approval tests.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 174 — Add cross-tenant version download denial test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving exact version IDs cannot bypass tenant/project authorization.

CONSTRAINT: Published/approved versions are immutable; access authorization occurs before Blob read.

RESTRICTION: Do not add listing tests.

USAGE: Read document-management/document-versioning/blob-storage/authorization rules and matching skills.

BEHAVIOR: Run focused verification and STOP.
```

# Part 8 — Transactional outbox and Service Bus foundation

## Prompt 175 — Create Documents OutboxMessage entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the service-local OutboxMessage persistence model in Documents.Core.

CONSTRAINT: Outbox belongs to the service database that commits the state change.

RESTRICTION: Do not add EF mapping, relay or Service Bus publisher.

USAGE: Read messaging/outbox-inbox/database rules and `add-outbox-publisher` skill.

BEHAVIOR: Run entity tests/compile only. STOP.
```

## Prompt 176 — Add Documents OutboxMessage EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes for Documents OutboxMessage.

CONSTRAINT: Include status/attempt/created/processed fields and unique event/message identity as designed.

RESTRICTION: Do not add relay.

USAGE: Read database/outbox rules.

BEHAVIOR: Run model test. STOP.
```

## Prompt 177 — Add Documents OutboxMessage DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only OutboxMessage in the Documents DbContext.

CONSTRAINT: No other schema changes.

RESTRICTION: Do not generate migration.

USAGE: Read database rule.

BEHAVIOR: Build/model test. STOP.
```

## Prompt 178 — Create Documents outbox schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only the Documents outbox schema migration.

CONSTRAINT: Migration delta is outbox-only.

RESTRICTION: Do not add triggers/relay.

USAGE: Read database/outbox rules.

BEHAVIOR: Inspect migration and run disposable SQL migration test. STOP.
```

## Prompt 179 — Create Approvals OutboxMessage entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the service-local OutboxMessage persistence model in Approvals.Core.

CONSTRAINT: Outbox belongs to the service database that commits the state change.

RESTRICTION: Do not add EF mapping, relay or Service Bus publisher.

USAGE: Read messaging/outbox-inbox/database rules and `add-outbox-publisher` skill.

BEHAVIOR: Run entity tests/compile only. STOP.
```

## Prompt 180 — Add Approvals OutboxMessage EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes for Approvals OutboxMessage.

CONSTRAINT: Include status/attempt/created/processed fields and unique event/message identity as designed.

RESTRICTION: Do not add relay.

USAGE: Read database/outbox rules.

BEHAVIOR: Run model test. STOP.
```

## Prompt 181 — Add Approvals OutboxMessage DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only OutboxMessage in the Approvals DbContext.

CONSTRAINT: No other schema changes.

RESTRICTION: Do not generate migration.

USAGE: Read database rule.

BEHAVIOR: Build/model test. STOP.
```

## Prompt 182 — Create Approvals outbox schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only the Approvals outbox schema migration.

CONSTRAINT: Migration delta is outbox-only.

RESTRICTION: Do not add triggers/relay.

USAGE: Read database/outbox rules.

BEHAVIOR: Inspect migration and run disposable SQL migration test. STOP.
```

## Prompt 183 — Create Engagement OutboxMessage entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the service-local OutboxMessage persistence model in Engagement.Core.

CONSTRAINT: Outbox belongs to the service database that commits the state change.

RESTRICTION: Do not add EF mapping, relay or Service Bus publisher.

USAGE: Read messaging/outbox-inbox/database rules and `add-outbox-publisher` skill.

BEHAVIOR: Run entity tests/compile only. STOP.
```

## Prompt 184 — Add Engagement OutboxMessage EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes for Engagement OutboxMessage.

CONSTRAINT: Include status/attempt/created/processed fields and unique event/message identity as designed.

RESTRICTION: Do not add relay.

USAGE: Read database/outbox rules.

BEHAVIOR: Run model test. STOP.
```

## Prompt 185 — Add Engagement OutboxMessage DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only OutboxMessage in the Engagement DbContext.

CONSTRAINT: No other schema changes.

RESTRICTION: Do not generate migration.

USAGE: Read database rule.

BEHAVIOR: Build/model test. STOP.
```

## Prompt 186 — Create Engagement outbox schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only the Engagement outbox schema migration.

CONSTRAINT: Migration delta is outbox-only.

RESTRICTION: Do not add triggers/relay.

USAGE: Read database/outbox rules.

BEHAVIOR: Inspect migration and run disposable SQL migration test. STOP.
```

## Prompt 187 — Create Identity OutboxMessage entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the service-local OutboxMessage persistence model in Identity.Core.

CONSTRAINT: Outbox belongs to the service database that commits the state change.

RESTRICTION: Do not add EF mapping, relay or Service Bus publisher.

USAGE: Read messaging/outbox-inbox/database rules and `add-outbox-publisher` skill.

BEHAVIOR: Run entity tests/compile only. STOP.
```

## Prompt 188 — Add Identity OutboxMessage EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes for Identity OutboxMessage.

CONSTRAINT: Include status/attempt/created/processed fields and unique event/message identity as designed.

RESTRICTION: Do not add relay.

USAGE: Read database/outbox rules.

BEHAVIOR: Run model test. STOP.
```

## Prompt 189 — Add Identity OutboxMessage DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only OutboxMessage in the Identity DbContext.

CONSTRAINT: No other schema changes.

RESTRICTION: Do not generate migration.

USAGE: Read database rule.

BEHAVIOR: Build/model test. STOP.
```

## Prompt 190 — Create Identity outbox schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only the Identity outbox schema migration.

CONSTRAINT: Migration delta is outbox-only.

RESTRICTION: Do not add triggers/relay.

USAGE: Read database/outbox rules.

BEHAVIOR: Inspect migration and run disposable SQL migration test. STOP.
```

## Prompt 191 — Define integration-event envelope contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only the shared integration-event envelope contract for message ID, event type/version, occurred-at, correlation, causation and tenant context where appropriate.

CONSTRAINT: Contracts contain no EF/domain entities and preserve W3C trace context metadata.

RESTRICTION: Do not define business events yet or publish messages.

USAGE: Read messaging/observability/messaging rules.

BEHAVIOR: Contract serialization test only. STOP.
```

## Prompt 192 — Define DocumentPublished integration event v1

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the versioned `DocumentPublished` integration-event contract with exact document/version/project/tenant identifiers and publication metadata needed by consumers.

CONSTRAINT: Past-tense fact; no Blob credential/object key unless explicitly required and safe.

RESTRICTION: Do not persist or publish it.

USAGE: Read messaging/messaging/document-management rules and `add-integration-event` skill.

BEHAVIOR: Serialization/contract test only. STOP.
```

## Prompt 193 — Persist DocumentPublished in publication transaction

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-006, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Modify only the existing publish-version transaction so publication state and a `DocumentPublished` outbox row commit atomically.

CONSTRAINT: No direct Service Bus call from business transaction.

RESTRICTION: Do not implement relay or consumers.

USAGE: Read outbox rules and `add-outbox-publisher` skill.

BEHAVIOR: Integration test proves rollback/commit atomicity. STOP.
```

## Prompt 194 — Create Documents outbox timer trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only a TimerTrigger entry point in `Documents.Functions` that invokes an outbox-relay abstraction.

CONSTRAINT: Trigger remains thin and contains no SQL or Service Bus business logic.

RESTRICTION: Do not implement relay internals yet.

USAGE: Read dotnet/outbox-inbox rules.

BEHAVIOR: Functions compile/trigger binding test. STOP.
```

## Prompt 195 — Implement Documents pending-outbox repository query

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository query that obtains a bounded batch of pending Documents outbox rows using the approved concurrency strategy.

CONSTRAINT: Use a bounded batch and approved concurrency/locking semantics; query only the owning service database.

RESTRICTION: Do not send messages or mark processed.

USAGE: Read database/outbox rules.

BEHAVIOR: Focused repository integration test. STOP.
```

## Prompt 196 — Implement Documents Service Bus publish adapter

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only adapter that publishes one already-serialized integration event/envelope to configured Service Bus entity.

CONSTRAINT: Configuration owns broker entity names and credentials; trace metadata preserved.

RESTRICTION: Do not query outbox or retry batch.

USAGE: Read messaging/observability rules.

BEHAVIOR: Mock/emulator publish test. STOP.
```

## Prompt 197 — Implement Documents outbox mark-processed operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository operation that marks one outbox message successfully processed using concurrency protection.

CONSTRAINT: Must not mark failed publication as success.

RESTRICTION: Do not implement relay loop.

USAGE: Read outbox/database rules.

BEHAVIOR: Focused persistence test. STOP.
```

## Prompt 198 — Implement Documents outbox relay batch operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only one bounded relay pass: read pending rows, publish each, mark successes, record bounded failure metadata.

CONSTRAINT: At-least-once semantics; safe to rerun; no domain state mutation.

RESTRICTION: Do not add new timer schedule or business events.

USAGE: Read outbox/messaging/observability rules.

BEHAVIOR: Focused relay tests for success, partial failure and repeat execution. STOP.
```

## Prompt 199 — Create Approvals outbox timer trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only a TimerTrigger entry point in `Approvals.Functions` that invokes an outbox-relay abstraction.

CONSTRAINT: Trigger remains thin and contains no SQL or Service Bus business logic.

RESTRICTION: Do not implement relay internals yet.

USAGE: Read dotnet/outbox-inbox rules.

BEHAVIOR: Functions compile/trigger binding test. STOP.
```

## Prompt 200 — Implement Approvals pending-outbox repository query

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository query that obtains a bounded batch of pending Approvals outbox rows using the approved concurrency strategy.

CONSTRAINT: Use a bounded batch and approved concurrency/locking semantics; query only the owning service database.

RESTRICTION: Do not send messages or mark processed.

USAGE: Read database/outbox rules.

BEHAVIOR: Focused repository integration test. STOP.
```

## Prompt 201 — Implement Approvals Service Bus publish adapter

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only adapter that publishes one already-serialized integration event/envelope to configured Service Bus entity.

CONSTRAINT: Configuration owns broker entity names and credentials; trace metadata preserved.

RESTRICTION: Do not query outbox or retry batch.

USAGE: Read messaging/observability rules.

BEHAVIOR: Mock/emulator publish test. STOP.
```

## Prompt 202 — Implement Approvals outbox mark-processed operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository operation that marks one outbox message successfully processed using concurrency protection.

CONSTRAINT: Must not mark failed publication as success.

RESTRICTION: Do not implement relay loop.

USAGE: Read outbox/database rules.

BEHAVIOR: Focused persistence test. STOP.
```

## Prompt 203 — Implement Approvals outbox relay batch operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only one bounded relay pass: read pending rows, publish each, mark successes, record bounded failure metadata.

CONSTRAINT: At-least-once semantics; safe to rerun; no domain state mutation.

RESTRICTION: Do not add new timer schedule or business events.

USAGE: Read outbox/messaging/observability rules.

BEHAVIOR: Focused relay tests for success, partial failure and repeat execution. STOP.
```

## Prompt 204 — Create Engagement outbox timer trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only a TimerTrigger entry point in `Engagement.Functions` that invokes an outbox-relay abstraction.

CONSTRAINT: Trigger remains thin and contains no SQL or Service Bus business logic.

RESTRICTION: Do not implement relay internals yet.

USAGE: Read dotnet/outbox-inbox rules.

BEHAVIOR: Functions compile/trigger binding test. STOP.
```

## Prompt 205 — Implement Engagement pending-outbox repository query

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository query that obtains a bounded batch of pending Engagement outbox rows using the approved concurrency strategy.

CONSTRAINT: Use a bounded batch and approved concurrency/locking semantics; query only the owning service database.

RESTRICTION: Do not send messages or mark processed.

USAGE: Read database/outbox rules.

BEHAVIOR: Focused repository integration test. STOP.
```

## Prompt 206 — Implement Engagement Service Bus publish adapter

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only adapter that publishes one already-serialized integration event/envelope to configured Service Bus entity.

CONSTRAINT: Configuration owns broker entity names and credentials; trace metadata preserved.

RESTRICTION: Do not query outbox or retry batch.

USAGE: Read messaging/observability rules.

BEHAVIOR: Mock/emulator publish test. STOP.
```

## Prompt 207 — Implement Engagement outbox mark-processed operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository operation that marks one outbox message successfully processed using concurrency protection.

CONSTRAINT: Must not mark failed publication as success.

RESTRICTION: Do not implement relay loop.

USAGE: Read outbox/database rules.

BEHAVIOR: Focused persistence test. STOP.
```

## Prompt 208 — Implement Engagement outbox relay batch operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only one bounded relay pass: read pending rows, publish each, mark successes, record bounded failure metadata.

CONSTRAINT: At-least-once semantics; safe to rerun; no domain state mutation.

RESTRICTION: Do not add new timer schedule or business events.

USAGE: Read outbox/messaging/observability rules.

BEHAVIOR: Focused relay tests for success, partial failure and repeat execution. STOP.
```

## Prompt 209 — Create Identity outbox timer trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only a TimerTrigger entry point in `Identity.Functions` that invokes an outbox-relay abstraction.

CONSTRAINT: Trigger remains thin and contains no SQL or Service Bus business logic.

RESTRICTION: Do not implement relay internals yet.

USAGE: Read dotnet/outbox-inbox rules.

BEHAVIOR: Functions compile/trigger binding test. STOP.
```

## Prompt 210 — Implement Identity pending-outbox repository query

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository query that obtains a bounded batch of pending Identity outbox rows using the approved concurrency strategy.

CONSTRAINT: Use a bounded batch and approved concurrency/locking semantics; query only the owning service database.

RESTRICTION: Do not send messages or mark processed.

USAGE: Read database/outbox rules.

BEHAVIOR: Focused repository integration test. STOP.
```

## Prompt 211 — Implement Identity Service Bus publish adapter

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only adapter that publishes one already-serialized integration event/envelope to configured Service Bus entity.

CONSTRAINT: Configuration owns broker entity names and credentials; trace metadata preserved.

RESTRICTION: Do not query outbox or retry batch.

USAGE: Read messaging/observability rules.

BEHAVIOR: Mock/emulator publish test. STOP.
```

## Prompt 212 — Implement Identity outbox mark-processed operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only repository operation that marks one outbox message successfully processed using concurrency protection.

CONSTRAINT: Must not mark failed publication as success.

RESTRICTION: Do not implement relay loop.

USAGE: Read outbox/database rules.

BEHAVIOR: Focused persistence test. STOP.
```

## Prompt 213 — Implement Identity outbox relay batch operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only one bounded relay pass: read pending rows, publish each, mark successes, record bounded failure metadata.

CONSTRAINT: At-least-once semantics; safe to rerun; no domain state mutation.

RESTRICTION: Do not add new timer schedule or business events.

USAGE: Read outbox/messaging/observability rules.

BEHAVIOR: Focused relay tests for success, partial failure and repeat execution. STOP.
```

# Part 9 — Approval request and decision model

## Prompt 214 — Create ApprovalRequest entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only ApprovalRequest entity with tenant/project, target type/id, exact target version where applicable, requested-by/at, state and correlation fields.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add decision entity, EF mapping or consumer.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 215 — Add ApprovalRequest EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes/concurrency rules for ApprovalRequest.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 216 — Create ApprovalsDbContext

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Approvals DbContext shell.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not register entities yet.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 217 — Add ApprovalRequest DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only ApprovalRequest.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not generate migration.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 218 — Create ApprovalRequest schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only ApprovalRequest schema.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add ApprovalDecision.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 219 — Create ApprovalDecision entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only immutable ApprovalDecision business-record entity with approver, decision, exact target identity/version, comments, timestamp and audit context.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not map/persist.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 220 — Add ApprovalDecision EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/append-only constraints/indexes for ApprovalDecision.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 221 — Add ApprovalDecision DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only ApprovalDecision.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not generate migration.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 222 — Create ApprovalDecision schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only ApprovalDecision schema delta.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not implement approve/reject commands.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 223 — Define approval-request read response contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only DTO required for client review of an approval and exact target version metadata.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not query Documents directly or add endpoint.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 224 — Implement approval-request repository lookup

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tenant-scoped approval lookup by approval ID.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not enrich target metadata.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 225 — Implement approval-request facade authorization

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authorization/orchestration for reading one approval request.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add route or cross-service metadata call.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 226 — Add approval-request GET endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only one GET endpoint returning the approval request state/target identity currently owned by Approvals.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add approve/reject action.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 227 — Define approval decision command contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only request contract for approve/reject decision plus optional comments and optimistic/concurrency token if designed.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not implement transition.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 228 — Implement approval-state transition business rule

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only pure rule validating Pending -> Approved or Pending -> Rejected and forbidding repeat terminal transition.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not persist.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 229 — Implement approval decision repository transaction

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only transaction that applies ApprovalRequest terminal state and appends one ApprovalDecision record.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not create outbox event yet.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 230 — Implement approve facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authenticated/authorized orchestration for Approved decision using existing business/repository behavior.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add controller or event.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 231 — Add approve HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the approve action for one approval request.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not implement reject or notifications.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 232 — Implement reject facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authenticated/authorized orchestration for Rejected decision.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add route/event.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 233 — Add reject HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the reject action.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add notifications.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 234 — Add repeated-decision conflict test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving a terminal approval cannot be approved/rejected again and prior decision row remains unchanged.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not test events.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 235 — Add stale-document-version approval test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving approval for document v3 is bound to v3 and does not confer approval on subsequently created v4.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not implement v4 approval request automatically.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

## Prompt 236 — Add cross-tenant approval decision denial test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving Tenant A cannot decide Tenant B approval even with a valid approval ID.

CONSTRAINT: Approvals owns approval state/history; document approvals always identify the exact immutable version.

RESTRICTION: Do not add UI.

USAGE: Read approvals/authorization/multi-tenancy/database rules and `add-approval-workflow` skill.

BEHAVIOR: Run the smallest focused model/repository/API test and STOP.
```

# Part 10 — DocumentPublished -> ApprovalRequested asynchronous workflow

## Prompt 237 — Define Approvals inbox entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only transactional inbox persistence entity for processed integration-message identity in Approvals.

CONSTRAINT: Inbox prevents duplicate durable approval creation under at-least-once delivery.

RESTRICTION: Do not add consumer.

USAGE: Read outbox-inbox/database rules and `add-inbox-consumer` skill.

BEHAVIOR: Entity test/compile only. STOP.
```

## Prompt 238 — Add Approvals inbox EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes for Approvals inbox.

CONSTRAINT: Message ID must be uniquely constrained.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read database/outbox-inbox rules.

BEHAVIOR: Model test. STOP.
```

## Prompt 239 — Add Approvals inbox DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only inbox set with ApprovalsDbContext.

CONSTRAINT: No other model change.

RESTRICTION: Do not generate migration.

USAGE: Read database rule.

BEHAVIOR: Build/model test. STOP.
```

## Prompt 240 — Create Approvals inbox schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only inbox schema delta.

CONSTRAINT: The migration must contain only the inbox table/index delta in the Approvals database.

RESTRICTION: Do not add consumer.

USAGE: Read database rule.

BEHAVIOR: Inspect/test migration. STOP.
```

## Prompt 241 — Create DocumentPublished Service Bus trigger shell in Approvals

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only ServiceBusTrigger function binding that receives `DocumentPublished` envelope and delegates to a facade/consumer abstraction.

CONSTRAINT: Trigger contains no domain logic or direct DbContext access.

RESTRICTION: Do not create approval request yet.

USAGE: Read dotnet/messaging rules.

BEHAVIOR: Functions binding/compile test. STOP.
```

## Prompt 242 — Implement DocumentPublished approval-policy decision

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only pure policy that determines whether the published document type/status requires client approval.

CONSTRAINT: Policy uses event metadata/current approved rules only; no side effects.

RESTRICTION: Do not persist approval request.

USAGE: Read approvals/document-management rules.

BEHAVIOR: Pure unit tests. STOP.
```

## Prompt 243 — Implement idempotent DocumentPublished consumer transaction

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only one transaction that checks inbox message ID, conditionally creates one ApprovalRequest if policy requires, records inbox completion, and does nothing on duplicate.

CONSTRAINT: No notification/event publication yet; exact document/version IDs come from the event.

RESTRICTION: Do not create `ApprovalRequested` outbox row yet.

USAGE: Read outbox-inbox/approvals rules.

BEHAVIOR: Integration tests for first delivery, duplicate delivery and no-approval policy. STOP.
```

## Prompt 244 — Define ApprovalRequested integration event v1

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only versioned past-tense `ApprovalRequested` event contract with approval, tenant/project, target identity/version and notification-safe metadata.

CONSTRAINT: No document Blob information or secrets.

RESTRICTION: Do not publish it.

USAGE: Read messaging rules.

BEHAVIOR: Serialization/contract test. STOP.
```

## Prompt 245 — Persist ApprovalRequested in approval-creation transaction

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Extend only the existing approval-creation consumer transaction to append an ApprovalRequested outbox row atomically when a new approval is created.

CONSTRAINT: Duplicate DocumentPublished delivery must not create duplicate approval or duplicate logical ApprovalRequested event.

RESTRICTION: Do not implement notification consumer.

USAGE: Read outbox/inbox rules.

BEHAVIOR: Integration test for atomicity and duplicate delivery. STOP.
```

## Prompt 246 — Define ApprovalGranted integration event v1

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only versioned `ApprovalGranted` event contract with approval, tenant/project, exact target/version, actor and decision timestamp metadata needed by consumers.

CONSTRAINT: Past-tense fact; no service-internal EF types.

RESTRICTION: Do not publish or consume it.

USAGE: Read messaging rules.

BEHAVIOR: Serialization test. STOP.
```

## Prompt 247 — Persist ApprovalGranted in decision transaction

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Extend only the corresponding decision transaction so `ApprovalGranted` outbox row commits atomically with ApprovalRequest state and ApprovalDecision append.

CONSTRAINT: No direct Service Bus publication.

RESTRICTION: Do not implement notification/milestone consumer.

USAGE: Read approvals/outbox rules.

BEHAVIOR: Integration test proves transaction atomicity. STOP.
```

## Prompt 248 — Define ApprovalRejected integration event v1

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only versioned `ApprovalRejected` event contract with approval, tenant/project, exact target/version, actor and decision timestamp metadata needed by consumers.

CONSTRAINT: Past-tense fact; no service-internal EF types.

RESTRICTION: Do not publish or consume it.

USAGE: Read messaging rules.

BEHAVIOR: Serialization test. STOP.
```

## Prompt 249 — Persist ApprovalRejected in decision transaction

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Extend only the corresponding decision transaction so `ApprovalRejected` outbox row commits atomically with ApprovalRequest state and ApprovalDecision append.

CONSTRAINT: No direct Service Bus publication.

RESTRICTION: Do not implement notification/milestone consumer.

USAGE: Read approvals/outbox rules.

BEHAVIOR: Integration test proves transaction atomicity. STOP.
```

# Part 11 — Asynchronous notifications

## Prompt 250 — Create NotificationDelivery entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only notification delivery/history entity with tenant, message/event identity, recipient reference/address as approved, template/type, state, attempts and timestamps.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not add EF mapping or email provider.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 251 — Add NotificationDelivery EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes for NotificationDelivery.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 252 — Create NotificationsDbContext

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Notifications DbContext shell.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not register entities.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 253 — Add NotificationDelivery DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only NotificationDelivery.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not generate migration.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 254 — Create NotificationDelivery schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only notification-delivery schema.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not add consumers.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 255 — Create Notifications inbox entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only processed-message inbox entity.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not map or consume.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 256 — Add Notifications inbox EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only inbox mapping with unique message ID.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 257 — Add Notifications inbox DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only inbox entity.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not generate migration.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 258 — Create Notifications inbox schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only inbox schema delta.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not add consumer.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 259 — Define notification sender abstraction

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Create only provider-neutral email notification sender contract.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not choose/configure a production provider.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 260 — Implement development notification sender

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only local/development sender implementation appropriate to the approved development baseline.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not add templates or consumers.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 261 — Create ApprovalRequested notification trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only ServiceBusTrigger function for ApprovalRequested -> facade/consumer delegation.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not send email in trigger.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 262 — Implement ApprovalRequested notification mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only mapping from ApprovalRequested event to notification template model/recipient intent.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not send or persist.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 263 — Implement idempotent ApprovalRequested notification consumer

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only inbox + delivery-record transaction and one sender invocation strategy according to notification durability design.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Duplicate event must not create duplicate logical notification. Do not update approval state.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 264 — Create ApprovalGranted notification trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only trigger binding/delegation for ApprovalGranted.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not send in trigger.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 265 — Implement ApprovalGranted notification mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only event -> notification model mapping.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not persist/send.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 266 — Implement idempotent ApprovalGranted notification consumer

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only durable/idempotent consumer behavior for granted notifications.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not touch Engagement state.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 267 — Create ApprovalRejected notification trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only trigger binding/delegation for ApprovalRejected.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not send in trigger.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 268 — Implement ApprovalRejected notification mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only event -> notification model mapping.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not persist/send.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 269 — Implement idempotent ApprovalRejected notification consumer

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only durable/idempotent consumer behavior for rejected notifications.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not touch Engagement state.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

## Prompt 270 — Add notification duplicate-delivery integration test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-009, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests and outcomes trigger notification work asynchronously so notification availability does not control business transaction success. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving replayed ApprovalRequested event does not result in a second logical notification delivery.

CONSTRAINT: Notification failure/retry is temporally decoupled from the originating approval transaction.

RESTRICTION: Do not test milestone updates.

USAGE: Read notifications/messaging/outbox-inbox/observability rules and `add-notification` skill.

BEHAVIOR: Run narrow model/consumer tests and STOP.
```

# Part 12 — Approval outcome -> Engagement propagation

## Prompt 271 — Create Engagement inbox entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only processed integration-message entity in Engagement.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not add consumer.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 272 — Add Engagement inbox EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only inbox mapping/unique message identity.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 273 — Add Engagement inbox DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only inbox set.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not generate migration.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 274 — Create Engagement inbox schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only inbox schema delta.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not add trigger.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 275 — Create ApprovalGranted Engagement trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only ServiceBusTrigger binding/delegation for ApprovalGranted.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not update milestones in trigger.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 276 — Define milestone approval-target matching rule

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only pure business rule that determines whether an approval outcome targets a specific milestone and what transition is eligible.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not persist.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 277 — Implement idempotent ApprovalGranted milestone consumer

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only transaction that checks inbox, applies eligible milestone transition, records inbox completion and no-ops on duplicate.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not send notifications or call Approvals by HTTP.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 278 — Create ApprovalRejected Engagement trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only ServiceBusTrigger binding/delegation for ApprovalRejected.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not mutate state in trigger.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 279 — Define rejected-approval milestone behavior

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only pure rule for eligible milestone state when relevant approval is rejected.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not persist.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 280 — Implement idempotent ApprovalRejected milestone consumer

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only inbox + eligible milestone transition transaction for rejection.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not notify or change approval history.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

## Prompt 281 — Add duplicate ApprovalGranted milestone test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving replay does not apply milestone transition twice or create duplicate durable side effects.

CONSTRAINT: Cross-domain outcome propagation is asynchronous and idempotent; Engagement owns milestone state.

RESTRICTION: Do not test notifications.

USAGE: Read domain-boundaries/messaging/outbox-inbox rules and event-consumer skill.

BEHAVIOR: Run focused consumer integration test and STOP.
```

# Part 13 — Durable audit trail

## Prompt 282 — Create AuditRecord entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only append-only AuditRecord persistence entity with tenant, actor, action, resource identity, occurred-at, correlation/causation, source and safe before/after metadata as designed.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not add EF mapping/consumer.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 283 — Add AuditRecord EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only append-only-oriented EF mapping/indexes for AuditRecord.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 284 — Create AuditDbContext

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Audit DbContext shell.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not register entity.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 285 — Add AuditRecord DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only AuditRecord.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not generate migration.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 286 — Create AuditRecord schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only audit schema.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not consume events.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 287 — Create Audit inbox entity

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only audit processed-message inbox entity.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not map.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 288 — Add Audit inbox EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only unique message identity mapping.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 289 — Add Audit inbox DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only audit inbox entity.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not generate migration.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 290 — Create Audit inbox schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only audit inbox schema.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not add triggers.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 291 — Create ApprovalRequested audit trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only ServiceBusTrigger binding/delegation for ApprovalRequested.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not write AuditDb in trigger.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 292 — Implement ApprovalRequested audit mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only event -> safe AuditRecord mapping.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not persist.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 293 — Implement idempotent ApprovalRequested audit consumer

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Implement only inbox + append AuditRecord transaction.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not update Approvals or Notifications.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 294 — Create ApprovalGranted audit trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only trigger binding/delegation for ApprovalGranted.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not persist in trigger.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 295 — Implement ApprovalGranted audit mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only event -> AuditRecord mapping.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not persist.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 296 — Implement idempotent ApprovalGranted audit consumer

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only inbox + append transaction.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not modify business state.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 297 — Create ApprovalRejected audit trigger shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only trigger binding/delegation for ApprovalRejected.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not persist in trigger.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 298 — Implement ApprovalRejected audit mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only event -> AuditRecord mapping.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not persist.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 299 — Implement idempotent ApprovalRejected audit consumer

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only inbox + append transaction.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not modify business state.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

## Prompt 300 — Add audit append-only integration test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-008, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving audit records cannot be updated/deleted through normal repository/application operations.

CONSTRAINT: Audit is durable business/security evidence and is separate from logs/traces.

RESTRICTION: Do not implement retention purge.

USAGE: Read audit/messaging/outbox-inbox/database rules and `add-audit-record` skill.

BEHAVIOR: Run focused model/consumer integration test and STOP.
```

# Part 14 — Commercial read model

## Prompt 301 — Create CommercialSummary entity/read-model row

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tenant/project-scoped read-model shape for invoice summary, hours consumed and retainer remaining with source and freshness timestamps.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not create accounting write behavior.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 302 — Add CommercialSummary EF configuration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only EF mapping/indexes for commercial read model.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not add DbSet/migration.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 303 — Create CommercialDbContext

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Commercial DbContext shell.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not register model.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 304 — Add CommercialSummary DbSet

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Register only commercial summary.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not generate migration.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 305 — Create CommercialSummary schema migration

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Generate only commercial read-model schema.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not add external integrations.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 306 — Define commercial summary response contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only portal-facing DTO for invoices/hours/retainer summary and as-of timestamp.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not expose source-system internals.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 307 — Implement commercial summary repository query

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tenant/project-scoped read query.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not add facade/controller.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 308 — Implement commercial summary business mapping

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only mapping/freshness semantics.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not write accounting data.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 309 — Implement commercial summary facade operation

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only authorization/orchestration.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not add endpoint.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 310 — Add commercial summary HTTP endpoint

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only GET endpoint for authorized project summary.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not implement invoice payment/accounting commands.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

## Prompt 311 — Add commercial summary tenant-isolation test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only cross-tenant denial test.

CONSTRAINT: Project Loop is a consumer/read model for commercial data, not the accounting source of truth.

RESTRICTION: Do not add dashboard UI.

USAGE: Read architecture/dotnet/database/multi-tenancy rules and endpoint skill.

BEHAVIOR: Run focused verification and STOP.
```

# Part 15 — Portal dashboard composition

## Prompt 312 — Define Angular project-health API client contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only typed Angular client method/model for the existing project-health HTTP endpoint.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not build component UI.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 313 — Implement Angular project-health API client

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only HTTP client implementation for the typed contract.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not cache or compose dashboard.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 314 — Create dashboard route component shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only lazy dashboard route component using design-system page primitives.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not fetch data yet.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 315 — Add project-health dashboard state

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only signal-based loading/success/error state for project-health call.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not add milestones/commercial/docs.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 316 — Render project-health dashboard card

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only one design-system-composed health surface from existing state.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not recreate design-system cards.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 317 — Define Angular milestones API client contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only typed client models/method for milestones endpoint.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not render.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 318 — Implement Angular milestones API client

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only HTTP implementation.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not alter dashboard layout.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 319 — Add milestone dashboard state

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only state/loading/error handling for milestones.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not render commercial/documents.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 320 — Render upcoming milestones dashboard section

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only milestone display using design-system components.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not implement milestone mutation.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 321 — Define Angular commercial-summary API client contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only typed client contract.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not render.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 322 — Implement Angular commercial-summary API client

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only HTTP implementation.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not cache.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 323 — Add commercial-summary dashboard state

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only loading/success/error state.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not render documents.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 324 — Render commercial-summary dashboard section

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only invoice/hour/retainer summary UI using design-system primitives.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not add accounting actions.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 325 — Define Angular document-list API client contract

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only typed client contract for document catalog list.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not render.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 326 — Implement Angular document-list API client

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only HTTP implementation.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not create upload UI.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 327 — Create project documents route component shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only documents page shell under application layout.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not fetch/render rows yet.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 328 — Add document-list page state

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only signal state for document list filters/loading/error/results.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not add upload/download behavior.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 329 — Render document catalog list

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only metadata list/table/cards using design-system primitives.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not expose Blob URLs or implement download.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 330 — Wire authorized version download action

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only UI action that calls the existing API download endpoint for an allowed exact version.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not generate SAS links client-side.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 331 — Define Angular approval API client contracts

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only typed read/approve/reject API contracts.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not build page.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 332 — Implement Angular approval API client

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only HTTP implementation.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not add state/UI.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 333 — Create approval-review route component shell

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only approval-review page shell.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not fetch data.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 334 — Add approval-review page state

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only state for loading approval metadata and decision submission.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not render actions yet.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 335 — Render exact-version approval context

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only UI that clearly identifies the exact target/document version being reviewed.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not add approve/reject buttons yet.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 336 — Render approve action

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only approve control using design-system button/dialog primitives and existing state/client.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not add reject control.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 337 — Render reject action

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only reject control and optional comment capture using design-system primitives.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not add notification UI.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

## Prompt 338 — Add stale-version approval UI test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002, LOOP-003, LOOP-007, LOOP-011
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system. Authorized users can browse project documents by metadata such as project, category, status and visibility without bypassing tenant/project authorization. Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. The dashboard can display externally sourced invoice, hours-consumed and retainer summary data without turning Project Loop into the system of record for accounting.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only component/e2e test proving the page displays the exact version from ApprovalRequest rather than assuming current document version.

CONSTRAINT: Angular 22 standalone/signals conventions and local Project Loop design system are mandatory; server remains authorization authority.

RESTRICTION: Do not change backend behavior.

USAGE: Read angular/design-system/authorization rules and relevant Angular skill.

BEHAVIOR: Run focused Angular unit/component/e2e test and STOP.
```

# Part 16 — Gateway and browser authentication seams

## Prompt 339 — Add Gateway route for Identity API

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only YARP route/cluster configuration for Identity API.

CONSTRAINT: YARP is the only browser-facing backend edge and downstream services still enforce authorization.

RESTRICTION: Do not route any other service.

USAGE: Read api-design/identity/authorization/security rules.

BEHAVIOR: Run targeted gateway integration tests and STOP.
```

## Prompt 340 — Add Gateway route for Engagement API

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only route/cluster for Engagement API.

CONSTRAINT: YARP is the only browser-facing backend edge and downstream services still enforce authorization.

RESTRICTION: Do not modify Identity route behavior.

USAGE: Read api-design/identity/authorization/security rules.

BEHAVIOR: Run targeted gateway integration tests and STOP.
```

## Prompt 341 — Add Gateway route for Documents API

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only route/cluster for Documents API.

CONSTRAINT: YARP is the only browser-facing backend edge and downstream services still enforce authorization.

RESTRICTION: Do not add Blob direct routing.

USAGE: Read api-design/identity/authorization/security rules.

BEHAVIOR: Run targeted gateway integration tests and STOP.
```

## Prompt 342 — Add Gateway route for Approvals API

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only route/cluster for Approvals API.

CONSTRAINT: YARP is the only browser-facing backend edge and downstream services still enforce authorization.

RESTRICTION: Do not add notifications/audit public routes.

USAGE: Read api-design/identity/authorization/security rules.

BEHAVIOR: Run targeted gateway integration tests and STOP.
```

## Prompt 343 — Add Gateway route for Commercial API

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only route/cluster for Commercial API.

CONSTRAINT: YARP is the only browser-facing backend edge and downstream services still enforce authorization.

RESTRICTION: Do not expose service database/internals.

USAGE: Read api-design/identity/authorization/security rules.

BEHAVIOR: Run targeted gateway integration tests and STOP.
```

## Prompt 344 — Configure browser authentication transport

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only the approved ASP.NET Core Identity browser authentication/session transport at the api-design/Identity seam according to ADR.

CONSTRAINT: YARP is the only browser-facing backend edge and downstream services still enforce authorization.

RESTRICTION: Do not implement registration/invitation UI.

USAGE: Read api-design/identity/authorization/security rules.

BEHAVIOR: Run targeted gateway integration tests and STOP.
```

## Prompt 345 — Configure gateway authorization fallback policy

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only secure-by-default authorization policy for protected routes.

CONSTRAINT: YARP is the only browser-facing backend edge and downstream services still enforce authorization.

RESTRICTION: Do not add feature-specific policies.

USAGE: Read api-design/identity/authorization/security rules.

BEHAVIOR: Run targeted gateway integration tests and STOP.
```

## Prompt 346 — Add 401 versus 403 gateway integration tests

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tests proving unauthenticated requests receive 401/redirect semantics per ADR and authenticated unauthorized requests receive 403 semantics.

CONSTRAINT: YARP is the only browser-facing backend edge and downstream services still enforce authorization.

RESTRICTION: Do not add tenant tests already covered elsewhere.

USAGE: Read api-design/identity/authorization/security rules.

BEHAVIOR: Run targeted gateway integration tests and STOP.
```

# Part 17 — Observability and trace continuity

## Prompt 347 — Add ServiceDefaults OpenTelemetry baseline

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only standard tracing/metrics/log correlation registration in ServiceDefaults.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not add custom domain spans.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 348 — Reference ServiceDefaults from Gateway

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only ServiceDefaults reference/bootstrap to Gateway.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not change routes.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 349 — Reference ServiceDefaults from Identity API

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only common observability bootstrap to Identity API.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not add custom metrics.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 350 — Reference ServiceDefaults from Engagement API

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only common observability bootstrap to Engagement API.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not add custom metrics.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 351 — Reference ServiceDefaults from Documents API

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only common observability bootstrap to Documents API.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not add custom metrics.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 352 — Reference ServiceDefaults from Approvals API

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only common observability bootstrap to Approvals API.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not add custom metrics.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 353 — Reference ServiceDefaults from Notifications Functions

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only common observability bootstrap to Notifications Functions.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not change consumers.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 354 — Reference ServiceDefaults from Engagement Functions

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only common observability bootstrap to Engagement Functions.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not change consumers.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 355 — Reference ServiceDefaults from Approvals Functions

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only common observability bootstrap to Approvals Functions.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not change consumers.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 356 — Reference ServiceDefaults from Audit Functions

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only common observability bootstrap to Audit Functions.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not change consumers.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 357 — Propagate correlation/trace context into outbox envelope

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only capture/persistence of current trace/correlation/causation metadata when creating an outbox event.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not change broker topology.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 358 — Restore parent trace context in Service Bus consumer

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only consumer-side extraction/restoration of W3C trace context from integration envelope/properties.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not alter business handling.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

## Prompt 359 — Add end-to-end approval trace test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only observability test/fixture proving one approval request/decision can be correlated across HTTP -> SQL/outbox -> Service Bus -> consumer spans.

CONSTRAINT: Distributed tracing must survive sync and async boundaries without placing secrets/PII in telemetry.

RESTRICTION: Do not introduce new business features.

USAGE: Read observability/messaging/security rules and trace skill.

BEHAVIOR: Run focused tracing/telemetry verification and STOP.
```

# Part 18 — Security and reliability quality gates

## Prompt 360 — Add document filename/path traversal security tests

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tests proving submitted display filenames cannot influence Blob object paths or escape configured storage namespace.

CONSTRAINT: Security/reliability tests must target one invariant at a time and preserve the existing architecture.

RESTRICTION: Do not change upload UX.

USAGE: Read security/testing plus relevant domain rule.

BEHAVIOR: Run only the named focused test/gate and STOP.
```

## Prompt 361 — Add raw Blob URL leakage test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-004, LOOP-006
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Document binaries are stored in Azure Blob Storage while SQL owns document metadata, lifecycle and Blob object references. Blob addressing must not become an authorization mechanism. Only explicitly published, client-visible document versions are exposed to authorized client users; server-side authorization precedes binary access.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only API contract tests proving document list/detail/upload responses contain no raw Blob connection strings, credentials or durable public URLs.

CONSTRAINT: Security/reliability tests must target one invariant at a time and preserve the existing architecture.

RESTRICTION: Do not add SAS support.

USAGE: Read security/testing plus relevant domain rule.

BEHAVIOR: Run only the named focused test/gate and STOP.
```

## Prompt 362 — Add approval actor spoofing test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-007, LOOP-008
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval requests target an exact resource identity and, for documents, an exact immutable document version. Approval transitions are authenticated and authorization checked. Approval decisions are immutable business records with actor, decision, target, timestamps and audit context; prior decision evidence is not destructively rewritten.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving approver identity is derived from authenticated principal rather than request body.

CONSTRAINT: Security/reliability tests must target one invariant at a time and preserve the existing architecture.

RESTRICTION: Do not add new roles.

USAGE: Read security/testing plus relevant domain rule.

BEHAVIOR: Run only the named focused test/gate and STOP.
```

## Prompt 363 — Add tenant-ID spoofing endpoint tests

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-001
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Authenticated users may access only tenants and projects for which server-side membership grants access. Client-supplied tenant identifiers are never sufficient authorization.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only tests proving request/body/query TenantId cannot override server-resolved tenant context.

CONSTRAINT: Security/reliability tests must target one invariant at a time and preserve the existing architecture.

RESTRICTION: Do not add new tenant behavior.

USAGE: Read security/testing plus relevant domain rule.

BEHAVIOR: Run only the named focused test/gate and STOP.
```

## Prompt 364 — Add outbox retry idempotency test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving a publish failure followed by retry does not corrupt outbox state and remains safe for at-least-once delivery.

CONSTRAINT: Security/reliability tests must target one invariant at a time and preserve the existing architecture.

RESTRICTION: Do not change business events.

USAGE: Read security/testing plus relevant domain rule.

BEHAVIOR: Run only the named focused test/gate and STOP.
```

## Prompt 365 — Add inbox duplicate message concurrency test

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-010, LOOP-012
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Approval outcomes may propagate to Engagement/milestone state asynchronously through idempotent event consumers. HTTP, SQL, Service Bus, Functions and downstream operations preserve distributed trace/correlation context so user actions and asynchronous propagation can be followed end to end.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test for concurrent duplicate delivery against a durable consumer to prove unique-message/inbox handling prevents duplicate side effects.

CONSTRAINT: Security/reliability tests must target one invariant at a time and preserve the existing architecture.

RESTRICTION: Do not change broker retry policy.

USAGE: Read security/testing plus relevant domain rule.

BEHAVIOR: Run only the named focused test/gate and STOP.
```

## Prompt 366 — Add Redis outage correctness test

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only test proving a representative cached read remains functionally correct when Redis is unavailable.

CONSTRAINT: Security/reliability tests must target one invariant at a time and preserve the existing architecture.

RESTRICTION: Do not add new cache features.

USAGE: Read security/testing plus relevant domain rule.

BEHAVIOR: Run only the named focused test/gate and STOP.
```

## Prompt 367 — Run architecture dependency test gate

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add/run only automated architecture tests enforcing host -> Core layering, no cross-service persistence references and no API/Functions inversion.

CONSTRAINT: Security/reliability tests must target one invariant at a time and preserve the existing architecture.

RESTRICTION: Do not refactor unrelated code unless a violation is directly exposed and scoped.

USAGE: Read security/testing plus relevant domain rule.

BEHAVIOR: Run only the named focused test/gate and STOP.
```

## Prompt 368 — Run design-system conformance test gate

```text
REQUIREMENTS:
  TRACEABILITY: LOOP-002
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: The portal exposes project health and project-facing status information through authorized, tenant-scoped APIs and an Angular dashboard that uses the local design system.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add/run only checks proving feature UI uses local design-system shell/primitives and does not duplicate application shell.

CONSTRAINT: Security/reliability tests must target one invariant at a time and preserve the existing architecture.

RESTRICTION: Do not redesign UX.

USAGE: Read security/testing plus relevant domain rule.

BEHAVIOR: Run only the named focused test/gate and STOP.
```

# Part 19 — Deployment and final product-completeness gates

## Prompt 369 — Add AppHost project references for Gateway and service hosts

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Aspire project resources/references for existing API/Functions hosts.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not add cloud deployment resources.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 370 — Add service-to-SQL Aspire references one service at a time

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only database resource/reference wiring for the next owning service according to database-per-service rule.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not share a database across services; execute this prompt once per service.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 371 — Add Documents-to-Blob Aspire reference

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Blob resource reference/configuration to Documents host.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not expose Blob to browser.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 372 — Add Functions-to-ServiceBus Aspire references

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Service Bus resource references required by existing Functions hosts.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not alter broker topology.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 373 — Add Redis reference to the first approved cache consumer

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Redis resource wiring for one existing cache consumer after a concrete cache use case exists.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not cache authoritative approval/document authorization state.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 374 — Add production configuration key inventory

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Document only the required production configuration/secret names and owning components.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not place secret values in repository.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 375 — Add production health checks for SQL

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only SQL dependency health checks for existing service-owned databases.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not add Service Bus/Blob checks in same prompt.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 376 — Add production health check for Blob Storage

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only Documents Blob dependency health check.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not add SQL/Service Bus checks.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 377 — Add production health check for Service Bus

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Add only messaging dependency health check appropriate to publisher/consumer hosts.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not change retry policy.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 378 — Run complete backend build/test gate

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Run only repository-defined backend build and test commands; fix only failures caused by the completed Project Loop implementation if the fixes remain within existing architecture.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not add features or refactor for style.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 379 — Run complete Angular build/test gate

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Run only Angular lint/typecheck/test/build/e2e commands defined by repository; fix only implementation defects.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not redesign UI.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 380 — Run final requirement traceability audit

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Compare LOOP-001..LOOP-012 against implemented tests/docs/code and report coverage gaps only.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not implement missing gaps in this prompt.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 381 — Run final architecture drift audit

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Compare code against CLAUDE.md, ADRs, rules and service-boundary constraints and report violations only.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not refactor in this prompt.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

## Prompt 382 — Run final document/approval invariants audit

```text
REQUIREMENTS:
  TRACEABILITY: GOV
  REQUIREMENT LINKS: [requirements](../requirements/requirements.md); [matrix](../requirements/requirements-matrix.md)
  REQUIREMENT INTENT: Project Loop implementation must remain traceable to the approved architecture and requirements. Missing or conflicting decisions are resolved explicitly rather than invented as implementation side effects.
  SOURCE OF TRUTH: Read the linked requirements before coding. If this prompt conflicts with the canonical requirement text or an approved ADR, STOP and report the drift.

SCOPE: Inspect implementation/tests specifically for Blob authorization, immutable versions, exact-version approvals and append-only decisions.

CONSTRAINT: Final gates verify the architecture already built; they do not become permission for opportunistic changes.

RESTRICTION: Do not change code.

USAGE: Read testing/deployment/architecture rules and `run-quality-gate` skill.

BEHAVIOR: Run/report exactly the named gate and STOP.
```

---

# Sequence summary

This library contains **383 atomic prompts** numbered `000` through `382`. Run them in order unless an earlier prompt explicitly establishes that the seam already exists and passes its verification. When skipping an already-satisfied prompt, record the evidence and do not silently assume downstream compatibility.

The sequence deliberately separates:

- entity from mapping from migration;
- repository from business from facade from controller;
- event contract from outbox persistence from relay from consumer;
- trigger binding from consumer business behavior;
- Blob storage mechanics from SQL metadata transactions;
- approval request creation from notification and milestone propagation;
- Angular API contracts from state from rendering;
- quality-gate discovery from implementation.

That separation is the core atomicity rule for Project Loop SCRUB execution.
