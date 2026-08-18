# Composing Project Loop features

Use this workflow when a Project Loop feature needs UI derived from the Lake Shore Drive design system. Start with the [portal screen map](project-loop-portal-composition.md), select the highest-level public pattern or recipe that matches the interaction, and add lower-level public components only for the remaining presentation. The [documentation index](README.md) links the authoritative guide for every supported API; follow those guides for inputs, outputs, states, accessibility, and responsive behavior instead of repeating their contracts here.

## Public import example

Import components, directives, and public models only from the application-facing package entry point:

```ts
import {
  DocumentFiltersComponent,
  DocumentListComponent,
  PageHeaderComponent,
  PaginationComponent,
  StateFeedbackComponent,
  type DocumentListItem,
} from '@lsd/design-system';
```

For this composition, use the individual [Page Header](page-header.md), [Document Filters](document-filters.md), [Document List](document-list.md), [Pagination](pagination.md), and [State Feedback](state-feedback.md) guides. See [Public imports](public-imports.md) for the supported package boundary and [Integration](integration.md) for workspace installation.

## Selection order

1. Find the screen in the [Project Loop portal composition guide](project-loop-portal-composition.md).
2. Prefer its primary public pattern, recipe, or layout when it already owns the semantic structure and responsive presentation.
3. Add supporting APIs linked from that screen row. Use primitives only when no higher-level public API expresses the interaction.
4. Keep feature-specific business composition in the feature. If a reusable presentation contract is missing, propose it in the design system instead of copying a control locally.
5. Verify the import is exported by `@lsd/design-system`; a source file existing under the design-system directory does not make it public.

## Ownership boundary

| Concern | Design-system API | Project Loop feature and service layers |
| --- | --- | --- |
| Component state | Owns local presentation state such as disclosure, focus, and caller-controlled input rendering | Owns screen state, selected records, filters, loading/error state machines, and business transitions |
| APIs | Emits typed user intent and renders caller-supplied results | Calls APIs, maps responses, handles retries/cancellation, caching, and errors |
| Routing | Renders caller-supplied URLs and active presentation | Reads and changes router state, resolves route parameters, and owns navigation effects |
| Authorization | Presents only the actions and records supplied to it; visibility never proves permission | Filters display data and actions, and enforces authorization again at the server boundary |
| Persistence | Never claims persistence from a click or stores domain records | Validates, persists, handles concurrency, refreshes authoritative state, and reports outcomes |

## Do / don't

Do import supported APIs through the package entry point:

```ts
import { ButtonComponent, InputComponent } from '@lsd/design-system';
```

Don't deep-import an implementation file, even when it currently exists:

```ts
// Unsupported: bypasses the reviewed public contract.
import { ButtonComponent } from '@lsd/design-system/primitives/button/button.component';
```

Do compose the public [Button](button.md), [Input](input.md), and other cataloged controls inside feature-specific layouts. Don't create a feature-local button, input, dialog, pagination control, or copied styling bundle that duplicates an existing public control role. Business-specific compositions that have no reusable design-system equivalent remain feature-owned.

