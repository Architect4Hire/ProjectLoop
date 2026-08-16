# Project Loop

Project memory written as a SCRUB-style engineering constitution — Scope, Constraints, Restrictions, Usage, Behavior.

This file is loaded for every Claude Code session. Keep enduring architecture rules here. Put path-specific rules in `.claude/rules/`, repeatable procedures in `.claude/skills/`, review work in `.claude/agents/`, and deterministic safeguards in `.claude/hooks/`.

## Scope
- Project Loop is a **multi-tenant consulting client portal**.
- The system exposes project health, milestones, meetings, decisions, deliverables, documents, approvals, commercial summaries, notifications, and audit history.
- The backend is a distributed .NET 10 solution composed with .NET Aspire.
- The frontend is Angular 22 + TypeScript and SHALL use the local Project Loop design system.
- YARP is the only browser-facing API gateway.
- ASP.NET Core Identity is the application identity system.
- Azure SQL / SQL Server is the durable relational store; each bounded service owns its database.
- Azure Blob Storage stores document binaries; SQL stores document metadata and lifecycle.
- Redis is shared infrastructure but never authoritative storage.
- Services use synchronous HTTP for immediate interactions and Azure Service Bus for temporal decoupling, cross-domain state propagation, fan-out, retryable work, and long-lived workflows.
- Bounded-domain ownership is architecture, not scaffolding. Do not create or split services as a side effect of a feature task.

## Constraints

### Runtime and orchestration
- .NET 10 is the backend target.
- Aspire is the local composition/orchestration model.
- AppHost declares deployable projects and infrastructure dependencies only.
- Do not put business logic in AppHost.

### Service shape and internal layering
Preserve this onion direction unless an ADR explicitly changes it:

```text
HTTP Controller ─┐
                 ├─> Facade -> Business -> Data -> Repository -> DbContext
Async Trigger ───┘
```

Responsibilities:
- **Controller / async trigger**: transport binding, auth context, serialization, correlation, delegation, response/settlement. No business rules.
- **Facade**: use-case validation, orchestration at the service boundary, tenant and authorization policy calls, cache use/invalidation, delegation to Business.
- **Business**: domain decisions, state transitions, model translation, decisions about integration facts. No EF queries, Redis calls, direct HTTP calls, or Service Bus publishing.
- **Data**: transaction boundaries, repository composition, durable persistence, outbox persistence, inbox/idempotency state when used.
- **Repository**: service-owned persistence operations only.
- **DbContext**: EF Core mapping/unit of work for the owning service database only.

### Domain boundaries
- Each bounded service owns its API, data model, database, cache namespace, emitted events, and consumed messages.
- Services SHALL NOT read another service database or reference another service's internals.
- Shared libraries contain cross-cutting mechanisms and stable contracts only, never shared domain logic.
- Integration contracts are versionable and SHALL NOT serialize EF entities.

### Multi-tenancy
- Tenant isolation is a system invariant.
- Every tenant-owned resource SHALL carry a TenantId or equivalent ownership key.
- TenantId supplied by the client SHALL NOT establish authorization.
- Tenant context SHALL be derived from authenticated memberships/claims validated server-side.
- All tenant-owned queries SHALL enforce tenant scoping.
- Cache keys, blob keys, events, logs, and audit records SHALL carry sufficient tenant context without leaking cross-tenant data.
- Cross-tenant admin capabilities require explicit privileged policies and audit records.

### Identity
- ASP.NET Core Identity is authoritative for application users, credentials, roles, claims, tokens, email verification, recovery, and external-login associations.
- Business domains reference users by stable identifiers and SHALL NOT depend on Identity EF models.
- External client users may authenticate with local Identity credentials. External providers may be added through ASP.NET Core Identity without changing business authorization semantics.
- Invitation and membership are business/application concerns; possession of an account alone does not grant tenant access.

### Authorization
- Authorization is enforced server-side at the owning domain.
- Document/content authorization occurs before binary access is granted.
- Client UI guards improve UX but are never security boundaries.
- Authorization decisions SHALL consider user, tenant membership, project membership, role/policy, resource visibility, and resource state when relevant.

### HTTP integration
Use synchronous HTTP when the caller requires an immediate answer.

Use HTTP for:
- domain queries;
- authorization-sensitive lookups;
- immediate validation;
- request/response operations;
- short commands that fit normal request budgets.

Rules:
- typed `HttpClient` clients only;
- service URLs from Aspire/configuration/service discovery;
- propagate cancellation, tracing, correlation, and auth context intentionally;
- define timeouts;
- retry only safe operations;
- use idempotency keys for retried commands where duplicates matter;
- avoid deep synchronous call chains;
- never treat HTTP success as atomic with another service's local transaction.

### Messaging and long-lived workflows
Azure Service Bus is used when temporal decoupling is required.

Use asynchronous messaging for:
- cross-domain state propagation;
- fan-out;
- independent subscribers;
- independent retry;
- work that must survive caller disconnects;
- notification delivery;
- durable workflow progression;
- eventual-consistency projections.

