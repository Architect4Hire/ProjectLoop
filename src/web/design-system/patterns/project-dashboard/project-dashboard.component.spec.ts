import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { ProjectDashboardComponent } from './project-dashboard.component';

@Component({
  standalone: true,
  imports: [ProjectDashboardComponent],
  template: `
    <lsd-project-dashboard id="overview">
      <div lsdProjectDashboardHealth>Health content</div>
      <div lsdProjectDashboardMetrics>Metrics content</div>
      <div lsdProjectDashboardMilestones>Milestones content</div>
      <div lsdProjectDashboardMeetings>Meetings content</div>
      <div lsdProjectDashboardDecisions>Decisions content</div>
      <div lsdProjectDashboardDeliverables>Deliverables content</div>
    </lsd-project-dashboard>
  `,
})
class ProjectDashboardTestHostComponent {}

describe('ProjectDashboardComponent', () => {
  let fixture: ComponentFixture<ProjectDashboardTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ProjectDashboardTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(ProjectDashboardTestHostComponent);
    fixture.detectChanges();
  });

  it('provides stable headings and names every independent region', () => {
    const sections = fixture.debugElement.queryAll(By.css('section'));
    const expected = [
      ['overview-health-heading', 'Project health'],
      ['overview-metrics-heading', 'Summary metrics'],
      ['overview-milestones-heading', 'Milestones'],
      ['overview-meetings-heading', 'Upcoming meetings'],
      ['overview-decisions-heading', 'Recent decisions'],
      ['overview-deliverables-heading', 'Deliverables'],
    ];

    expect(sections).toHaveSize(6);
    sections.forEach((section, index) => {
      const [id, text] = expected[index];
      const heading = section.query(By.css('h2')).nativeElement as HTMLElement;
      expect(heading.id).toBe(id);
      expect(heading.textContent).toContain(text);
      expect((section.nativeElement as HTMLElement).getAttribute('aria-labelledby')).toBe(id);
    });
  });

  it('preserves desktop and mobile reading order in the DOM', () => {
    const sections = fixture.debugElement.queryAll(By.css('section')).map(
      section => (section.nativeElement as HTMLElement).className.match(/region--([a-z]+)/)?.[1],
    );
    expect(sections).toEqual(['health', 'metrics', 'milestones', 'meetings', 'decisions', 'deliverables']);
  });

  it('projects each region independently', () => {
    const dashboard = fixture.nativeElement as HTMLElement;
    expect(dashboard.querySelector('.lsd-project-dashboard__region--health')?.textContent).toContain('Health content');
    expect(dashboard.querySelector('.lsd-project-dashboard__region--deliverables')?.textContent).toContain('Deliverables content');
  });
});
