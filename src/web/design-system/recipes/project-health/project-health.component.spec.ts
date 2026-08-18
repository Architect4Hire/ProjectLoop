import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { ProjectHealthComponent, type ProjectHealthStatus } from './project-health.component';

@Component({
  standalone: true,
  imports: [ProjectHealthComponent],
  template: `
    <lsd-project-health
      id="delivery"
      [status]="status()"
      description="Caller-supplied explanation of the current state."
      [lastUpdated]="{ label: '16 August 2026 at 9:30 AM', dateTime: '2026-08-16T14:30:00Z' }"
      [indicators]="[
        { id: 'readiness', label: 'Readiness', value: 72, valueText: '72 percent ready' },
        { id: 'confidence', label: 'Confidence', value: 4, max: 5, valueText: '4 of 5' }
      ]"
    />
  `,
})
class ProjectHealthTestHostComponent {
  readonly status = signal<ProjectHealthStatus>('healthy');
}

describe('ProjectHealthComponent', () => {
  let fixture: ComponentFixture<ProjectHealthTestHostComponent>;
  let host: ProjectHealthTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ProjectHealthTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(ProjectHealthTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('pairs every caller-selected health state with visible text', () => {
    const expected: Record<ProjectHealthStatus, string> = {
      healthy: 'Healthy',
      attention: 'Needs attention',
      'at-risk': 'At risk',
      unknown: 'Health unknown',
    };

    for (const status of Object.keys(expected) as ProjectHealthStatus[]) {
      host.status.set(status);
      fixture.detectChanges();
      const badge = fixture.debugElement.query(By.css('lsd-badge')).nativeElement as HTMLElement;
      expect(badge.textContent).toContain(expected[status]);
    }
  });

  it('renders the caller explanation and machine-readable last-updated time', () => {
    const card = fixture.nativeElement as HTMLElement;
    const time = fixture.debugElement.query(By.css('time')).nativeElement as HTMLTimeElement;
    expect(card.textContent).toContain('Caller-supplied explanation of the current state.');
    expect(card.textContent).toContain('Last updated');
    expect(time.dateTime).toBe('2026-08-16T14:30:00Z');
    expect(time.textContent).toContain('16 August 2026 at 9:30 AM');
  });

  it('presents optional caller-supplied indicators with namespaced progress IDs', () => {
    const progress = fixture.debugElement.queryAll(By.css('progress'));
    expect(progress).toHaveSize(2);
    expect((progress[0].nativeElement as HTMLProgressElement).id).toBe('delivery-readiness-progress');
    expect((progress[0].nativeElement as HTMLProgressElement).getAttribute('aria-valuetext')).toBe('72 percent ready');
    expect((progress[1].nativeElement as HTMLProgressElement).max).toBe(5);
  });
});
