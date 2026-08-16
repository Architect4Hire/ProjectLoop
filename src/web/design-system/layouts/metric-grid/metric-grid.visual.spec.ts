interface MetricGridVisualCase {
  readonly name: string;
  readonly appearance: 'light' | 'dark';
  readonly viewport: 'desktop' | 'tablet' | 'mobile';
  readonly itemCount: 1 | 2 | 3 | 4;
}

export const metricGridVisualCases: readonly MetricGridVisualCase[] = [
  { name: 'one-light-desktop', appearance: 'light', viewport: 'desktop', itemCount: 1 },
  { name: 'two-dark-desktop', appearance: 'dark', viewport: 'desktop', itemCount: 2 },
  { name: 'three-light-tablet', appearance: 'light', viewport: 'tablet', itemCount: 3 },
  { name: 'four-dark-tablet', appearance: 'dark', viewport: 'tablet', itemCount: 4 },
  { name: 'four-light-mobile', appearance: 'light', viewport: 'mobile', itemCount: 4 },
  { name: 'one-dark-mobile', appearance: 'dark', viewport: 'mobile', itemCount: 1 },
];

describe('Metric grid visual coverage', () => {
  it('covers one through four items across responsive widths and appearances', () => {
    expect(new Set(metricGridVisualCases.map(item => item.itemCount))).toEqual(new Set([1, 2, 3, 4]));
    expect(new Set(metricGridVisualCases.map(item => item.viewport))).toEqual(new Set(['desktop', 'tablet', 'mobile']));
    expect(new Set(metricGridVisualCases.map(item => item.appearance))).toEqual(new Set(['light', 'dark']));
  });
});
