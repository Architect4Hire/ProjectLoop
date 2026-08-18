import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DocumentUploadComponent, type DocumentUploadIntent, type DocumentUploadState } from './document-upload.component';

@Component({ standalone: true, imports: [DocumentUploadComponent], template: `
  <lsd-document-upload id="document-upload" accept="application/pdf" [maxFileSizeBytes]="10"
    filePolicyDescription="PDF files up to 10 bytes"
    [categoryOptions]="[{ value: 'architecture', label: 'Architecture' }]"
    [visibilityOptions]="[{ value: 'confidential', label: 'Confidential' }]"
    [state]="state()" [progressValue]="40" [failureMessage]="failureMessage"
    (uploadIntent)="intent = $event" />` })
class Host {
  readonly state = signal<DocumentUploadState>('idle');
  failureMessage = 'Network interrupted. Your file was not uploaded.';
  intent: DocumentUploadIntent | null = null;
}

describe('DocumentUploadComponent', () => {
  let fixture: ComponentFixture<Host>; let host: Host;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports: [Host] }).compileComponents(); fixture = TestBed.createComponent(Host); host = fixture.componentInstance; fixture.detectChanges(); });
  it('displays validation without emitting upload intent', () => {
    fixture.debugElement.query(By.css('form')).nativeElement.dispatchEvent(new SubmitEvent('submit', { bubbles: true, cancelable: true })); fixture.detectChanges();
    const text = fixture.debugElement.queryAll(By.css('[role="alert"]')).map((item) => item.nativeElement.textContent).join(' ');
    expect(text).toContain('Select a file that meets the upload policy.'); expect(text).toContain('Enter a document title.');
    expect(text).toContain('Select a document category.'); expect(text).toContain('Select document visibility.'); expect(host.intent).toBeNull();
  });
  it('renders caller-owned progress with native semantics', () => {
    host.state.set('uploading'); fixture.detectChanges(); const progress = fixture.debugElement.query(By.css('progress')).nativeElement as HTMLProgressElement;
    expect(progress.value).toBe(40); expect(progress.max).toBe(100); expect(fixture.nativeElement.textContent).toContain('40%');
  });
  it('emits cancellation intent only', () => {
    host.state.set('uploading'); fixture.detectChanges(); fixture.debugElement.query(By.css('.lsd-document-upload__actions button')).nativeElement.click(); expect(host.intent).toEqual({ type: 'cancel' });
  });
  it('announces failure and emits retry intent for caller recovery', () => {
    host.state.set('failed'); fixture.detectChanges(); const failure = fixture.debugElement.query(By.css('#document-upload-failure')).nativeElement as HTMLElement;
    expect(failure.getAttribute('role')).toBe('alert'); expect(failure.textContent).toContain('Network interrupted');
    fixture.debugElement.query(By.css('.lsd-document-upload__actions button')).nativeElement.click(); expect(host.intent).toEqual({ type: 'retry' });
  });
  it('announces caller-confirmed completion without presenting another upload action', () => {
    host.state.set('completed'); fixture.detectChanges();
    const status = fixture.debugElement.query(By.css('[role="status"]')).nativeElement as HTMLElement;
    expect(status.getAttribute('aria-live')).toBe('polite');
    expect(fixture.debugElement.query(By.css('#document-upload-completed')).nativeElement.textContent).toContain('Upload completed');
    const upload = fixture.debugElement.queryAll(By.css('button'))
      .map((item) => item.nativeElement as HTMLButtonElement)
      .find((button) => button.textContent?.includes('Upload document'));
    expect(upload?.disabled).toBeTrue();
  });
});
