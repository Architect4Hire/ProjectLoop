import { ChangeDetectionStrategy, Component, computed, input, numberAttribute } from '@angular/core';

@Component({
  selector: 'lsd-skeleton',
  standalone: true,
  host: {
    'aria-hidden': 'true',
    role: 'presentation',
  },
  templateUrl: './skeleton.component.html',
  styleUrl: './skeleton.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkeletonComponent {
  readonly lines = input(3, { transform: numberAttribute });

  protected readonly placeholders = computed(() => {
    const requested = this.lines();
    const count = Number.isFinite(requested) ? Math.floor(requested) : 3;
    return Array.from({ length: Math.max(1, Math.min(10, count)) });
  });
}
