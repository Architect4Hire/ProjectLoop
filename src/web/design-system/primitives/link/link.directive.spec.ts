import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { LinkDirective } from './link.directive';

@Component({
  standalone: true,
  imports: [LinkDirective],
  template: `
    <a lsdLink [href]="href" [tone]="tone()" [impact]="impact()" [size]="size()" [shape]="shape()">
      Projected destination
    </a>
    <a
      lsdLink
      href="https://example.com/guide"
      target="_blank"
      rel="noopener noreferrer"
      aria-label="External guide (opens in a new tab)">
      External guide
    </a>
  `,
})
class LinkTestHostComponent {
  href = '/projects/42';
  readonly tone = signal<'primary' | 'danger'>('primary');
  readonly impact = signal<'bold' | 'light' | 'minimal'>('minimal');
  readonly size = signal<'small' | 'medium' | 'large'>('medium');
  readonly shape = signal<'square' | 'rounded' | 'pill'>('rounded');
}

describe('LinkDirective', () => {
  let fixture: ComponentFixture<LinkTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [LinkTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(LinkTestHostComponent);
    fixture.detectChanges();
  });

  const links = (): HTMLAnchorElement[] =>
    fixture.debugElement.queryAll(By.css('a')).map(({ nativeElement }) => nativeElement as HTMLAnchorElement);

  it('preserves native anchor semantics, href, and projected content', () => {
    const link = links()[0];
    expect(link.tagName).toBe('A');
    expect(link.getAttribute('href')).toBe('/projects/42');
    expect(link.textContent).toContain('Projected destination');
  });

  it('remains keyboard focusable when it has an href', () => {
    const link = links()[0];
    link.focus();
    expect(link.tabIndex).toBe(0);
    expect(document.activeElement).toBe(link);
  });

  it('preserves explicit external-link navigation and labeling', () => {
    const link = links()[1];
    expect(link.target).toBe('_blank');
    expect(link.rel).toBe('noopener noreferrer');
    expect(link.getAttribute('aria-label')).toBe('External guide (opens in a new tab)');
  });

  it('applies the selected Button-aligned visual variants', () => {
    const host = fixture.componentInstance;
    host.tone.set('danger');
    host.impact.set('bold');
    host.size.set('large');
    host.shape.set('pill');
    fixture.detectChanges();

    const link = links()[0];
    expect(link.classList).toContain('bg-status-danger');
    expect(link.classList).toContain('min-h-12');
    expect(link.classList).toContain('rounded-full');
  });
});
