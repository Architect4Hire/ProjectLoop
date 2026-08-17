# Version-bound approval

## Purpose

`VersionBoundApprovalComponent` composes Approval Request Banner, Approval Comment Field, Version Chip, and Approval Actions around one exact document version. It keeps the requested target/version visible beside decision controls while the caller owns authorization, validation, persistence, and workflow state.

## API and states

- Required `target` is a document target with an exact, non-empty `versionLabel`; the banner validates that contract.
- `requestStatus`, `requester`, and `requestedTime` populate the request banner.
- `provenance`, `reviewStatus`, `processing`, warnings, and disabled inputs pass caller-owned state to Approval Actions.
- `comment` is controlled model state. Its requirement, maximum count, help, and error are caller inputs.
- Optional `currentVersion` is presented separately as Current and never replaces the requested version.
- `decisionIntent` emits `approve | reject | request-change` together with the exact requested target and current comment. It performs no persistence.

```html
<lsd-version-bound-approval
  id="plan-review"
  [target]="requestedV3"
  [currentVersion]="{ label: 'v4' }"
  [requestStatus]="approvalStatusPresentation('requested')"
  [requester]="requester"
  [requestedTime]="requestedTime"
  [provenance]="provenance"
  [(comment)]="comment"
  (decisionIntent)="persistAuthorizedDecision($event)" />
```

## Accessibility and responsive behavior

The outer region is labeled with the target name and requested version. The banner repeats exact-version context in the primary summary; Approval Actions repeats it beside the controls. A different current version is explicitly labeled and qualified. At narrow widths the context stacks without changing its DOM or reading order.

## Relationship to Review Approval

Use this pattern to record a decision against an already identified document version. Use generic `ReviewApprovalComponent` when users must compare current and proposed content. This pattern deliberately provides no content-comparison slots or behavior.

## Do / don't

Do filter authorization and persist emitted intents in the caller. Do verify the emitted target version server-side. Don't replace the requested version with the current version, infer permissions, or duplicate content-comparison behavior here.

## Public import

```ts
import {
  VersionBoundApprovalComponent,
  type VersionBoundApprovalIntent,
} from 'src/web/design-system/public-api';
```
