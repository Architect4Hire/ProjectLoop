# Document filters

## Purpose

`DocumentFiltersComponent` is a Project Loop document filter form for caller-supplied project, category, status, and visibility options. It composes Filter Action Bar, Select, and Button APIs and emits intent only. It does not fetch options, apply authorization, filter records, paginate results, or call an API.

## API and states

- Required `id` provides stable control and disclosure IDs.
- Required `projectOptions`, `categoryOptions`, `statusOptions`, and `visibilityOptions` are immutable display-ready `DocumentFilterOption` arrays supplied by the caller.
- `accessibleName` labels the filter region and defaults to “Document filters.”
- `filterChange` is the only intent output. It emits `DocumentFilterChangeIntent` with `source: 'submit' | 'reset'` and all four typed `string | null` filter values.
- Submit preserves current values. Reset clears all four controls before emitting. The caller decides whether and how to update results.

```html
<lsd-document-filters
  id="documents"
  [projectOptions]="authorizedProjects"
  [categoryOptions]="categories"
  [statusOptions]="statuses"
  [visibilityOptions]="authorizedVisibilityValues"
  (filterChange)="handleDocumentFilterIntent($event)" />
```

## Accessibility

The form has a named Filter Action Bar region and native submit behavior. Every select has a persistent visible label associated by `for` and `id`; placeholders describe the unfiltered state but never replace labels. Reset and Apply filters use native buttons through the Button primitive. Narrow filter disclosure retains `aria-expanded`, `aria-controls`, active-count text, and focus restoration from Filter Action Bar.

## Responsive behavior

Four controls form a single row on wide screens, two columns below the desktop breakpoint, and one column inside Filter Action Bar's narrow disclosure below 48rem. DOM and keyboard order remain project, category, status, visibility, reset, then submit.

## Do / don't

Do provide already-authorized, localized option labels and handle the emitted intent in application code. Don't fetch options, infer permissions, apply filters, own query state, or hide a control's visible label in this recipe.

## Visual coverage

Unit coverage verifies associated labels, selected-value submission, complete reset, and the single typed output. Narrow visual coverage should include collapsed and expanded filters with long localized option labels.

## Public import

```ts
import { DocumentFiltersComponent, type DocumentFilterChangeIntent } from 'src/web/design-system/public-api';
```
