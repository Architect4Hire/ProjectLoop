# Progress primitive

`ProgressComponent` presents determinate or indeterminate operation progress using a native `<progress>` element and a token-styled visual track. Import it from `@lsd/design-system`.

```html
<lsd-progress id="summary-generation" label="Generating summary" [value]="completed" [max]="total" />
<lsd-progress id="document-loading" label="Loading document" />
```

## Contract

- `id` and `label` are required. The stable caller-supplied ID associates the visible label with the native progress element.
- `value` is optional. A finite value creates determinate progress and is clamped between zero and `max`. Omitting `value` creates native indeterminate progress by omitting the progress element's `value` attribute.
- `max` defaults to `100`. Non-finite or non-positive maxima fall back to `100`.
- `valueText` optionally supplies visible and accessible state text, such as `3 of 8 files`. Otherwise determinate state displays a rounded percentage and indeterminate state displays `In progress`.

The visible label names the native progress element, and visible value text is mirrored through `aria-valuetext`. The visual track is decorative. Indeterminate animation stops under `prefers-reduced-motion: reduce`, and determinate width changes lose their transition while the progress state remains visible.

This primitive reports caller-provided progress only. It does not calculate upload progress, infer project health, start work, poll services, or decide completion.
