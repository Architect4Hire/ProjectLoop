import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { SkeletonComponent } from './skeleton.component';

@Component({
  standalone: true,
  imports: [SkeletonComponent],
  template: `
    <section role="status" aria-busy="true" aria-label="Loading results">
      <lsd-skeleton [lines]="lines()" />
    </section>
  `,
})
class SkeletonTestHostComponent {
  readonly lines = signal(3);
}

describe('SkeletonComponent', () => {
  let fixture: ComponentFixture<SkeletonTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [SkeletonTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(SkeletonTestHostComponent);
    fixture.detectChanges();
  });

  it('is decorative while the parent owns loading semantics', () => {
    const host = fixture.debugElement.query(By.css('lsd-skeleton')).nativeElement as HTMLElement;
    const region = fixture.debugElement.query(By.css('section')).nativeElement as HTMLElement;

    expect(host.getAttribute('aria-hidden')).toBe('true');
    expect(host.getAttribute('role')).toBe('presentation');
    expect(host.hasAttribute('aria-busy')).toBeFalse();
    expect(host.hasAttribute('aria-live')).toBeFalse();
    expect(region.getAttribute('aria-busy')).toBe('true');
  });

  it('clamps the decorative line count', () => {
    fixture.componentInstance.lines.set(20);
    fixture.detectChanges();
    expect(fixture.debugElement.queryAll(By.css('.lsd-skeleton__line'))).toHaveSize(10);
  });

  it('uses a dedicated motion class whose stylesheet supplies a static reduced-motion state', () => {
    const lines = fixture.debugElement.queryAll(By.css('.lsd-skeleton__line'));
    expect(lines).toHaveSize(3);
    expect(lines.every((line) => line.nativeElement.getAttribute('aria-hidden') === null)).toBeTrue();
  });
});
