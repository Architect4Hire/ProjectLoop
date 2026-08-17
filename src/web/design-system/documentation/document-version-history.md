# Document version history

## Purpose

`DocumentVersionHistoryComponent` presents caller-owned versions in chronological order with exact Version Chip identity, publication/approval/current qualification, actor and localized time text, and keyed action slots. It deliberately does not reuse Activity Stream because version identity must remain primary and qualifiers must stay structurally bound to their exact version. It does not sort, mutate, approve, publish, or resolve versions.

## API and states

- Required `id` provides stable list and heading relationships.
- Required immutable `versions` contains `DocumentHistoryVersion` values with stable ID, exact `versionLabel`, optional Version Chip qualifier and localized qualifier label, actor, machine-readable `occurredAt`, and visible `timestampLabel`.
- `accessibleName` labels the containing region and ordered list.
- `actionsLabel` prefixes each contextual action-group label.
- Key an `ng-template` with `lsdDocumentVersionActions="version-id"` to project actions only into that version. Its implicit context is the unchanged version, with `index` and `count` also available.

```html
<lsd-document-version-history id="history" [versions]="chronologicalVersions">
  <ng-template lsdDocumentVersionActions="v3" let-version>
    <a lsdLink [href]="version.detailsUrl">View {{ version.versionLabel }}</a>
  </ng-template>
</lsd-document-version-history>
```

## Accessibility

Native ordered-list and article semantics expose chronology. Every item is named by its Version Chip, whose exact label and qualifier are visible text. Actor text and native `time` metadata remain in the same item. Each action slot is a labeled group naming the exact version. Consequently, an approved v3 cannot visually or accessibly imply that current v4 is approved.

## Responsive behavior

The timeline preserves one DOM order at every width. Headers keep version identity and actions aligned on wider screens and stack them below 30rem without moving the qualifier away from its Version Chip. Long actor, time, qualifier, and action text wrap within their own version item.

## Do / don't

Do supply immutable versions in the chronology users should read and bind each action template to a stable version ID. Do keep approval, publication, and current qualification independently resolved by the caller. Don't sort or mutate history, infer approval from current status, reuse a generic event layout that obscures exact versions, or place one version's actions in another item.

## Visual coverage

Unit coverage explicitly verifies v3 Approved and v4 Current remain separate, chronological actor/time rendering, keyed action labels, and input immutability. Narrow visual coverage should include long localized timestamps and actions stacked beneath their own chips.

## Public import

```ts
import {
  DocumentVersionHistoryActionsDirective,
  DocumentVersionHistoryComponent,
} from 'src/web/design-system/public-api';
```
