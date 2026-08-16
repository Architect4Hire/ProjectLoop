import { ChangeDetectionStrategy, Component } from '@angular/core';

import { LinkDirective } from '../../primitives/link/link.directive';

@Component({
  selector: 'lsd-skip-link',
  standalone: true,
  imports: [LinkDirective],
  templateUrl: './skip-link.component.html',
  styleUrl: './skip-link.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkipLinkComponent {}
