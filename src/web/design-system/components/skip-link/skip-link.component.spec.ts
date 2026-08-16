import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { SkipLinkComponent } from './skip-link.component';

@Component({
  standalone: true,
  imports: [SkipLinkComponent],
  template: `
    <lsd-skip-link />
    @if (showMain) { <main id="main-content" tabindex="-1">Application content</main> }
  `,
})
class SkipLinkTestHostComponent {
  showMain = true;
}

describe('SkipLinkComponent', () => {
  let fixture: ComponentFixture<SkipLinkTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [SkipLinkTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(SkipLinkTestHostComponent);
    fixture.detectChanges();
  });

  const link = (): HTMLAnchorElement =>
    fixture.debugElement.query(By.css('a')).nativeElement as HTMLAnchorElement;

  it('uses the Link primitive and native keyboard-activatable anchor semantics', () => {
    const anchor = link();
    anchor.focus();
    const enter = new KeyboardEvent('keydown', { key: 'Enter', bubbles: true, cancelable: true });

    expect(anchor.matches('a[lsdLink]')).toBeTrue();
    expect(anchor.getAttribute('href')).toBe('#main-content');
    expect(anchor.tabIndex).toBe(0);
    expect(document.activeElement).toBe(anchor);
    expect(anchor.dispatchEvent(enter)).toBeTrue();
    expect(enter.defaultPrevented).toBeFalse();
    expect(anchor.textContent?.trim()).toBe('Skip to main content');
  });

  it('does not intercept activation when the main-content target is missing', () => {
    fixture.componentInstance.showMain = false;
    fixture.detectChanges();
    const anchor = link();
    let defaultPrevented = false;
    anchor.addEventListener('click', (event) => { defaultPrevented = event.defaultPrevented; });

    expect(() => anchor.click()).not.toThrow();
    expect(defaultPrevented).toBeFalse();
    expect(document.getElementById('main-content')).toBeNull();
  });
});
