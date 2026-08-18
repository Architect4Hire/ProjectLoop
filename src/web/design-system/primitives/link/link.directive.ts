import { Directive, computed, input } from '@angular/core';

export type LinkImpact = 'bold' | 'light' | 'minimal';
export type LinkShape = 'square' | 'rounded' | 'pill';
export type LinkSize = 'small' | 'medium' | 'large';
export type LinkTone = 'primary' | 'danger' | 'success' | 'warning' | 'info' | 'neutral';

@Directive({
  selector: 'a[lsdLink]',
  standalone: true,
  host: {
    '[class]': 'classes()',
  },
})
export class LinkDirective {
  readonly impact = input<LinkImpact>('minimal');
  readonly shape = input<LinkShape>('rounded');
  readonly size = input<LinkSize>('medium');
  readonly tone = input<LinkTone>('primary');
  readonly fullWidth = input(false);

  protected readonly classes = computed(() =>
    [
      'lsd-link inline-flex items-center justify-center gap-2 font-semibold underline-offset-4 transition-colors focus-visible:outline-none',
      this.sizeClasses[this.size()],
      this.shapeClasses[this.shape()],
      this.impactClasses[this.tone()][this.impact()],
      this.fullWidth() ? 'w-full' : '',
    ].filter(Boolean).join(' '),
  );

  private readonly sizeClasses: Record<LinkSize, string> = {
    small: 'min-h-8 px-3 text-xs',
    medium: 'min-h-10 px-5 text-sm',
    large: 'min-h-12 px-7 text-lg',
  };

  private readonly shapeClasses: Record<LinkShape, string> = {
    square: 'rounded-none',
    rounded: 'rounded-lg',
    pill: 'rounded-full',
  };

  private readonly impactClasses: Record<LinkTone, Record<LinkImpact, string>> = {
    primary: {
      bold: 'bg-accent-primary text-text-on-accent hover:bg-accent-primary/90 visited:text-text-on-accent',
      light: 'bg-accent-primary/20 text-accent-primary hover:bg-accent-primary/30 visited:text-accent-primary',
      minimal: 'bg-transparent text-accent-primary hover:bg-accent-primary/10 visited:text-accent-primary',
    },
    danger: {
      bold: 'bg-status-danger text-text-on-danger hover:bg-status-danger/90 visited:text-text-on-danger',
      light: 'bg-status-danger/20 text-status-danger hover:bg-status-danger/30 visited:text-status-danger',
      minimal: 'bg-transparent text-status-danger hover:bg-status-danger/10 visited:text-status-danger',
    },
    success: {
      bold: 'bg-status-success text-text-on-success hover:bg-status-success/90 visited:text-text-on-success',
      light: 'bg-status-success/20 text-status-success hover:bg-status-success/30 visited:text-status-success',
      minimal: 'bg-transparent text-status-success hover:bg-status-success/10 visited:text-status-success',
    },
    warning: {
      bold: 'bg-status-warning text-text-on-warning hover:bg-status-warning/90 visited:text-text-on-warning',
      light: 'bg-status-warning/20 text-status-warning hover:bg-status-warning/30 visited:text-status-warning',
      minimal: 'bg-transparent text-status-warning hover:bg-status-warning/10 visited:text-status-warning',
    },
    info: {
      bold: 'bg-status-info text-text-on-info hover:bg-status-info/90 visited:text-text-on-info',
      light: 'bg-status-info/20 text-status-info hover:bg-status-info/30 visited:text-status-info',
      minimal: 'bg-transparent text-status-info hover:bg-status-info/10 visited:text-status-info',
    },
    neutral: {
      bold: 'bg-surface-raised text-text-primary hover:bg-surface-raised/90 visited:text-text-primary',
      light: 'bg-surface-raised/40 text-text-primary hover:bg-surface-raised/60 visited:text-text-primary',
      minimal: 'bg-transparent text-text-muted hover:bg-surface-raised/40 visited:text-text-muted',
    },
  };
}
