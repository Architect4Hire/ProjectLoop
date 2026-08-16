import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'lsd-metric-grid',
  standalone: true,
  templateUrl: './metric-grid.component.html',
  styleUrl: './metric-grid.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MetricGridComponent {}
