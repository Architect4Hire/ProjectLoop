# Milestone list recipe

## Purpose

`MilestoneListComponent` presents caller-supplied milestones in a native ordered list with visible status, due-date information, and optional navigation. It preserves the chronological reading order provided by the caller and contains no schedule calculations.

## Usage

```ts
readonly milestones: readonly MilestoneSummary[] = [
  {
    id: 'review',
    title: 'Review completed',
    status: { label: 'Overdue', variant: 'danger' },
    dueDate: { label: '15 August 2026', dateTime: '2026-08-15' },
    navigation: { href: '/milestones/review', label: 'Open review milestone' },
  },
  {
    id: 'approval',
    title: 'Approval recorded',
    status: { label: 'Planned', variant: 'neutral' },
  },
];
```

```html
<lsd-milestone-list [milestones]="milestones" />
```

## Inputs and states

- `milestones` is a required immutable array. Each item has a stable ID, title, and caller-defined badge presentation.
- `dueDate` optionally supplies visible formatted text and a machine-readable `dateTime`. Missing dates render `missingDateLabel` instead of an invented date.
- `navigation` optionally supplies a native anchor destination and accessible label. Items without it remain non-interactive.
- `label` names the section, while `emptyMessage` and `missingDateLabel` customize explicit fallback text.

The component iterates the array exactly as supplied. It does not sort, mutate, group, or filter milestones.

## Status and dates

Status meaning is entirely caller-owned. To present lateness, pass visible text such as “Overdue” with an appropriate badge variant. The recipe never compares dates with the current time and never infers that a missing or past date is late.

## Accessibility

Native ordered-list semantics communicate sequence. Status always has visible text. Valid machine-readable dates use the native `time` element, while missing dates use visible fallback text. Optional navigation composes the Link primitive and retains native anchor behavior.

## Responsive behavior

Titles, date labels, and badges wrap without changing DOM order. At narrow widths the badge moves below the milestone content. Long titles use available inline space and wrap instead of causing horizontal overflow.

## Do / don't

Do sort milestones chronologically before passing them, provide display-ready date/status labels, and use stable IDs. Don't expect the recipe to infer lateness, change chronology, fetch milestones, or mutate caller data.

## Visual coverage

Capture empty, caller-labelled overdue, and long-title presentations in light/dark appearances and desktop/mobile widths. Include missing dates and items with and without navigation.

## Public import

```ts
import { MilestoneListComponent } from 'src/web/design-system/public-api';
```
