interface RecentDecisionsVisualCase {
  readonly name: string;
  readonly appearance: 'light' | 'dark';
  readonly viewport: 'desktop' | 'mobile';
  readonly state: 'empty' | 'mixed-status';
}

export const recentDecisionsVisualCases: readonly RecentDecisionsVisualCase[] = [
  { name: 'mixed-light-desktop', appearance: 'light', viewport: 'desktop', state: 'mixed-status' },
  { name: 'mixed-dark-desktop', appearance: 'dark', viewport: 'desktop', state: 'mixed-status' },
  { name: 'mixed-light-mobile', appearance: 'light', viewport: 'mobile', state: 'mixed-status' },
  { name: 'empty-dark-mobile', appearance: 'dark', viewport: 'mobile', state: 'empty' },
];

describe('Recent decisions visual coverage', () => {
  it('covers empty and mixed-status lists at desktop and mobile widths', () => {
    expect(new Set(recentDecisionsVisualCases.map(item => item.state))).toEqual(new Set(['empty', 'mixed-status']));
    expect(new Set(recentDecisionsVisualCases.map(item => item.appearance))).toEqual(new Set(['light', 'dark']));
    expect(new Set(recentDecisionsVisualCases.map(item => item.viewport))).toEqual(new Set(['desktop', 'mobile']));
  });
});
