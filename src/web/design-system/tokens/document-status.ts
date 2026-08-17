import type { BadgeVariant } from '../primitives/badge/badge.component';

export const documentStatuses = [
  'draft',
  'published',
  'superseded',
  'archived',
  'unavailable',
] as const;

export type DocumentStatus = (typeof documentStatuses)[number];

export interface DocumentStatusPresentation {
  readonly label: string;
  readonly variant: BadgeVariant;
}

export type DocumentStatusLabels = Readonly<Record<DocumentStatus, string>>;

export const defaultDocumentStatusLabels: DocumentStatusLabels = {
  draft: 'Draft',
  published: 'Published',
  superseded: 'Superseded',
  archived: 'Archived',
  unavailable: 'Unavailable',
};

export const documentStatusBadgeVariants: Readonly<Record<DocumentStatus, BadgeVariant>> = {
  draft: 'info',
  published: 'approved',
  superseded: 'deprecated',
  archived: 'archived',
  unavailable: 'neutral',
};

export function documentStatusPresentation(
  status: DocumentStatus,
  labels: DocumentStatusLabels = defaultDocumentStatusLabels,
): DocumentStatusPresentation {
  return {
    label: labels[status],
    variant: documentStatusBadgeVariants[status],
  };
}
