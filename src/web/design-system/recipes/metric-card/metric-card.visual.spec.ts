interface MetricCardVisualCase {
  readonly name: string;
  readonly appearance: 'light' | 'dark';
  readonly viewport: 'desktop' | 'mobile';
  readonly state: 'default' | 'long-value' | 'loading';
}

export const metricCardVisualCases: readonly MetricCardVisualCase[] = [
  { name: 'default-light-desktop', appearance: 'light', viewport: 'desktop', state: 'default' },
  { name: 'default-dark-desktop', appearance: 'dark', viewport: 'desktop', state: 'default' },
  { name: 'long-value-light-mobile', appearance: 'light', viewport: 'mobile', state: 'long-value' },
  { name: 'loading-dark-mobile', appearance: 'dark', viewport: 'mobile', state: 'loading' },
];

describe('Metric card visual coverage', () => {
  it('covers appearances, widths, long values, and loading', () => {
    expect(new Set(metricCardVisualCases.map(item => item.appearance))).toEqual(new Set(['light', 'dark']));
    expect(new Set(metricCardVisualCases.map(item => item.viewport))).toEqual(new Set(['desktop', 'mobile']));
    expect(new Set(metricCardVisualCases.map(item => item.state))).toEqual(new Set(['default', 'long-value', 'loading']));
  });
});
