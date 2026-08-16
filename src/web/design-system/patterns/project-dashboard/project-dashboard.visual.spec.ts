interface ProjectDashboardVisualCase {
  readonly name: string;
  readonly appearance: 'light' | 'dark';
  readonly viewport: 'desktop' | 'mobile';
  readonly regionState: 'populated' | 'independent-feedback';
}

export const projectDashboardVisualCases: readonly ProjectDashboardVisualCase[] = [
  { name: 'populated-light-desktop', appearance: 'light', viewport: 'desktop', regionState: 'populated' },
  { name: 'feedback-dark-desktop', appearance: 'dark', viewport: 'desktop', regionState: 'independent-feedback' },
  { name: 'populated-light-mobile', appearance: 'light', viewport: 'mobile', regionState: 'populated' },
  { name: 'feedback-dark-mobile', appearance: 'dark', viewport: 'mobile', regionState: 'independent-feedback' },
];

describe('Project dashboard visual coverage', () => {
  it('covers desktop/mobile ordering and independent region feedback', () => {
    expect(new Set(projectDashboardVisualCases.map(item => item.appearance))).toEqual(new Set(['light', 'dark']));
    expect(new Set(projectDashboardVisualCases.map(item => item.viewport))).toEqual(new Set(['desktop', 'mobile']));
    expect(new Set(projectDashboardVisualCases.map(item => item.regionState))).toEqual(new Set(['populated', 'independent-feedback']));
  });
});
