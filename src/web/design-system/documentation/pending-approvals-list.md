# Pending approvals list

## Purpose

`PendingApprovalsListComponent` presents a compact list of caller-authorized, caller-sorted pending approval requests with target identity, applicable exact version, requester, due text, and native review navigation. It does not approve, reject, authorize, sort, or infer urgency.

## API and states

- Required readonly `items` are rendered unchanged in caller order.
- Each item has a stable ID, discriminated target, requester, caller-owned due presentation, and authorized review destination.
- Document targets require an exact `versionLabel`; other target types cannot supply a version.
- Due presentation requires visible text and an existing Badge variant, with optional machine-readable `dateTime`. Any “Overdue” meaning must be supplied explicitly by the caller.
- `label`, `emptyMessage`, and `reviewLabel` customize visible or accessible copy.

```html
<lsd-pending-approvals-list
  label="My pending approvals"
  [items]="authorizedSortedRequests" />
```

## Accessibility and responsive behavior

Native list semantics identify the collection. Target type, target name, exact document version, requester, and due meaning are visible text. Dates may use native `time[datetime]`, and review navigation retains native anchor behavior through Link. Rows wrap into two columns at narrow widths without changing DOM or caller order.

## Do / don't

Do supply only authorized items in the intended order, exact document versions, explicit due labels, and authorized destinations. Don't calculate urgency, sort records, approve inline, or infer access in this recipe.

## Public import

```ts
import {
  PendingApprovalsListComponent,
  type PendingApprovalItem,
} from 'src/web/design-system/public-api';
```
