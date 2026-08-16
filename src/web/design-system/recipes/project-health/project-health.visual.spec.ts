import type { ProjectHealthStatus } from './project-health.component';

interface ProjectHealthVisualCase {
  readonly name: string;
  readonly appearance: 'light' | 'dark';
  readonly viewport: 'desktop' | 'mobile';
  readonly status: ProjectHealthStatus;
  readonly indicators: 'none' | 'multiple';
}

export const projectHealthVisualCases: readonly ProjectHealthVisualCase[] = [
  { name: 'healthy-light-desktop', appearance: 'light', viewport: 'desktop', status: 'healthy', indicators: 'multiple' },
  { name: 'attention-dark-desktop', appearance: 'dark', viewport: 'desktop', status: 'attention', indicators: 'multiple' },
  { name: 'at-risk-light-mobile', appearance: 'light', viewport: 'mobile', status: 'at-risk', indicators: 'multiple' },
  { name: 'unknown-dark-mobile', appearance: 'dark', viewport: 'mobile', status: 'unknown', indicators: 'none' },
];

describe('Project health visual coverage', () => {
  it('covers every status, appearance, width, and optional indicators', () => {
    expect(new Set(projectHealthVisualCases.map(item => item.status))).toEqual(new Set(['healthy', 'attention', 'at-risk', 'unknown']));
    expect(new Set(projectHealthVisualCases.map(item => item.appearance))).toEqual(new Set(['light', 'dark']));
    expect(new Set(projectHealthVisualCases.map(item => item.viewport))).toEqual(new Set(['desktop', 'mobile']));
    expect(new Set(projectHealthVisualCases.map(item => item.indicators))).toEqual(new Set(['none', 'multiple']));
  });
});
