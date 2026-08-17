# Audit event

## Purpose

`AuditEventComponent` presents one already-redacted audit event with actor, action, resource, UTC-display text, correlation identifier, and an optional safe summary slot. It does not accept raw-log, token, secret, or document-body fields.

## API and states

- Required inputs are stable `id`, `action`, resource type/label, UTC display and ISO time, and correlation identifier.
- A missing or blank `actor` renders explicit `Actor unavailable` text, customizable through `unavailableActorLabel`.
- `hasDetails` enables the labeled `[lsdAuditEventDetails]` projection for already-redacted summary metadata.
- `correlationCopyRequested` emits the complete identifier; the caller performs clipboard access and may announce its result.
- Resource type uses a neutral Badge and never implies event severity.

```html
<lsd-audit-event
  id="audit-event-42"
  [actor]="event.actor"
  [action]="event.action"
  [resource]="event.resource"
  [occurredAt]="event.utcPresentation"
  [correlationId]="event.correlationId"
  [hasDetails]="true"
  (correlationCopyRequested)="copyCorrelation($event)">
  <p lsdAuditEventDetails>{{ event.redactedSummary }}</p>
</lsd-audit-event>
```

## Accessibility and responsive behavior

The event is an article named by its action. The resource and missing-actor state remain visible text, and UTC data uses native `time[datetime]`. Long resource and correlation identifiers wrap. The copy button has an identifier-specific accessible name. At narrow widths the header and correlation controls stack without changing DOM order.

## Safe details boundary

Only project display-ready, already-redacted summary fields. Do not project document bodies, secrets, access or refresh tokens, credentials, authorization headers, stack traces, or raw log payloads. The component neither sanitizes nor fetches audit data; callers own redaction before presentation.

## Do / don't

Do retain the complete display-safe correlation identifier and absolute event time. Don't use this recipe as an audit store, infer an actor, or pass sensitive payloads through the details slot.

## Public import

```ts
import { AuditEventComponent, type AuditEventResource } from 'src/web/design-system/public-api';
```
