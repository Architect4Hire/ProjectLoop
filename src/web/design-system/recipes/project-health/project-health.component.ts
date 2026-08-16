import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

import { BadgeComponent, type BadgeVariant, ProgressComponent, SurfaceComponent } from '../../primitives';

export type ProjectHealthStatus = 'healthy' | 'attention' | 'at-risk' | 'unknown';

export interface ProjectHealthUpdatedTime {
  readonly label: string;
  readonly dateTime?: string;
}

export interface ProjectHealthIndicator {
  readonly id: string;
  readonly label: string;
  readonly value?: number;
  readonly max?: number;
  readonly valueText?: string;
}

interface HealthPresentation {
  readonly label: string;
  readonly variant: BadgeVariant;
}

@Component({
  selector: 'lsd-project-health',
  standalone: true,
  imports: [BadgeComponent, ProgressComponent, SurfaceComponent],
  templateUrl: './project-health.component.html',
  styleUrl: './project-health.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectHealthComponent {
  readonly id = input.required<string>();
  readonly status = input.required<ProjectHealthStatus>();
  readonly description = input.required<string>();
  readonly lastUpdated = input.required<ProjectHealthUpdatedTime>();
  readonly indicators = input<readonly ProjectHealthIndicator[]>([]);
  readonly label = input('Project health');

  protected readonly statusPresentation = computed(() => this.statusPresentations[this.status()]);

  protected indicatorId(indicator: ProjectHealthIndicator): string {
    return `${this.id()}-${indicator.id}`;
  }

  private readonly statusPresentations: Record<ProjectHealthStatus, HealthPresentation> = {
    healthy: { label: 'Healthy', variant: 'success' },
    attention: { label: 'Needs attention', variant: 'warning' },
    'at-risk': { label: 'At risk', variant: 'danger' },
    unknown: { label: 'Health unknown', variant: 'neutral' },
  };
}
