import type { BadgeVariant } from '../primitives/badge/badge.component';

export const approvalStatuses = [
  'requested',
  'approved',
  'rejected',
  'cancelled',
  'expired',
] as const;

export type ApprovalStatus = (typeof approvalStatuses)[number];

export interface ApprovalStatusPresentation {
  readonly label: string;
  readonly variant: BadgeVariant;
}

export type ApprovalStatusLabels = Readonly<Record<ApprovalStatus, string>>;

export const defaultApprovalStatusLabels: ApprovalStatusLabels = {
  requested: 'Requested',
  approved: 'Approved',
  rejected: 'Rejected',
  cancelled: 'Cancelled',
  expired: 'Expired',
};

export const approvalStatusBadgeVariants: Readonly<Record<ApprovalStatus, BadgeVariant>> = {
  requested: 'info',
  approved: 'approved',
  rejected: 'danger',
  cancelled: 'neutral',
  expired: 'warning',
};

export function approvalStatusPresentation(
  status: ApprovalStatus,
  labels: ApprovalStatusLabels = defaultApprovalStatusLabels,
): ApprovalStatusPresentation {
  return {
    label: labels[status],
    variant: approvalStatusBadgeVariants[status],
  };
}
