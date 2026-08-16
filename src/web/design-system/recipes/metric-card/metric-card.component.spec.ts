import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { MetricCardComponent, type MetricTrend } from './metric-card.component';

@Component({
  standalone: true,
  imports: [MetricCardComponent],
  template: `
    <lsd-metric-card
      label="Completion"
      [value]="value"
      [trend]="trend"
      context="Compared with the previous period"
      [action]="{ label: 'View details', href: '/details' }"
      [loading]="loading"
    />
  `,
})
class MetricCardTestHostComponent {
  value = '123,456,789,012,345 units completed';
  trend: MetricTrend = { label: '12% higher than previous period', direction: 'up' };
  loading = false;
}

describe('MetricCardComponent', () => {
  let fixture: ComponentFixture<MetricCardTestHostComponent>;
  let host: MetricCardTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [MetricCardTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(MetricCardTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('renders caller-formatted values and trend meaning as text', () => {
    const card = fixture.nativeElement as HTMLElement;
    expect(card.textContent).toContain('123,456,789,012,345 units completed');
    expect(card.textContent).toContain('12% higher than previous period');
    expect(card.querySelector('lsd-icon')?.getAttribute('aria-hidden')).toBe('true');
  });

  it('uses a native action link without intercepting its destination', () => {
    const action = fixture.debugElement.query(By.css('a[lsdLink]')).nativeElement as HTMLAnchorElement;
    expect(action.getAttribute('href')).toBe('/details');
    expect(action.textContent).toContain('View details');
  });

  it('keeps loading content safe and communicates the busy state', () => {
    host.loading = true;
    fixture.detectChanges();

    const article = fixture.debugElement.query(By.css('article')).nativeElement as HTMLElement;
    expect(article.getAttribute('aria-busy')).toBe('true');
    expect(article.textContent).toContain('Loading metric value');
    expect(article.textContent).not.toContain(host.value);
    expect(fixture.debugElement.query(By.css('a[lsdLink]'))).toBeNull();
  });

  it('provides wrapping hooks for long values at narrow widths', () => {
    const value = fixture.debugElement.query(By.css('.lsd-metric-card__value')).nativeElement as HTMLElement;
    expect(value.textContent).toContain(host.value);
    expect(value.classList).toContain('lsd-metric-card__value');
  });
});
