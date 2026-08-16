import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { IconComponent } from '../../icons';
import { LinkDirective, SurfaceComponent } from '../../primitives';

export interface MeetingTimePresentation {
  readonly label: string;
  readonly dateTime?: string;
}

export interface MeetingAction {
  readonly label: string;
  readonly href: string;
  readonly accessibleLabel?: string;
}

export interface MeetingSummary {
  readonly id: string;
  readonly title: string;
  readonly time: MeetingTimePresentation;
  readonly location: string;
  readonly action?: MeetingAction;
}

@Component({
  selector: 'lsd-upcoming-meetings',
  standalone: true,
  imports: [IconComponent, LinkDirective, SurfaceComponent],
  templateUrl: './upcoming-meetings.component.html',
  styleUrl: './upcoming-meetings.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UpcomingMeetingsComponent {
  readonly meetings = input.required<readonly MeetingSummary[]>();
  readonly label = input('Upcoming meetings');
  readonly emptyMessage = input('No upcoming meetings.');
}
