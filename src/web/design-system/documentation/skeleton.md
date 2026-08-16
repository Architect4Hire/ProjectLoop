# Skeleton primitive

`SkeletonComponent` renders reusable placeholder lines while content is loading. It is decorative only; import it from `@lsd/design-system` and place it inside a caller-owned loading region.

```html
<section role="status" aria-busy="true" aria-label="Loading results">
  <lsd-skeleton [lines]="4" />
</section>
```

## Contract

- `lines` defaults to `3` and is clamped to 1–10 whole placeholder lines.
- The component host is permanently `aria-hidden="true"` and presentational. It never supplies a status role, accessible label, live region, or `aria-busy` state.
- The parent region owns loading text, announcements, and busy-state changes. Do not use a skeleton without an equivalent accessible loading state.

Placeholder pulse timing uses semantic motion tokens. Under `prefers-reduced-motion: reduce`, animation is removed and the placeholders remain fully visible, so motion never carries loading meaning by itself.

`StateFeedbackComponent` composes this primitive for its skeleton variant while retaining ownership of the loading region, copy, `aria-busy`, and announcements.
