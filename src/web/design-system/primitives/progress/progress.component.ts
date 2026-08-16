import { ChangeDetectionStrategy, Component, computed, input, numberAttribute } from '@angular/core';

function optionalNumber(value: unknown): number | undefined {
  return value === undefined || value === null ? undefined : numberAttribute(value);
}

@Component({
  selector: 'lsd-progress',
  standalone: true,
  host: { '[attr.id]': 'null' },
  templateUrl: './progress.component.html',
  styleUrl: './progress.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProgressComponent {
  readonly id = input.required<string>();
  readonly label = input.required<string>();
  readonly value = input<number | undefined, unknown>(undefined, { transform: optionalNumber });
  readonly max = input(100, { transform: numberAttribute });
  readonly valueText = input<string | undefined>(undefined);

  protected readonly progressId = computed(() => `${this.id()}-progress`);
  protected readonly labelId = computed(() => `${this.id()}-label`);
  protected readonly valueId = computed(() => `${this.id()}-value`);
  protected readonly determinate = computed(() => this.value() !== undefined);
  protected readonly normalizedMax = computed(() => {
    const maximum = this.max();
    return Number.isFinite(maximum) && maximum > 0 ? maximum : 100;
  });
  protected readonly normalizedValue = computed(() => {
    const current = this.value();
    if (current === undefined || !Number.isFinite(current)) return 0;
    return Math.min(Math.max(current, 0), this.normalizedMax());
  });
  protected readonly percentage = computed(() =>
    this.determinate() ? (this.normalizedValue() / this.normalizedMax()) * 100 : undefined,
  );
  protected readonly visibleValue = computed(() => {
    if (this.valueText()) return this.valueText()!;
    const percentage = this.percentage();
    return percentage === undefined ? 'In progress' : `${Math.round(percentage)}%`;
  });
}
