import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

import { IconComponent, type IconName } from '../../icons';
import { BadgeComponent, type BadgeVariant, LinkDirective, SkeletonComponent, SurfaceComponent } from '../../primitives';

export type MetricTrendDirection = 'up' | 'down' | 'neutral';

export interface MetricTrend {
  readonly label: string;
  readonly direction: MetricTrendDirection;
}

export interface MetricCardAction {
  readonly label: string;
  readonly href: string;
  readonly accessibleLabel?: string;
}

interface TrendPresentation {
  readonly icon: IconName;
  readonly variant: BadgeVariant;
}

@Component({
  selector: 'lsd-metric-card',
  standalone: true,
  imports: [BadgeComponent, IconComponent, LinkDirective, SkeletonComponent, SurfaceComponent],
  templateUrl: './metric-card.component.html',
  styleUrl: './metric-card.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MetricCardComponent {
  readonly label = input.required<string>();
  readonly value = input.required<string>();
  readonly trend = input<MetricTrend | null>(null);
  readonly context = input<string | null>(null);
  readonly action = input<MetricCardAction | null>(null);
  readonly loading = input(false);

  protected readonly trendPresentation = computed(() => {
    const trend = this.trend();
    return trend ? this.trendPresentations[trend.direction] : null;
  });

  private readonly trendPresentations: Record<MetricTrendDirection, TrendPresentation> = {
    up: { icon: 'chevron-up', variant: 'success' },
    down: { icon: 'chevron-down', variant: 'danger' },
    neutral: { icon: 'minus', variant: 'neutral' },
  };
}
