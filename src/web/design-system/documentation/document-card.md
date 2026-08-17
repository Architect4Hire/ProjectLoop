# Document card

## Purpose

`DocumentCardComponent` summarizes one caller-authorized document using display-ready title, category, status, visibility, exact version, updated metadata, and projected actions. It composes Surface, Badge, and Version Chip. It never downloads, publishes, approves, authorizes, or mutates a document.

## API and states

Required `document` is a `DocumentCardViewModel` with a stable ID and:

- `title` and `category` as display-ready text.
- `status` and `visibility` as visible labels paired with public Badge variants. Callers own their vocabulary and meaning.
- `version.label` as the exact version identifier. Optional qualifier data is passed unchanged to Version Chip and describes only that version.
- `updated.label` as localized display text and optional `updated.dateTime` as its machine-readable value.
- `headingLevel` accepts 2–4 and defaults to 3.
- Content marked `lsdDocumentCardActions` is projected into a labeled action group. The caller owns authorization and behavior.

```html
<lsd-document-card [document]="authorizedDocument">
  <a lsdLink lsdDocumentCardActions [href]="authorizedDocument.detailsUrl">View details</a>
</lsd-document-card>
```

## Accessibility

The card is an article named by its visible title. Category, status, visibility, exact version, qualifier, and updated text remain visible; confidential or restricted meaning never relies on color. A machine-readable timestamp uses native `time`. Projected actions retain their own native link or button behavior.

## Responsive behavior

Long titles, categories, and labels wrap without changing content. At narrow widths the header, actions, and metadata stack while preserving DOM and reading order: identity, actions, status and visibility, then exact version and update metadata.

## Do / don't

Do pass only authorized, display-ready metadata and already-authorized actions. Do preserve exact version identifiers and explicit visibility labels. Don't fetch, download, publish, approve, infer permissions, or resolve workflow state in the recipe.

## Visual coverage

Unit coverage exercises long document names, explicit confidential visibility, exact version composition, projected actions, and narrow-layout hooks. Workspace visual coverage should capture long-label and narrow-width presentations in both appearances.

## Public import

```ts
import { DocumentCardComponent, type DocumentCardViewModel } from 'src/web/design-system/public-api';
```
