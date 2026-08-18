import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { PaginationComponent } from './pagination.component';

@Component({
  standalone: true,
  imports: [PaginationComponent],
  template: `
    <lsd-pagination
      accessibleName="Results pages"
      [currentPage]="currentPage()"
      [totalPages]="totalPages"
      (pageChange)="requestedPage = $event" />
  `,
})
class PaginationTestHostComponent {
  readonly currentPage = signal(1);
  totalPages = 5;
  requestedPage: number | null = null;
}

describe('PaginationComponent', () => {
  let fixture: ComponentFixture<PaginationTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [PaginationTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(PaginationTestHostComponent);
    fixture.detectChanges();
  });

  const buttons = (): HTMLButtonElement[] =>
    fixture.debugElement.queryAll(By.css('button')).map(({ nativeElement }) => nativeElement as HTMLButtonElement);

  it('exposes the first-page state and does not emit beyond the lower boundary', () => {
    expect(buttons()[0].disabled).toBeTrue();
    expect(buttons()[1].disabled).toBeFalse();
    expect(fixture.debugElement.query(By.css('[aria-current="page"]')).nativeElement.textContent).toContain('Page 1 of 5');

    buttons()[0].click();
    expect(fixture.componentInstance.requestedPage).toBeNull();
  });

  it('exposes the last-page state and does not emit beyond the upper boundary', () => {
    fixture.componentInstance.currentPage.set(5);
    fixture.detectChanges();

    expect(buttons()[0].disabled).toBeFalse();
    expect(buttons()[1].disabled).toBeTrue();
    buttons()[1].click();
    expect(fixture.componentInstance.requestedPage).toBeNull();
  });

  it('uses keyboard-native buttons to emit intent without changing controlled state', () => {
    fixture.componentInstance.currentPage.set(3);
    fixture.detectChanges();
    const next = buttons()[1];
    next.focus();
    const enter = new KeyboardEvent('keydown', { key: 'Enter', bubbles: true, cancelable: true });

    expect(document.activeElement).toBe(next);
    expect(next.dispatchEvent(enter)).toBeTrue();
    expect(enter.defaultPrevented).toBeFalse();
    next.click();
    expect(fixture.componentInstance.requestedPage).toBe(4);
    expect(fixture.componentInstance.currentPage()).toBe(3);
  });
});
