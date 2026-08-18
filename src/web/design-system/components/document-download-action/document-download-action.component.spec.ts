import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DocumentDownloadActionComponent, type DocumentDownloadIntent, type DocumentDownloadState } from './document-download-action.component';

@Component({ standalone: true, imports: [DocumentDownloadActionComponent], template: `
  <lsd-document-download-action id="architecture-download" documentLabel="Architecture plan v4"
    [state]="state()" [progressValue]="55" failureMessage="The authorized download could not be prepared."
    (actionIntent)="intent = $event" />` })
class Host { readonly state = signal<DocumentDownloadState>('ready'); intent: DocumentDownloadIntent | null = null; }

describe('DocumentDownloadActionComponent', () => {
  let fixture: ComponentFixture<Host>; let host: Host;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports: [Host] }).compileComponents(); fixture = TestBed.createComponent(Host); host = fixture.componentInstance; fixture.detectChanges(); });
  it('announces preparing, downloading, failed, and unavailable states', () => {
    const expected: ReadonlyArray<[DocumentDownloadState, string, string]> = [
      ['preparing', 'Preparing Architecture plan v4 for download.', 'status'],
      ['downloading', 'Downloading Architecture plan v4.', 'status'],
      ['failed', 'The authorized download could not be prepared.', 'alert'],
      ['unavailable', 'Download unavailable.', 'status'],
    ];
    for (const [state, text, role] of expected) {
      host.state.set(state); fixture.detectChanges();
      const announcement = fixture.debugElement.query(By.css('.lsd-document-download-action__announcement')).nativeElement as HTMLElement;
      expect(announcement.textContent).toContain(text); expect(announcement.getAttribute('role')).toBe(role);
      expect(announcement.getAttribute('aria-live')).toBe(state === 'failed' ? 'assertive' : 'polite');
    }
  });
  it('renders native progress semantics while downloading', () => {
    host.state.set('downloading'); fixture.detectChanges(); const progress = fixture.debugElement.query(By.css('progress')).nativeElement as HTMLProgressElement;
    expect(progress.value).toBe(55); expect(progress.getAttribute('aria-valuetext')).toBe('55%');
  });
  it('emits retry intent only from failed state', () => {
    host.state.set('failed'); fixture.detectChanges(); fixture.debugElement.query(By.css('button')).nativeElement.click();
    expect(host.intent).toEqual({ type: 'retry' });
  });
  it('contains no URL input and emits retrieval intent from ready state', () => {
    fixture.debugElement.query(By.css('button')).nativeElement.click(); expect(host.intent).toEqual({ type: 'download' });
    expect(fixture.debugElement.query(By.css('a'))).toBeNull();
  });
});
