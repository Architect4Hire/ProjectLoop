# Upcoming meetings recipe

## Purpose

`UpcomingMeetingsComponent` presents caller-supplied meeting titles, localized time text, location or channel text, and optional action links. It is a display recipe with no calendar, conferencing, timezone, or authorization integration.

## Usage

```ts
readonly meetings: readonly MeetingSummary[] = [
  {
    id: 'planning',
    title: 'Planning conversation',
    time: {
      label: 'Monday, August 17 at 9:30 AM CDT',
      dateTime: '2026-08-17T09:30:00-05:00',
    },
    location: 'Video channel',
    action: {
      label: 'Join',
      href: '/authorized/join',
      accessibleLabel: 'Join planning conversation',
    },
  },
];
```

```html
<lsd-upcoming-meetings [meetings]="meetings" />
```

## Inputs and states

- `meetings` is a required immutable list of display-ready summaries with stable IDs.
- `time.label` is required localized text. Optional `dateTime` adds a machine-readable value without changing the visible text.
- `location` contains caller-selected location or channel text.
- `action` is optional and renders a native Link only when supplied by the caller.
- `label` names the section, and `emptyMessage` customizes the explicit no-meetings presentation.

## Caller ownership

Callers convert meeting times to the intended timezone and locale before rendering. Callers also decide whether the current user may see or use a join/action destination; omit `action` when it is not authorized. The recipe does not fetch events, inspect calendar accounts, generate conference URLs, refresh credentials, or join meetings.

## Accessibility

Meetings use native list semantics. Visible time and location/channel text remain adjacent, and valid machine-readable timestamps use `time`. Action links retain native anchor semantics and can receive a meeting-specific accessible label. The chevron icon is decorative.

## Responsive behavior

At wide widths, details and actions share a row. At narrow widths, the action moves below the details while DOM and reading order stay unchanged. Long titles, localized times, and location/channel text wrap without horizontal overflow.

## Do / don't

Do pass display-safe, localized text and only caller-authorized action links. Don't pass raw timestamps expecting conversion, integrate a calendar or conferencing SDK, or infer join permissions in this recipe.

## Visual coverage

Capture populated and empty presentations in light/dark appearances at desktop/mobile widths. Include long titles, long localized time/location text, and meetings both with and without actions.

## Public import

```ts
import { UpcomingMeetingsComponent } from 'src/web/design-system/public-api';
```
