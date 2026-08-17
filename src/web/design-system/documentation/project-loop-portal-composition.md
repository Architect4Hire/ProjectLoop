# Project Loop portal composition

This guide maps Project Loop portal screens to the supported public design-system compositions. It describes composition and ownership boundaries; the linked component guides remain authoritative for individual inputs, outputs, accessibility behavior, and variants.

## Public imports

Import every composition and public model from the application alias. Feature code must not import implementation files below a barrel.

```ts
import {
  ApprovalHistoryComponent,
  DocumentDownloadActionComponent,
  DocumentFiltersComponent,
  DocumentListComponent,
  DocumentUploadComponent,
  DocumentVersionHistoryComponent,
  MetricCardComponent,
  MetricGridComponent,
  MilestoneListComponent,
  PageHeaderComponent,
  PaginationComponent,
  PendingApprovalsListComponent,
  PortalShellComponent,
  ProjectDashboardComponent,
  ProjectHealthComponent,
  RecentDecisionsComponent,
  StateFeedbackComponent,
  UpcomingMeetingsComponent,
  VersionBoundApprovalComponent,
  type AppNavigationLink,
} from '@lsd/design-system';
```

The `@lsd/design-system` alias resolves to `src/web/design-system/public-api.ts`. Wildcard aliases and deep imports are unsupported.

## Portal frame

Use [Portal Shell](portal-shell.md) once around authenticated portal routes. Supply caller-authorized `AppNavigationLink` models and project the application header, active route content, and notification viewport into the shell-owned slots.

| Portal Shell area | Design-system ownership | Application ownership |
| --- | --- | --- |
| Skip navigation | Skip link and stable `#main-content` focus target | Nothing beyond keeping the shell mounted around route content |
| Header slot, `lsdPortalHeader` | Responsive header boundary | Brand, display-safe account identity, and caller-owned user actions |
| Navigation | Semantic navigation presentation and active/count rendering | Route URLs, active-state calculation, authorization filtering, and mobile navigation state |
| Main slot, `lsdPortalMain` | Main landmark, focus target, and responsive content boundary | Router outlet or selected feature screen |
| Notification slot, `lsdPortalNotifications` | Projection location | Notification service/viewport instance and application announcements |

The shell does not read router or authentication state. It does not decide which links or actions a person may access.

## Screen map

| Project Loop screen | Primary public composition | Supporting public compositions | Feature-owned state |
| --- | --- | --- | --- |
| Dashboard | [Project Dashboard](project-dashboard.md) | [Page Header](page-header.md), [Project Health](project-health.md), [Metric Grid](metric-grid.md), [Metric Card](metric-card.md), [Milestone List](milestone-list.md), [Upcoming Meetings](upcoming-meetings.md), [Recent Decisions](recent-decisions.md) | Project identity, formatted metrics, calculated health, authorized records, independent loading/errors, retry operations |
| Documents collection | [Document List](document-list.md) | [Document Filters](document-filters.md), [Page Header](page-header.md), [Pagination](pagination.md), [State Feedback](state-feedback.md) | Filter values/options, authorization, fetching, sorting, page totals, URL state, selection and empty-state copy |
| Document detail/version screen | [Document Version History](document-version-history.md) | [Version Chip](version-chip.md), [Document Download Action](document-download-action.md), [Document Upload](document-upload.md) when those actions belong on the route | Exact current/approved version facts, retrieval/upload operations, MIME and size policy, temporary download lifecycle, permission checks |
| Approval queue | [Pending Approvals List](pending-approvals-list.md) | [Page Header](page-header.md), [Pagination](pagination.md), [State Feedback](state-feedback.md) | Authorized and sorted requests, due text, target URLs, filtering, paging and fetching |
| Approval review | [Version-bound Approval](version-bound-approval.md) | [Approval History](approval-history.md) and caller-selected feedback/confirmation presentation | Exact target/version, current-version context, authorization, persistence, comment rules, processing/errors and navigation after a decision |
| Audit | [Audit Timeline](audit-timeline.md) | [Page Header](page-header.md) and [State Feedback](state-feedback.md) | Redacted records, filtering, ordering, paging/load-more state, correlation-copy handling and access control |

## Dashboard slot ownership

`ProjectDashboardComponent` owns region order, responsive placement, visible headings, and named sections. The application owns what each slot displays.

| Slot | Typical public presentation | Caller responsibilities |
| --- | --- | --- |
| `lsdProjectDashboardHealth` | `ProjectHealthComponent` | Calculate health and supply explanatory display text |
| `lsdProjectDashboardMetrics` | `MetricGridComponent` containing `MetricCardComponent` instances | Format values and trends; retain source data |
| `lsdProjectDashboardMilestones` | `MilestoneListComponent` | Supply authorized chronological records and already-derived labels |
| `lsdProjectDashboardMeetings` | `UpcomingMeetingsComponent` | Localize time-zone text and authorize join/navigation actions |
| `lsdProjectDashboardDecisions` | `RecentDecisionsComponent` | Supply authorized, sorted decisions and URLs |
| `lsdProjectDashboardDeliverables` | Caller-selected public recipe or `StateFeedbackComponent` | Choose the feature-specific deliverable presentation and manage its independent state |

Loading, empty, recoverable-error, and terminal state belongs to each region independently. Do not merge those states into one dashboard-wide loading flag.

## Documents ownership

Compose collection controls, results, and paging as siblings in the route's main content. `DocumentFiltersComponent` emits filter intent; `DocumentListComponent` selects row/card presentation responsively; `PaginationComponent` emits page intent. None of them fetches or authorizes data.

Keep the exact version supplied by the feature attached to every document and action. The feature decides which authorized representation to provide, performs upload/download operations, and maps API responses into display-ready public view models. The design system presents those values without resolving the current version, creating storage URLs, or inferring permissions.

## Approval ownership

The approval queue navigates to a review route; it does not approve inline. On the review route, `VersionBoundApprovalComponent` keeps the exact review target beside the actions. A current `v4` context must not change an approved `v3` qualifier. `ApprovalHistoryComponent` presents caller-supplied append-only evidence separately.

The feature owns authorization, transition eligibility, persistence, validation policy, and conflict handling. Treat emitted approve/reject/request-change values as intent only. Refresh caller state after persistence; never optimistically relabel another version as approved.

## Audit ownership

`AuditTimelineComponent` owns append-oriented presentation, event disclosure, and paging intent. The feature supplies already-redacted display data in authoritative order and performs filtering, loading, correlation copying, and access checks. Do not pass raw logs, secrets, tokens, document bodies, or unreviewed structured payloads through audit detail slots.

## Responsibility boundary

| Concern | Design system | Project Loop feature/API/security layers |
| --- | --- | --- |
| Semantic landmarks, headings, accessible names, keyboard behavior | Owns presentation contracts | Places compositions in the correct route hierarchy and supplies meaningful labels |
| Responsive layout and stable DOM reading order | Owns | Chooses which authorized records enter the layout |
| Visible loading/error/empty/progress presentation | Provides public components | Owns state machines, retries, cancellation and request lifecycles |
| Display-ready labels, dates, values and exact versions | Renders unchanged | Formats/localizes and validates authoritative values |
| Fetching, caching, mutation and concurrency | Never owns | Owns entirely |
| Authentication, authorization and tenant boundaries | Never owns | Filters links/actions/data before presentation and enforces every operation server-side |
| Domain calculations and transitions | Never owns | Calculates health, approval eligibility, lateness and workflow transitions |

Do not place API clients, route inspection, authorization rules, domain models, or business transitions in projected content helpers inside the design-system source tree.
