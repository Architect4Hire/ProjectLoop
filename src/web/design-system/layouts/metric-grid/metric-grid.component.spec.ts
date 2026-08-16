import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { MetricCardComponent } from '../../recipes/metric-card/metric-card.component';
import { MetricGridComponent } from './metric-grid.component';

@Component({
  standalone: true,
  imports: [MetricCardComponent, MetricGridComponent],
  template: `
    <lsd-metric-grid>
      @for (metric of metrics; track metric.label) {
        <lsd-metric-card [label]="metric.label" [value]="metric.value" />
      }
    </lsd-metric-grid>
  `,
})
class MetricGridTestHostComponent {
  metrics = [
    { label: 'First', value: '1' },
    { label: 'Second', value: '2' },
    { label: 'Third', value: '3' },
    { label: 'Fourth', value: '4' },
  ];
}

describe('MetricGridComponent', () => {
  let fixture: ComponentFixture<MetricGridTestHostComponent>;
  let host: MetricGridTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [MetricGridTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(MetricGridTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('projects one through four metric cards without adding card semantics', () => {
    expect(fixture.debugElement.queryAll(By.directive(MetricCardComponent))).toHaveSize(4);
    const grid = fixture.debugElement.query(By.css('.lsd-metric-grid')).nativeElement as HTMLElement;
    expect(grid.getAttribute('role')).toBeNull();

    host.metrics = host.metrics.slice(0, 1);
    fixture.detectChanges();
    expect(fixture.debugElement.queryAll(By.directive(MetricCardComponent))).toHaveSize(1);
  });

  it('preserves caller DOM order', () => {
    const labels = Array.from(
      (fixture.nativeElement as HTMLElement).querySelectorAll('.lsd-metric-card__label'),
      element => element.textContent?.trim(),
    );
    expect(labels).toEqual(['First', 'Second', 'Third', 'Fourth']);
  });
});