Integration events describe completed facts in past tense.
Do not replace simple queries with request/reply messaging.
Long-lived workflows SHALL have durable workflow state.

### Transactional outbox
A service that atomically persists business state and publishes an integration event SHALL use a transactional outbox.
- state and outbox commit in one local transaction;
- application/domain layers do not publish the transactional event directly;
- an outbox relay publishes persisted messages;
- never mark dispatched before broker acknowledgement;
- relay retries are idempotent and observable.

### Idempotency and transactional inbox
Every asynchronous consumer SHALL be idempotent.
A transactional inbox SHOULD be used when message processing mutates durable state and duplicate application could produce incorrect state or side effects.
Redis alone is not durable proof of message processing.

### Documents
Documents are first-class domain resources, not arbitrary attachments.

SQL owns:
- document identity and metadata;
- classification/type;
- tenant/project relationships;
- visibility;
- lifecycle/publication state;
- version relationships;
- hashes and storage references;
- audit relationships.

Blob Storage owns binary content.

Rules:
- raw blob URLs are never authorization boundaries;
- blob object keys SHALL be opaque and tenant-aware in organization without exposing sensitive names;
- document access is authorized before stream/download access is issued;
- uploads have explicit size/type policies;
- content hashes are recorded;
- malware scanning/quarantine is part of the upload lifecycle design;
- deleted/superseded content follows documented retention policy.

### Document versions
- A published or approved version is immutable.
- Replacing content creates a new version.
- Approval is always against a specific version.
- Approval of v3 does not imply approval of v4.
- CurrentVersion is a pointer, not a mutable binary slot.

### Approvals
Approvals are immutable business records.
An approval SHALL capture:
- approval/request identifier;
- tenant;
- target resource and target version;
- approver identity;
- decision;
- timestamp UTC;
- optional comments;
- relevant audit/correlation metadata.

Approval history SHALL NOT be destructively rewritten.
Approval state transitions SHALL be validated in domain logic.
This system provides authenticated business approvals, not legal electronic signatures unless a future ADR introduces e-signature integration.

### Audit
Application logs are not the authoritative business audit trail.
Business-significant and security-sensitive actions create explicit audit records.
Audit records SHOULD be append-only from the application's perspective.

### Redis
Redis is an optimization or coordination mechanism, not a source of truth.
- cache keys are namespaced by owning domain and version;
- every entry has an intentional TTL unless an ADR says otherwise;
- invalidation is designed with writes;
- correctness survives eviction;
- services do not inspect another domain's cache values.

### Angular 22
Preferred patterns:
- standalone components/directives/pipes;
- signals for local/view state;
- `computed()` for derived state;
- `effect()` only for true side effects;
- typed reactive forms;
- route-level lazy loading;
- functional providers/interceptors/guards where clear;
- `OnPush` and zoneless-compatible code;
- built-in control flow (`@if`, `@for`, `@switch`);
- `@defer` for deliberate deferred UI;
- strict TypeScript;
- accessible semantic HTML.

Do not introduce React conventions, JSX, hooks, Redux, Next.js, or React-specific state vocabulary.

### Design system
The UI source of truth is `src/web/design-system/`.
Feature code composes design-system primitives and recipes rather than repeating large Tailwind utility bundles.
The design system owns tokens, typography, spacing, elevation, radii, Tailwind recipes, form controls, navigation, page shells, states, dialogs, drawers, notifications, tables, document UI, approval UI, and timeline/audit patterns.

### Observability
A single operation must be traceable across Angular, YARP, service HTTP calls, SQL, Redis, Blob Storage, outbox, Azure Service Bus, inbox/consumer, and downstream notifications.
Carry correlation IDs and trace context intentionally.
Do not log secrets, credentials, SAS tokens, or document bodies.

## Restrictions
- Do not invent service boundaries during feature implementation.
- Do not use a shared database to simplify cross-domain reads.
- Do not use Service Bus for ordinary request/response queries.
- Do not publish transactional events directly from controllers/business logic.
- Do not use Redis as authoritative storage.
- Do not expose Blob Storage directly without application authorization.
- Do not mutate approved/published document versions.
- Do not infer tenant access from a route/body TenantId.
- Do not duplicate design-system components in feature code.
- Do not introduce legal e-signature semantics without an ADR.

## Usage
Before implementation:
1. Read this file.
2. Read relevant `.claude/rules/` files.
3. Read relevant ADRs and design docs.
4. Use the matching `.claude/skills/` procedure.
5. Preserve existing architectural intent; do not create a competing pattern.

After implementation:
1. run tests and formatting;
2. verify tenant boundary and authorization behavior;
3. verify observability/correlation;
4. verify event idempotency when messaging is used;
5. run the appropriate review agent/quality gate.

## Behavior
- Prefer the simplest design that preserves domain ownership and system invariants.
- Explain architecture-impacting tradeoffs before changing established patterns.
- Treat docs and ADRs as executable architectural constraints, not optional commentary.
- When implementation reveals an architectural gap, update or add an ADR before normalizing the new pattern.
