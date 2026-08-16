import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { BadgeComponent, type BadgeVariant, LinkDirective, SurfaceComponent } from '../../primitives';

export interface MilestoneStatusPresentation {
  readonly label: string;
  readonly variant: BadgeVariant;
}

export interface MilestoneDueDate {
  readonly label: string;
  readonly dateTime?: string;
}

export interface MilestoneNavigationIntent {
  readonly href: string;
  readonly label?: string;
}

export interface MilestoneSummary {
  readonly id: string;
  readonly title: string;
  readonly status: MilestoneStatusPresentation;
  readonly dueDate?: MilestoneDueDate;
  readonly navigation?: MilestoneNavigationIntent;
}

@Component({
  selector: 'lsd-milestone-list',
  standalone: true,
  imports: [BadgeComponent, LinkDirective, SurfaceComponent],
  templateUrl: './milestone-list.component.html',
  styleUrl: './milestone-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MilestoneListComponent {
  readonly milestones = input.required<readonly MilestoneSummary[]>();
  readonly label = input('Milestones');
  readonly emptyMessage = input('No milestones to show.');
  readonly missingDateLabel = input('Due date not provided');
}
