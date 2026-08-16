# Field message primitive

`FieldMessageComponent` renders reusable help, error, or success text for a form control without evaluating or owning validation. Import it from `@lsd/design-system`.

```html
<input
  id="project-name"
  aria-describedby="project-name-help project-name-error"
  aria-errormessage="project-name-error"
  aria-invalid="true" />
<lsd-field-message id="project-name-help">Use the client-facing name.</lsd-field-message>
<lsd-field-message id="project-name-error" kind="error">A project name is required.</lsd-field-message>
```

## Contract

- `id` is required and must be stable and unique. The caller references it from the native control's `aria-describedby`; errors may also be referenced by `aria-errormessage`.
- `kind`: `help | error | success`; the default is `help`.
- Project the message content. Keep it concise and actionable.
- Help text has no live-region behavior because persistent instructions should be discovered through the control's description association.
- Error messages use an atomic assertive alert. Render or update them when validation fails; do not announce errors before the user has had a reasonable opportunity to complete the field.
- Success messages use an atomic polite status.

The primitive does not set `aria-invalid`, decide whether a value is valid, manage touched/submitted state, or mutate a control's accessibility attributes. Callers own validation and all IDREF associations. Prefer conditional rendering so obsolete error and success messages are removed when their state no longer applies.
