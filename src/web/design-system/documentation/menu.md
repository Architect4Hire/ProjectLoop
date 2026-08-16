# Action menu primitive

`MenuComponent` provides a native menu-button trigger, disclosure state, and managed focus for compact user or row actions. `MenuItemDirective` marks projected native buttons or anchors as menu items. Import both from `@lsd/design-system`.

```html
<lsd-menu id="project-actions" accessibleLabel="Project actions">
  <app-more-icon lsdMenuTriggerContent aria-hidden="true" />
  <button lsdMenuItem type="button" (click)="rename()">Rename</button>
  <a lsdMenuItem href="/projects/42/history">View history</a>
</lsd-menu>
```

## Contract

- `id` is required and stable. It derives associated trigger and menu IDs.
- `accessibleLabel` is required and names the native button trigger. Trigger projection through `lsdMenuTriggerContent` is visual and does not replace that name.
- Apply `lsdMenuItem` only to native `<button>` actions or `<a>` destinations. Items retain their own click handlers, `href`, RouterLink behavior, and application decisions.
- Disabled native buttons and items with `aria-disabled="true"` are skipped by managed focus. An unavailable anchor must omit `href`; `aria-disabled` alone does not suppress navigation.

Arrow Down opens the menu and focuses its first available item; Arrow Up opens it at the last item. Within the menu, Arrow Up/Down wraps through available items and Home/End moves to the boundaries. Escape dismisses and restores focus to the trigger. Selecting an item also restores trigger focus without preventing its native action or navigation. Tab and outside pointer interaction dismiss without stealing the user's destination focus.

The menu owns disclosure and focus only. Callers own action handling, routes, authorization-based inclusion, and destructive confirmation. Do not show an unauthorized item merely disabled, and do not use this primitive as confirmation UI.
