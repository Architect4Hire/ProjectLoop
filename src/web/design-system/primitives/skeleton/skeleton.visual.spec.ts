export interface SkeletonVisualCase {
  readonly name: string;
  readonly reducedMotion: boolean;
  readonly lines: number;
}

export const skeletonVisualCases: readonly SkeletonVisualCase[] = [
  { name: 'default-motion', reducedMotion: false, lines: 3 },
  { name: 'reduced-motion-static', reducedMotion: true, lines: 3 },
];

describe('Skeleton visual coverage', () => {
  it('includes a reduced-motion case', () => {
    expect(skeletonVisualCases.some((item) => item.reducedMotion)).toBeTrue();
  });
});
