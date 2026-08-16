interface MilestoneListVisualCase {
  readonly name: string;
  readonly appearance: 'light' | 'dark';
  readonly viewport: 'desktop' | 'mobile';
  readonly presentation: 'empty' | 'overdue-labelled' | 'long-title';
}

export const milestoneListVisualCases: readonly MilestoneListVisualCase[] = [
  { name: 'overdue-light-desktop', appearance: 'light', viewport: 'desktop', presentation: 'overdue-labelled' },
  { name: 'long-title-dark-desktop', appearance: 'dark', viewport: 'desktop', presentation: 'long-title' },
  { name: 'long-title-light-mobile', appearance: 'light', viewport: 'mobile', presentation: 'long-title' },
  { name: 'empty-dark-mobile', appearance: 'dark', viewport: 'mobile', presentation: 'empty' },
];

describe('Milestone list visual coverage', () => {
  it('covers required presentations, widths, and appearances', () => {
    expect(new Set(milestoneListVisualCases.map(item => item.presentation))).toEqual(new Set(['empty', 'overdue-labelled', 'long-title']));
    expect(new Set(milestoneListVisualCases.map(item => item.viewport))).toEqual(new Set(['desktop', 'mobile']));
    expect(new Set(milestoneListVisualCases.map(item => item.appearance))).toEqual(new Set(['light', 'dark']));
  });
});
