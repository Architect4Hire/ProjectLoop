import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ConfirmationDialogComponent, type ConfirmationCancelReason } from './confirmation-dialog.component';

@Component({
  standalone: true,
  imports: [ConfirmationDialogComponent],
  template: `
    <button id="opener" type="button">Open confirmation</button>
    <lsd-confirmation-dialog
      id="consequence-confirmation"
      title="Confirm action"
      consequenceText="This change affects all collaborators."
      actionLabel="Apply change"
      actionTone="warning"
      [open]="open()"
      [processing]="processing()"
      (confirmed)="confirm()"
      (cancelled)="cancel($event)" />
  `,
})
class ConfirmationDialogTestHostComponent {
  readonly open = signal(false);
  readonly processing = signal(false);
  confirmCount = 0;
  cancelReasons: ConfirmationCancelReason[] = [];
  cancel(reason: ConfirmationCancelReason): void { this.cancelReasons.push(reason); this.open.set(false); }
  confirm(): void { this.confirmCount += 1; }
}

describe('ConfirmationDialogComponent', () => {
  let fixture: ComponentFixture<ConfirmationDialogTestHostComponent>;
  let host: ConfirmationDialogTestHostComponent;
  let dialog: HTMLDialogElement;
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ConfirmationDialogTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(ConfirmationDialogTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
    dialog = fixture.debugElement.query(By.css('dialog')).nativeElement;
    Object.defineProperties(dialog, {
      showModal: { configurable: true, value: () => dialog.setAttribute('open', '') },
      close: { configurable: true, value: () => dialog.removeAttribute('open') },
    });
    spyOn(dialog, 'showModal').and.callFake(() => dialog.setAttribute('open', ''));
    spyOn(dialog, 'close').and.callFake(() => dialog.removeAttribute('open'));
  });

  it('opens modally and moves focus inside the confirmation dialog', async () => {
    const opener = fixture.debugElement.query(By.css('#opener')).nativeElement as HTMLButtonElement;
    opener.focus();
    host.open.set(true);
    fixture.detectChanges();
    await fixture.whenStable();
    expect(dialog.showModal).toHaveBeenCalled();
    expect(dialog.contains(document.activeElement)).toBeTrue();
  });

  it('emits an Escape cancellation intent and restores trigger focus', () => {
    const opener = fixture.debugElement.query(By.css('#opener')).nativeElement as HTMLButtonElement;
    opener.focus();
    host.open.set(true);
    fixture.detectChanges();
    dialog.dispatchEvent(new Event('cancel', { cancelable: true }));
    fixture.detectChanges();
    expect(host.cancelReasons).toEqual(['escape']);
    expect(document.activeElement).toBe(opener);
  });

  it('locks dismissal and both actions while processing', () => {
    host.open.set(true);
    host.processing.set(true);
    fixture.detectChanges();
    dialog.dispatchEvent(new Event('cancel', { cancelable: true }));
    const buttons = fixture.debugElement.queryAll(By.css('.lsd-confirmation-dialog__actions button'));
    expect(buttons.every((button) => (button.nativeElement as HTMLButtonElement).disabled)).toBeTrue();
    expect(host.cancelReasons).toEqual([]);
    expect(host.confirmCount).toBe(0);
    expect(fixture.nativeElement.textContent).toContain('Processing');
  });

  it('emits caller-owned confirmation and explicit cancel-button intents', () => {
    host.open.set(true);
    fixture.detectChanges();
    const buttons = fixture.debugElement.queryAll(By.css('.lsd-confirmation-dialog__actions button'));
    (buttons[1].nativeElement as HTMLButtonElement).click();
    expect(host.confirmCount).toBe(1);
    (buttons[0].nativeElement as HTMLButtonElement).click();
    expect(host.cancelReasons).toEqual(['cancel-button']);
  });
});
