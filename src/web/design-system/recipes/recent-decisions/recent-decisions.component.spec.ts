import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { RecentDecisionsComponent, type RecentDecisionRecord } from './recent-decisions.component';

@Component({
  standalone: true,
  imports: [RecentDecisionsComponent],
  template: `<lsd-recent-decisions [decisions]="decisions" />`,
})
class RecentDecisionsTestHostComponent {
  decisions: readonly RecentDecisionRecord[] = [
    {
      id: 'accepted',
      label: 'Use the shared public entry point',
      status: { label: 'Accepted', variant: 'success' },
      date: { label: 'August 16, 2026', dateTime: '2026-08-16' },
      navigation: { href: '/decisions/accepted', accessibleLabel: 'Open decision: Use the shared public entry point' },
    },
    {
      id: 'proposed',
      label: 'Adopt the proposed review sequence',
      status: { label: 'Proposed', variant: 'warning' },
      date: { label: 'August 15, 2026', dateTime: '2026-08-15' },
      navigation: { href: '/decisions/proposed' },
    },
    {
      id: 'superseded',
      label: 'Retain the previous publishing flow',
      status: { label: 'Superseded', variant: 'neutral' },
      date: { label: 'August 14, 2026' },
      navigation: { href: '/decisions/superseded' },
    },
  ];
}

describe('RecentDecisionsComponent', () => {
  let fixture: ComponentFixture<RecentDecisionsTestHostComponent>;
  let host: RecentDecisionsTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [RecentDecisionsTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(RecentDecisionsTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('renders a compact mixed-status list using visible badge labels', () => {
    const items = fixture.debugElement.queryAll(By.css('li'));
    const badges = fixture.debugElement.queryAll(By.css('lsd-badge'));
    expect(items).toHaveSize(3);
    expect(badges.map(badge => (badge.nativeElement as HTMLElement).textContent?.trim()))
      .toEqual(['Accepted', 'Proposed', 'Superseded']);
  });

  it('uses native links for caller-authorized records and preserves date text', () => {
    const firstLink = fixture.debugElement.query(By.css('a[lsdLink]')).nativeElement as HTMLAnchorElement;
    const firstTime = fixture.debugElement.query(By.css('time')).nativeElement as HTMLTimeElement;
    expect(firstLink.getAttribute('href')).toBe('/decisions/accepted');
    expect(firstLink.getAttribute('aria-label')).toBe('Open decision: Use the shared public entry point');
    expect(firstTime.textContent).toContain('August 16, 2026');
    expect(firstTime.dateTime).toBe('2026-08-16');
  });

  it('renders an explicit empty presentation', () => {
    host.decisions = [];
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('ul'))).toBeNull();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('No recent decisions.');
  });
});
