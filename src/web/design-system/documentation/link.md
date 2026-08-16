# Link primitive

`LinkDirective` styles a native anchor for navigation while retaining the Button primitive's tone, impact, size, and shape vocabulary. Import it from `@lsd/design-system`; do not import its implementation path.

```html
<a lsdLink href="/projects/42" tone="primary" impact="minimal">Open project</a>
```

Apply the directive to an actual `<a>` so native `href`, projected content, browser navigation, context menus, and Angular RouterLink behavior remain authoritative. The directive has no click handler and does not emit an action event.

## Contract

- `tone`: `primary | danger | success | warning | info | neutral`
- `impact`: `bold | light | minimal`; links default to `minimal`
- `size`: `small | medium | large`
- `shape`: `square | rounded | pill`
- `href`, `target`, `rel`, `download`, `hreflang`, `referrerpolicy`, and `aria-*` remain native anchor attributes. RouterLink can set `href` without mediation by this primitive.
- Project visible link text or other inline content directly into the anchor. The accessible name must identify the destination.

Links with `target="_blank"` must identify that behavior in visible text or an accessible label, and should use `rel="noopener noreferrer"` when appropriate. The primitive does not infer whether a URL is external or change browsing context.

Visited styles deliberately retain each semantic tone so a visited link does not become indistinguishable from its chosen visual variant. The shared interaction foundation supplies the focus-visible ring.

Anchors do not support a native `disabled` attribute. For unavailable navigation, prefer conditional plain text. If an anchor must remain for semantic context, omit `href` and set `aria-disabled="true"`; do not leave a live destination on an apparently disabled link. An anchor without `href` is not keyboard focusable by default.
