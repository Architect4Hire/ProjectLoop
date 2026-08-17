# Approval request banner

## Purpose

`ApprovalRequestBannerComponent` presents caller-supplied approval request context. It composes Alert Banner, Badge, and Version Chip without approving, rejecting, fetching, or changing the request.

## Public API

- `id`, `target`, `status`, `requester`, and `requestedTime` are required inputs.
- `target` is discriminated by `type`. A `document` target requires a non-empty exact `versionLabel`; other targets do not accept a version.
- `status` accepts an explicit label and an existing Badge variant, such as the result of `approvalStatusPresentation`.
- `requestedTime` contains visible caller-formatted text and may include a machine-readable `dateTime` value.
- `title` defaults to `Approval request`.
- The component has no action outputs or approve/reject controls.

```html
<lsd-approval-request-banner
  id="plan-approval"
  [target]="{
    type: 'document',
    typeLabel: 'Document',
    label: 'Delivery plan',
    versionLabel: 'v3'
  }"
  [status]="approvalStatusPresentation('requested')"
  requester="Morgan Lee"
  [requestedTime]="{ label: '17 August 2026 at 10:30 AM', dateTime: '2026-08-17T15:30:00Z' }" />
```

## Accessibility and responsive behavior

The Alert Banner supplies the labeled group structure. Status meaning and the exact version are visible text; neither relies on color. The requester and requested time use description-list semantics, and a supplied ISO time is rendered with `time[datetime]`. The banner wraps its summary and metadata at narrow widths without changing DOM or reading order.

## Do and don't

Do pass display-ready, authorized target and requester text. For document requests, pass the exact requested version rather than a generic current-version label. Do not place approve or reject behavior in this banner; compose a separate caller-authorized action surface where required.

## Public import

```ts
import {
  ApprovalRequestBannerComponent,
  type ApprovalRequestTarget,
} from 'src/web/design-system/public-api';
```
