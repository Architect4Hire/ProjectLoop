# Document row

## Purpose

`DocumentRowComponent` is the native-table-row equivalent of Document Card. It presents the same caller-authorized, display-ready title, category, status, visibility, exact version, updated metadata, and projected actions in a compact scanning layout. It composes Badge and Version Chip and adds no sorting, selection, download, publication, or approval behavior.

## API and states

- Apply `lsdDocumentRow` to a native `tr` inside a caller-owned semantic table.
- Required `document` uses the same `DocumentCardViewModel` as Document Card, preserving every field and the exact version label.
- `actionsLabel` defaults to “Document actions.” Content marked `lsdDocumentRowActions` is projected into the final cell.
- Status, visibility, version qualification, timestamps, action authorization, and all display wording remain caller-owned.

```html
<table>
  <thead>
    <tr><th>Title</th><th>Category</th><th>Status</th><th>Visibility</th><th>Version</th><th>Updated</th><th>Actions</th></tr>
  </thead>
  <tbody>
    <tr lsdDocumentRow [document]="authorizedDocument">
      <a lsdLink lsdDocumentRowActions [href]="authorizedDocument.detailsUrl">View details</a>
    </tr>
  </tbody>
</table>
```

## Accessibility

The title is a native `th` with `scope="row"`, so it labels the document row in conjunction with the caller's column headers. Status, visibility, exact version, qualifier, and updated metadata remain visible text. Projected actions are grouped under a contextual accessible label containing the document title. The recipe adds no row click, checkbox, `aria-selected`, or sorting semantics.

## Responsive behavior

Cells wrap long display-ready content while preserving native table and DOM order. The caller should place the table in the Data Table-style named horizontal overflow region at narrow widths; the row does not hide or reorder fields.

## Do / don't

Do use the same authorized model supplied to Document Card, provide matching column headers, and project native links or buttons. Don't place the component outside a table, add interaction to the whole row, or ask it to sort, select, download, publish, approve, or resolve document state.

## Visual coverage

Unit coverage verifies native row-header scope, contextual action labeling, all semantic fields, exact Version Chip composition, and the absence of sorting and selection controls. Narrow visual coverage should use a horizontally scrollable semantic table without hiding columns.

## Public import

```ts
import { DocumentRowComponent, type DocumentCardViewModel } from 'src/web/design-system/public-api';
```
