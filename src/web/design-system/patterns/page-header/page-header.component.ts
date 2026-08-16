import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { BreadcrumbComponent, type BreadcrumbItem } from '../../components/breadcrumb/breadcrumb.component';
import { SeparatorComponent } from '../../primitives/separator/separator.component';

export type PageHeaderHeadingLevel = 1 | 2 | 3 | 4 | 5 | 6;

export interface PageHeaderMetadata {
  readonly label: string;
  readonly value: string;
}

@Component({
  selector: 'lsd-page-header',
  standalone: true,
  imports: [BreadcrumbComponent, SeparatorComponent],
  templateUrl: './page-header.component.html',
  styleUrl: './page-header.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PageHeaderComponent {
  readonly title = input.required<string>();
  readonly headingLevel = input<PageHeaderHeadingLevel>(1);
  readonly description = input<string | undefined>(undefined);
  readonly breadcrumbs = input<readonly BreadcrumbItem[]>([]);
  readonly breadcrumbLabel = input('Breadcrumb');
  readonly metadata = input<readonly PageHeaderMetadata[]>([]);
}
