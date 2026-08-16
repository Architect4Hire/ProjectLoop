import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { MilestoneListComponent, type MilestoneSummary } from './milestone-list.component';

@Component({
  standalone: true,
  imports: [MilestoneListComponent],
  template: `<lsd-milestone-list [milestones]="milestones" />`,
})
class MilestoneListTestHostComponent {
  milestones: readonly MilestoneSummary[] = [
    {
      id: 'first',
      title: 'A very long caller-supplied milestone title that must remain readable without changing its chronological position',
      status: { label: 'Overdue', variant: 'danger' },
      dueDate: { label: '15 August 2026', dateTime: '2026-08-15' },
      navigation: { href: '/milestones/first', label: 'Open first milestone' },
    },
    {
      id: 'second',
      title: 'Second milestone',
      status: { label: 'Planned', variant: 'neutral' },
    },
  ];
}

describe('MilestoneListComponent', () => {
  let fixture: ComponentFixture<MilestoneListTestHostComponent>;
  let host: MilestoneListTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [MilestoneListTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(MilestoneListTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('preserves caller order in a native ordered list', () => {
    const list = fixture.debugElement.query(By.css('ol'));
    const items = list.queryAll(By.css('li'));
    expect(items.map(item => (item.nativeElement as HTMLElement).textContent)).toEqual([
      jasmine.stringContaining('A very long caller-supplied milestone title'),
      jasmine.stringContaining('Second milestone'),
    ]);
  });

  it('presents caller-labelled overdue status without inferring lateness', () => {
    const first = fixture.debugElement.query(By.css('li')).nativeElement as HTMLElement;
    const time = fixture.debugElement.query(By.css('time')).nativeElement as HTMLTimeElement;
    expect(first.textContent).toContain('Overdue');
    expect(time.dateTime).toBe('2026-08-15');
    expect(first.querySelector('a')?.getAttribute('href')).toBe('/milestones/first');
  });

  it('handles missing dates with visible fallback text', () => {
    const items = fixture.debugElement.queryAll(By.css('li'));
    expect((items[1].nativeElement as HTMLElement).textContent).toContain('Due date not provided');
  });

  it('renders an explicit empty presentation', () => {
    host.milestones = [];
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('ol'))).toBeNull();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('No milestones to show.');
  });
});
