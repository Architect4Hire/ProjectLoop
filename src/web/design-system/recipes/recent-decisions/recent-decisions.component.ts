import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { BadgeComponent, type BadgeVariant, LinkDirective, SurfaceComponent } from '../../primitives';

export interface RecentDecisionStatus {
  readonly label: string;
  readonly variant: BadgeVariant;
}

export interface RecentDecisionDate {
  readonly label: string;
  readonly dateTime?: string;
}

export interface RecentDecisionNavigation {
  readonly href: string;
  readonly accessibleLabel?: string;
}

export interface RecentDecisionRecord {
  readonly id: string;
  readonly label: string;
  readonly status: RecentDecisionStatus;
  readonly date: RecentDecisionDate;
  readonly navigation: RecentDecisionNavigation;
}

@Component({
  selector: 'lsd-recent-decisions',
  standalone: true,
  imports: [BadgeComponent, LinkDirective, SurfaceComponent],
  templateUrl: './recent-decisions.component.html',
  styleUrl: './recent-decisions.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RecentDecisionsComponent {
  readonly decisions = input.required<readonly RecentDecisionRecord[]>();
  readonly label = input('Recent decisions');
  readonly emptyMessage = input('No recent decisions.');
}
