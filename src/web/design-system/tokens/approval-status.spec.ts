import {
  approvalStatuses,
  approvalStatusPresentation,
  type ApprovalStatus,
  type ApprovalStatusLabels,
} from './approval-status';

describe('approval status presentation', () => {
  it('maps every approval state to its semantic Badge presentation', () => {
    const expected = {
      requested: { label: 'Requested', variant: 'info' },
      approved: { label: 'Approved', variant: 'approved' },
      rejected: { label: 'Rejected', variant: 'danger' },
      cancelled: { label: 'Cancelled', variant: 'neutral' },
      expired: { label: 'Expired', variant: 'warning' },
    } as const satisfies Record<ApprovalStatus, ReturnType<typeof approvalStatusPresentation>>;

    expect(approvalStatuses).toEqual(Object.keys(expected));
    for (const status of approvalStatuses) {
      expect(approvalStatusPresentation(status)).toEqual(expected[status]);
    }
  });

  it('accepts caller-localized labels without changing Badge semantics', () => {
    const labels: ApprovalStatusLabels = {
      requested: 'Solicitada',
      approved: 'Aprobada',
      rejected: 'Rechazada',
      cancelled: 'Cancelada',
      expired: 'Vencida',
    };

    expect(approvalStatusPresentation('approved', labels)).toEqual({
      label: 'Aprobada',
      variant: 'approved',
    });
  });
});
