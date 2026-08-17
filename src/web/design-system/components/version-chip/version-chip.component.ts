import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

import { BadgeComponent, type BadgeVariant } from '../../primitives/badge/badge.component';

export type VersionQualifier = 'current' | 'approved' | 'published';

@Component({
  selector: 'lsd-version-chip',
  standalone: true,
  imports: [BadgeComponent],
  templateUrl: './version-chip.component.html',
  styleUrl: './version-chip.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VersionChipComponent {
  /** Exact, caller-formatted version identifier. It is always rendered visibly. */
  readonly versionLabel = input.required<string>();
  /** Caller-owned qualifier for this version only. */
  readonly qualifier = input<VersionQualifier | undefined>(undefined);
  readonly qualifierLabel = input<string | undefined>(undefined);

  protected readonly visibleQualifier = computed(() => {
    const qualifier = this.qualifier();
    return qualifier ? (this.qualifierLabel() ?? this.qualifierLabels[qualifier]) : undefined;
  });

  protected readonly variant = computed<BadgeVariant>(() => {
    const qualifier = this.qualifier();
    return qualifier ? this.qualifierVariants[qualifier] : 'neutral';
  });

  private readonly qualifierLabels: Readonly<Record<VersionQualifier, string>> = {
    current: 'Current',
    approved: 'Approved',
    published: 'Published',
  };

  private readonly qualifierVariants: Readonly<Record<VersionQualifier, BadgeVariant>> = {
    current: 'info',
    approved: 'approved',
    published: 'success',
  };
}
