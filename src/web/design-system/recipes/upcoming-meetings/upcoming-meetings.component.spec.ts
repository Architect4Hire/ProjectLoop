import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { UpcomingMeetingsComponent, type MeetingSummary } from './upcoming-meetings.component';

@Component({
  standalone: true,
  imports: [UpcomingMeetingsComponent],
  template: `<lsd-upcoming-meetings [meetings]="meetings" />`,
})
class UpcomingMeetingsTestHostComponent {
  meetings: readonly MeetingSummary[] = [
    {
      id: 'planning',
      title: 'A long planning conversation title that remains readable in a narrow container',
      time: { label: 'Monday, August 17 at 9:30 AM CDT', dateTime: '2026-08-17T09:30:00-05:00' },
      location: 'Video channel',
      action: { label: 'Join', href: '/authorized/join', accessibleLabel: 'Join planning conversation' },
    },
    {
      id: 'review',
      title: 'Review conversation',
      time: { label: 'Tuesday, August 18 at 2:00 PM CDT' },
      location: 'Room 204',
    },
  ];
}

describe('UpcomingMeetingsComponent', () => {
  let fixture: ComponentFixture<UpcomingMeetingsTestHostComponent>;
  let host: UpcomingMeetingsTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [UpcomingMeetingsTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(UpcomingMeetingsTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('renders caller-localized time and location text without conversion', () => {
    const first = fixture.debugElement.query(By.css('li')).nativeElement as HTMLElement;
    const time = fixture.debugElement.query(By.css('time')).nativeElement as HTMLTimeElement;
    expect(first.textContent).toContain('Monday, August 17 at 9:30 AM CDT');
    expect(first.textContent).toContain('Video channel');
    expect(time.dateTime).toBe('2026-08-17T09:30:00-05:00');
  });

  it('renders only caller-authorized native action links', () => {
    const actions = fixture.debugElement.queryAll(By.css('a[lsdLink]'));
    expect(actions).toHaveSize(1);
    const action = actions[0].nativeElement as HTMLAnchorElement;
    expect(action.getAttribute('href')).toBe('/authorized/join');
    expect(action.getAttribute('aria-label')).toBe('Join planning conversation');
  });

  it('keeps long titles available to the narrow-layout wrapping hook', () => {
    const title = fixture.debugElement.query(By.css('.lsd-upcoming-meetings__title')).nativeElement as HTMLElement;
    expect(title.textContent).toContain('A long planning conversation title');
    expect(title.classList).toContain('lsd-upcoming-meetings__title');
  });

  it('renders an explicit no-meetings state', () => {
    host.meetings = [];
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('ul'))).toBeNull();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('No upcoming meetings.');
  });
});
