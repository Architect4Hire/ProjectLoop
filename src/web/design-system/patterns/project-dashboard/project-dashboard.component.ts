import { ChangeDetectionStrategy, Component, input } from '@angular/core';

export type ProjectDashboardHeadingLevel = 2 | 3;

@Component({
  selector: 'lsd-project-dashboard',
  standalone: true,
  templateUrl: './project-dashboard.component.html',
  styleUrl: './project-dashboard.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectDashboardComponent {
  readonly id = input.required<string>();
  readonly headingLevel = input<ProjectDashboardHeadingLevel>(2);
  readonly healthHeading = input('Project health');
  readonly metricsHeading = input('Summary metrics');
  readonly milestonesHeading = input('Milestones');
  readonly meetingsHeading = input('Upcoming meetings');
  readonly decisionsHeading = input('Recent decisions');
  readonly deliverablesHeading = input('Deliverables');

  protected headingId(region: string): string {
    return `${this.id()}-${region}-heading`;
  }
}
