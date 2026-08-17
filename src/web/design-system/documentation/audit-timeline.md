# Audit timeline

## Purpose

`AuditTimelineComponent` composes Activity Stream, Audit Event, and controlled paging into an append-oriented audit presentation. The caller owns authorized filtering, chronological ordering, page totals, loading, fetching, and persistence.

## API and states

- Required readonly `events` are rendered in supplied order and never sorted or mutated.
- Each event contains display-ready audit identity, actor, action, resource, UTC time, correlation ID, optional attribution, and optional already-redacted detail summary.
- `paging` is either controlled `pages` state for Pagination or controlled `load-more` state with `hasMore` and optional `loading`.
- `pageChange` and `loadMoreRequested` emit intent only. The component does not change the current page or append records itself.
- `correlationCopyRequested` forwards Audit Event copy intent for caller-owned clipboard behavior.

```html
<lsd-audit-timeline
  id="workspace-audit"
  [events]="authorizedOrderedEvents"
  [paging]="{ mode: 'load-more', hasMore: hasMore(), loading: loading() }"
  (loadMoreRequested)="loadNextPage()" />
```

## Accessibility and responsive behavior

Activity Stream provides named ordered chronology and an accessible empty state. Expanded entries compose full Audit Event semantics, including UTC `time`, resource identity, correlation copying, and redacted details. Pagination is a labeled navigation region; load-more uses a native Button state. Narrow layouts stack nested event details and widen the load-more control without changing record order.

## Append-only boundary

Treat `events` as the caller's current immutable window or accumulated append-only page set. Accepting a paging intent and combining results remains caller work. This pattern provides no edit, delete, reorder, filter, fetch, or persistence API.

## Do / don't

Do pass only authorized, already-redacted display data in established chronology. Don't infer ordering, mutate records, fetch pages, expose raw logs, or treat an emitted paging intent as proof that data loaded.

## Public import

```ts
import { AuditTimelineComponent, type AuditTimelinePaging } from 'src/web/design-system/public-api';
```
