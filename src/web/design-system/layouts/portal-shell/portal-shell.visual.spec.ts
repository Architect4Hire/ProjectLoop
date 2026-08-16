export const portalShellVisualCases = [
  { appearance: 'light', viewport: 'desktop', navigation: 'rail' },
  { appearance: 'dark', viewport: 'desktop', navigation: 'rail' },
  { appearance: 'light', viewport: 'mobile', navigation: 'horizontal' },
  { appearance: 'dark', viewport: 'mobile', navigation: 'horizontal' },
] as const;

describe('portal shell visual regression cases', () => {
  it('covers persistent desktop and horizontal mobile navigation', () => {
    expect(new Set(portalShellVisualCases.map((item) => item.appearance))).toEqual(new Set(['light', 'dark']));
    expect(new Set(portalShellVisualCases.map((item) => item.viewport))).toEqual(new Set(['desktop', 'mobile']));
    expect(new Set(portalShellVisualCases.map((item) => item.navigation))).toEqual(new Set(['rail', 'horizontal']));
  });
});
