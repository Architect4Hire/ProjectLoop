import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { ButtonComponent } from '../../primitives/button/button.component';
import { DocumentVersionHistoryActionsDirective } from './document-version-history-actions.directive';
import {
  DocumentVersionHistoryComponent,
  type DocumentHistoryVersion,
} from './document-version-history.component';

@Component({
  standalone: true,
  imports: [ButtonComponent, DocumentVersionHistoryActionsDirective, DocumentVersionHistoryComponent],
  template: `
    <lsd-document-version-history id="proposal-history" [versions]="versions()">
      <ng-template lsdDocumentVersionActions="v3" let-version>
        <button lsdButton type="button">View {{ version.versionLabel }}</button>
      </ng-template>
      <ng-template lsdDocumentVersionActions="v4" let-version>
        <button lsdButton type="button">Compare {{ version.versionLabel }}</button>
      </ng-template>
    </lsd-document-version-history>
  `,
})
class DocumentVersionHistoryTestHostComponent {
  readonly versions = signal<readonly DocumentHistoryVersion[]>([
    {
      id: 'v3',
      versionLabel: 'v3',
      qualifier: 'approved',
      actor: 'Avery Architect',
      occurredAt: '2026-08-15T16:30:00-05:00',
      timestampLabel: 'August 15, 2026 at 4:30 PM',
    },
    {
      id: 'v4',
      versionLabel: 'v4',
      qualifier: 'current',
      actor: 'Morgan Editor',
      occurredAt: '2026-08-16T09:00:00-05:00',
      timestampLabel: 'August 16, 2026 at 9:00 AM',
    },
  ]);
}

describe('DocumentVersionHistoryComponent', () => {
  let fixture: ComponentFixture<DocumentVersionHistoryTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [DocumentVersionHistoryTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(DocumentVersionHistoryTestHostComponent);
    fixture.detectChanges();
  });

  it('keeps v3 approved and v4 current visibly bound to separate history items', () => {
    const items = fixture.debugElement.queryAll(By.css('.lsd-document-version-history__item'));
    expect(items.length).toBe(2);
    expect(items[0].nativeElement.textContent).toContain('v3');
    expect(items[0].nativeElement.textContent).toContain('Approved');
    expect(items[0].nativeElement.textContent).not.toContain('Current');
    expect(items[1].nativeElement.textContent).toContain('v4');
    expect(items[1].nativeElement.textContent).toContain('Current');
    expect(items[1].nativeElement.textContent).not.toContain('Approved');
  });

  it('preserves caller-supplied chronological order, actor text, and native times', () => {
    const items = fixture.debugElement.queryAll(By.css('.lsd-document-version-history__item'));
    expect(items.map((item) => item.query(By.css('lsd-version-chip')).nativeElement.textContent.trim())).toEqual([
      'v3·Approved',
      'v4·Current',
    ]);
    expect(items[0].nativeElement.textContent).toContain('Avery Architect');
    expect(items[1].nativeElement.textContent).toContain('Morgan Editor');
    expect(items[0].query(By.css('time')).nativeElement.getAttribute('datetime')).toBe('2026-08-15T16:30:00-05:00');
  });

  it('renders each keyed action slot in its version context with an explicit label', () => {
    const groups = fixture.debugElement.queryAll(By.css('.lsd-document-version-history__actions'));
    expect(groups[0].nativeElement.getAttribute('aria-label')).toBe('Version actions for v3');
    expect(groups[0].nativeElement.textContent).toContain('View v3');
    expect(groups[1].nativeElement.getAttribute('aria-label')).toBe('Version actions for v4');
    expect(groups[1].nativeElement.textContent).toContain('Compare v4');
  });

  it('does not mutate the supplied versions', () => {
    expect(fixture.componentInstance.versions().map((version) => version.id)).toEqual(['v3', 'v4']);
    expect(fixture.componentInstance.versions()[0].qualifier).toBe('approved');
    expect(fixture.componentInstance.versions()[1].qualifier).toBe('current');
  });
});
