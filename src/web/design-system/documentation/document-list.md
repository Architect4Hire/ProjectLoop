# Document list

## Purpose

`DocumentListComponent` responsively presents one caller-owned document collection as semantic Document Rows on wider screens and ordered Document Cards on narrow screens. It owns presentation only and does not filter, paginate, call APIs, select records, authorize data, or reorder the supplied collection.

## API and states

- Required `id` gives the empty State Feedback a stable identifier.
- Required immutable `documents` contains authorized, display-ready `DocumentCardViewModel` values shared by both recipes.
- `accessibleName` labels the collection and its desktop table caption.
- `emptyTitle` and optional `emptyDescription` provide caller-ready empty copy.
- Empty input renders one State Feedback region. One and many items use the same responsive structure.

```html
<lsd-document-list
  id="project-documents"
  accessibleName="Project documents"
  [documents]="authorizedDocuments"
  emptyTitle="No documents available" />
```

## Accessibility

The wide presentation is a native table whose Document Rows use scoped row headers. The narrow presentation is an ordered list of Document Cards. CSS ensures only the active presentation participates in layout and the accessibility tree. Both render the same visible category, title, status, visibility, exact Version Chip, and updated metadata. No row is clickable or selectable.

## Responsive behavior

At 48rem and wider, the semantic table supports cross-column scanning. Below 48rem, the table is removed from layout and an ordered card list is displayed. Both presentations iterate the same immutable array with the same tracking key, preserving caller-supplied item and DOM reading order without sorting or field transformation.

## Do / don't

Do filter, authorize, paginate, and format records before passing them in. Do preserve stable IDs and exact version labels. Don't use the pattern as a data source, selection model, paginator, filter control, or workflow controller.

## Visual coverage

Unit coverage exercises empty, one, many, and responsive presentation hooks, including identical information and item order in rows and cards. Narrow visual coverage should confirm the card list replaces rather than accompanies the table.

## Public import

```ts
import { DocumentListComponent } from 'src/web/design-system/public-api';
```
