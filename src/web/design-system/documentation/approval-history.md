# Approval history

## Purpose

`ApprovalHistoryComponent` presents append-oriented approval evidence through Activity Stream. Every caller-supplied decision retains its actor, UTC instant and display text, comment summary, target identity, and exact version without edit, delete, or grouping behavior.

## API and states

- Required readonly `entries` are rendered in exactly the supplied chronological order; the component does not sort, reverse, group, or mutate them.
- Each entry requires a stable `id`, decision label, actor, ISO `occurredAt`, caller-formatted `timestampLabel`, comment summary, target type/name, and exact `versionLabel`.
- Optional Activity Stream attribution defaults to `human-authored`.
- Accessible name and empty-state copy are caller-overridable.
- There are no action outputs or mutation controls.

```html
<lsd-approval-history id="document-approval-history" accessibleName="Document approval history" [entries]="chronologicalApprovalEvidence" />
```

## Accessibility and responsive behavior

Activity Stream provides a named ordered list, actor/decision headings, and native `time[datetime]`. Target type, name, and exact version are always present in the visible event line and never hidden by version grouping. Expanding native decision evidence reveals the same identity as a Version Chip beside the comment summary. Narrow layouts wrap evidence without changing DOM order.

## Immutable evidence

This is a presentation of caller-owned evidence, not an audit store. Repeated decisions for the same target remain separate ordered entries, and decisions across versions never collapse into one current identity. The component maps the readonly input to Activity Stream views without changing input records.

## Do / don't

Do supply stable IDs, absolute ISO timestamps, localized UTC-display text, and exact historical versions. Don't provide edit/delete affordances, infer missing versions, reorder history in CSS, or replace historical identity with the current version.

## Public import

```ts
import { ApprovalHistoryComponent, type ApprovalHistoryEntry } from 'src/web/design-system/public-api';
```
