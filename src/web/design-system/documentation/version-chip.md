# Version chip

## Purpose

`VersionChipComponent` compactly identifies one exact caller-supplied version and may qualify that same version as current, approved, or published. It does not resolve versions, approval, publication, or lifecycle state.

## API and states

- Required `versionLabel` is caller-formatted and always rendered as visible text without normalization.
- Optional `qualifier` is `current | approved | published`. It describes only the version label in the same chip and never implies that any other version is approved.
- Optional `qualifierLabel` replaces the default visible qualifier text for localization.
- With no qualifier, the chip uses the neutral Badge treatment. Current is informational, approved uses approved semantics, and published uses success semantics.

```html
<lsd-version-chip versionLabel="v2.7.0" qualifier="current" />
<lsd-version-chip versionLabel="2026.08" qualifier="published" qualifierLabel="Released" />
```

## Accessibility

The exact version and qualifier are ordinary visible text in the same Badge. The qualifier text, not color, communicates current, approved, or published state. The chip is noninteractive and does not enter keyboard focus or announce state changes.

## Responsive behavior

The small chip remains inline and constrains long caller-formatted labels with ellipsis when its container is narrow. Callers should retain the full version identifier in surrounding document metadata when truncation is possible.

## Do / don't

Do supply the exact display identifier and only a qualifier already resolved for that version. Do provide localized qualifier text where needed. Don't derive current or approval state in the component, use a qualifier for a different version, or treat the chip as an action.

## Public import

```ts
import { VersionChipComponent } from 'src/web/design-system/public-api';
```
