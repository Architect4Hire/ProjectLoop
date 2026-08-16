# Project dashboard pattern

## Purpose

`ProjectDashboardComponent` provides responsive placement and named slots for project health, metrics, milestones, meetings, decisions, and deliverables. It owns only region headings and layout. Callers own all data, permissions, recipe inputs, and each region's loading/error/empty state.

## Usage

```html
<lsd-project-dashboard id="project-overview">
  <lsd-project-health lsdProjectDashboardHealth ... />

  <lsd-metric-grid lsdProjectDashboardMetrics>
    @for (metric of metrics(); track metric.id) {
      <lsd-metric-card [label]="metric.label" [value]="metric.formattedValue" />
    }
  </lsd-metric-grid>

  <lsd-milestone-list lsdProjectDashboardMilestones [milestones]="milestones()" />
  <lsd-upcoming-meetings lsdProjectDashboardMeetings [meetings]="meetings()" />
  <lsd-recent-decisions lsdProjectDashboardDecisions [decisions]="decisions()" />
  <app-authorized-deliverables lsdProjectDashboardDeliverables />
</lsd-project-dashboard>
```

## Inputs and slots

- `id` is required and namespaces the six stable heading IDs.
- `headingLevel` accepts `2` or `3` so region headings fit the surrounding page hierarchy.
- Each heading has a caller-overridable text input: `healthHeading`, `metricsHeading`, `milestonesHeading`, `meetingsHeading`, `decisionsHeading`, and `deliverablesHeading`.
- Named slots are `lsdProjectDashboardHealth`, `lsdProjectDashboardMetrics`, `lsdProjectDashboardMilestones`, `lsdProjectDashboardMeetings`, `lsdProjectDashboardDecisions`, and `lsdProjectDashboardDeliverables`.

The slot attributes are content-projection selectors, not directives or data APIs.

## Independent region states

Any region may project its public recipe, a `StateFeedbackComponent`, or caller-owned switching markup that selects between them. Keep each region's loading, error, empty, and retry state independent. The dashboard does not accept a combined loading input, coordinate requests, suppress other regions, or announce state changes itself.

```html
<lsd-state-feedback
  lsdProjectDashboardMeetings
  variant="loading"
  heading="Loading upcoming meetings"
/>
```

## Accessibility

Each region is a native `section` named by a visible stable heading. Callers should choose a heading level consistent with the page header and keep projected content's own labels meaningful. State Feedback retains its own live-region behavior when projected.

## Responsive behavior and ordering

Desktop uses a twelve-column grid: health occupies four columns, metrics eight, and the remaining regions form two-column rows. Tablet and mobile widths stack every region. CSS changes placement only; DOM and reading order always remain health, metrics, milestones, meetings, decisions, then deliverables.

## Do / don't

Do compose Metric Grid and public recipes, supply already-authorized records, and handle State Feedback independently per region. Don't define dashboard view models in this pattern, fetch data, merge loading states, reorder regions visually, or put feature logic in the slots.

## Visual coverage

Capture light/dark desktop and mobile layouts with all regions populated. Include cases where individual regions independently show State Feedback and verify the stable health-to-deliverables order at every width.

## Public import

```ts
import { ProjectDashboardComponent } from 'src/web/design-system/public-api';
```
