import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { VersionChipComponent, type VersionQualifier } from './version-chip.component';

@Component({
  standalone: true,
  imports: [VersionChipComponent],
  template: `
    <lsd-version-chip
      [versionLabel]="versionLabel"
      [qualifier]="qualifier"
      [qualifierLabel]="qualifierLabel" />
  `,
})
class VersionChipTestHostComponent {
  versionLabel = 'v2.7.0-rc.1';
  qualifier: VersionQualifier | undefined;
  qualifierLabel: string | undefined;
}

describe('VersionChipComponent', () => {
  let fixture: ComponentFixture<VersionChipTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [VersionChipTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(VersionChipTestHostComponent);
    fixture.detectChanges();
  });

  it('always renders the exact caller-supplied version label as visible text', () => {
    const version = fixture.debugElement.query(By.css('.lsd-version-chip__version')).nativeElement as HTMLElement;
    expect(version.textContent).toBe('v2.7.0-rc.1');
  });

  it('distinguishes every qualifier with visible text rather than color alone', () => {
    const expected: Record<VersionQualifier, string> = {
      current: 'Current',
      approved: 'Approved',
      published: 'Published',
    };

    for (const qualifier of Object.keys(expected) as VersionQualifier[]) {
      fixture.componentInstance.qualifier = qualifier;
      fixture.detectChanges();
      const qualifierText = fixture.debugElement.query(By.css('.lsd-version-chip__qualifier')).nativeElement as HTMLElement;
      expect(qualifierText.textContent).toBe(expected[qualifier]);
      expect(fixture.nativeElement.textContent).toContain('v2.7.0-rc.1');
    }
  });

  it('uses an overrideable qualifier label without changing the exact version text', () => {
    fixture.componentInstance.qualifier = 'approved';
    fixture.componentInstance.qualifierLabel = 'Aprobada';
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('.lsd-version-chip__qualifier')).nativeElement.textContent).toBe('Aprobada');
    expect(fixture.debugElement.query(By.css('.lsd-version-chip__version')).nativeElement.textContent).toBe('v2.7.0-rc.1');
  });
});
