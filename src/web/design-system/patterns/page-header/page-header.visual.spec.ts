export const pageHeaderVisualCases = [
  { appearance: 'light', viewport: 'desktop', state: 'full' },
  { appearance: 'dark', viewport: 'desktop', state: 'metadata' },
  { appearance: 'light', viewport: 'mobile', state: 'actions-wrapped' },
] as const;

describe('page header visual regression cases', () => {
  it('covers appearance and narrow action wrapping', () => {
    expect(new Set(pageHeaderVisualCases.map((item) => item.appearance))).toEqual(new Set(['light', 'dark']));
    expect(new Set(pageHeaderVisualCases.map((item) => item.viewport))).toEqual(new Set(['desktop', 'mobile']));
    expect(pageHeaderVisualCases).toContain({ appearance: 'light', viewport: 'mobile', state: 'actions-wrapped' });
  });
});
