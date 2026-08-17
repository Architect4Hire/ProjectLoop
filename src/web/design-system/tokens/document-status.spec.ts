import {
  documentStatuses,
  documentStatusPresentation,
  type DocumentStatus,
  type DocumentStatusLabels,
} from './document-status';

describe('document status presentation', () => {
  it('maps every document state to its semantic Badge presentation', () => {
    const expected = {
      draft: { label: 'Draft', variant: 'info' },
      published: { label: 'Published', variant: 'approved' },
      superseded: { label: 'Superseded', variant: 'deprecated' },
      archived: { label: 'Archived', variant: 'archived' },
      unavailable: { label: 'Unavailable', variant: 'neutral' },
    } as const satisfies Record<DocumentStatus, ReturnType<typeof documentStatusPresentation>>;

    expect(documentStatuses).toEqual(Object.keys(expected));
    for (const status of documentStatuses) {
      expect(documentStatusPresentation(status)).toEqual(expected[status]);
    }
  });

  it('accepts caller-localized labels without changing Badge semantics', () => {
    const labels: DocumentStatusLabels = {
      draft: 'Borrador',
      published: 'Publicado',
      superseded: 'Reemplazado',
      archived: 'Archivado',
      unavailable: 'No disponible',
    };

    expect(documentStatusPresentation('published', labels)).toEqual({
      label: 'Publicado',
      variant: 'approved',
    });
  });
});
