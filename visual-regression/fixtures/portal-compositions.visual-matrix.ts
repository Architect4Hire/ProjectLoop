export type VisualAppearance = 'light' | 'dark';
export type VisualViewport = 'desktop' | 'mobile';
export type VisualComposition = 'shell' | 'dashboard' | 'documents' | 'document-detail' | 'approval' | 'audit' | 'controls' | 'fields' | 'choices' | 'overlays' | 'feedback';

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
  { name: 'dashboard-light-desktop-populated', composition: 'dashboard', appearance: 'light', viewport: 'desktop', state: 'populated' },
  { name: 'dashboard-dark-mobile-partial', composition: 'dashboard', appearance: 'dark', viewport: 'mobile', state: 'partial' },
  { name: 'dashboard-light-desktop-empty', composition: 'dashboard', appearance: 'light', viewport: 'desktop', state: 'empty' },
  { name: 'dashboard-dark-mobile-loading', composition: 'dashboard', appearance: 'dark', viewport: 'mobile', state: 'loading' },
  { name: 'dashboard-light-desktop-error', composition: 'dashboard', appearance: 'light', viewport: 'desktop', state: 'error' },
  { name: 'documents-light-desktop-many-long-metadata', composition: 'documents', appearance: 'light', viewport: 'desktop', state: 'many' },
  { name: 'documents-dark-mobile-empty', composition: 'documents', appearance: 'dark', viewport: 'mobile', state: 'empty' },
  { name: 'documents-dark-mobile-one-card', composition: 'documents', appearance: 'dark', viewport: 'mobile', state: 'one' },
  { name: 'document-detail-light-desktop-exact-versions', composition: 'document-detail', appearance: 'light', viewport: 'desktop', state: 'ready' },
  { name: 'document-detail-dark-mobile-downloading', composition: 'document-detail', appearance: 'dark', viewport: 'mobile', state: 'download-downloading' },
  { name: 'document-detail-light-desktop-upload-failure', composition: 'document-detail', appearance: 'light', viewport: 'desktop', state: 'upload-failed' },
  { name: 'approval-light-desktop-version-bound', composition: 'approval', appearance: 'light', viewport: 'desktop', state: 'pending' },
  { name: 'approval-dark-mobile-processing', composition: 'approval', appearance: 'dark', viewport: 'mobile', state: 'processing' },
  { name: 'approval-light-desktop-approved', composition: 'approval', appearance: 'light', viewport: 'desktop', state: 'approved' },
  { name: 'approval-dark-mobile-rejected', composition: 'approval', appearance: 'dark', viewport: 'mobile', state: 'rejected' },
  { name: 'approval-light-desktop-changes-requested', composition: 'approval', appearance: 'light', viewport: 'desktop', state: 'changes-requested' },
  { name: 'audit-light-desktop-many-events', composition: 'audit', appearance: 'light', viewport: 'desktop', state: 'long' },
  { name: 'audit-dark-mobile-load-more', composition: 'audit', appearance: 'dark', viewport: 'mobile', state: 'loading' },
  { name: 'audit-light-desktop-pages-long-identifiers', composition: 'audit', appearance: 'light', viewport: 'desktop', state: 'pages' },
  { name: 'controls-light-desktop-focus-visible', composition: 'controls', appearance: 'light', viewport: 'desktop', state: 'focus' },
  { name: 'controls-dark-mobile-states-reduced-motion', composition: 'controls', appearance: 'dark', viewport: 'mobile', state: 'states' },
  { name: 'fields-light-desktop-focus-and-states', composition: 'fields', appearance: 'light', viewport: 'desktop', state: 'focus' },
  { name: 'fields-dark-mobile-contrast-and-states', composition: 'fields', appearance: 'dark', viewport: 'mobile', state: 'states' },
  { name: 'feedback-light-desktop-state-family', composition: 'feedback', appearance: 'light', viewport: 'desktop', state: 'states' },
  { name: 'feedback-dark-mobile-state-family', composition: 'feedback', appearance: 'dark', viewport: 'mobile', state: 'states' },
] as const;
