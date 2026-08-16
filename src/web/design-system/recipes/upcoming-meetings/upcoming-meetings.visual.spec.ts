interface UpcomingMeetingsVisualCase {
  readonly name: string;
  readonly appearance: 'light' | 'dark';
  readonly viewport: 'desktop' | 'mobile';
  readonly state: 'meetings' | 'empty';
}

export const upcomingMeetingsVisualCases: readonly UpcomingMeetingsVisualCase[] = [
  { name: 'meetings-light-desktop', appearance: 'light', viewport: 'desktop', state: 'meetings' },
  { name: 'meetings-dark-desktop', appearance: 'dark', viewport: 'desktop', state: 'meetings' },
  { name: 'long-meeting-light-mobile', appearance: 'light', viewport: 'mobile', state: 'meetings' },
  { name: 'empty-dark-mobile', appearance: 'dark', viewport: 'mobile', state: 'empty' },
];

describe('Upcoming meetings visual coverage', () => {
  it('covers meetings, empty state, appearances, and narrow layout', () => {
    expect(new Set(upcomingMeetingsVisualCases.map(item => item.state))).toEqual(new Set(['meetings', 'empty']));
    expect(new Set(upcomingMeetingsVisualCases.map(item => item.appearance))).toEqual(new Set(['light', 'dark']));
    expect(new Set(upcomingMeetingsVisualCases.map(item => item.viewport))).toEqual(new Set(['desktop', 'mobile']));
  });
});
