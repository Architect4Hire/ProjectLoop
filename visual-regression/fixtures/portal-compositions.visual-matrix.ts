export type VisualAppearance = 'light' | 'dark';
export type VisualViewport = 'desktop' | 'mobile';
export type VisualComposition = 'shell' | 'dashboard' | 'documents' | 'approval' | 'audit';

export interface PortalCompositionVisualCase {
  readonly name: string;
  readonly composition: VisualComposition;
  readonly appearance: VisualAppearance;
  readonly viewport: VisualViewport;
  readonly state: string;
}

export const portalCompositionVisualMatrix: readonly PortalCompositionVisualCase[] = [
  { name: 'shell-light-desktop-long-content', composition: 'shell', appearance: 'light', viewport: 'desktop', state: 'long' },
  { name: 'shell-dark-mobile-compact-navigation', composition: 'shell', appearance: 'dark', viewport: 'mobile', state: 'compact' },
  { name: 'dashboard-light-desktop-populated', composition: 'dashboard', appearance: 'light', viewport: 'desktop', state: 'long' },
  { name: 'dashboard-dark-mobile-independent-feedback', composition: 'dashboard', appearance: 'dark', viewport: 'mobile', state: 'feedback' },
  { name: 'documents-light-desktop-long-confidential', composition: 'documents', appearance: 'light', viewport: 'desktop', state: 'long' },
  { name: 'documents-dark-mobile-empty', composition: 'documents', appearance: 'dark', viewport: 'mobile', state: 'empty' },
  { name: 'approval-light-desktop-version-bound', composition: 'approval', appearance: 'light', viewport: 'desktop', state: 'long' },
  { name: 'approval-dark-mobile-processing', composition: 'approval', appearance: 'dark', viewport: 'mobile', state: 'processing' },
  { name: 'audit-light-desktop-many-events', composition: 'audit', appearance: 'light', viewport: 'desktop', state: 'long' },
  { name: 'audit-dark-mobile-load-more', composition: 'audit', appearance: 'dark', viewport: 'mobile', state: 'loading' },
] as const;
