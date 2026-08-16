import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

import { ButtonComponent } from '../../primitives/button/button.component';

@Component({
  selector: 'lsd-pagination',
  standalone: true,
  imports: [ButtonComponent],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PaginationComponent {
  readonly currentPage = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly accessibleName = input('Pagination');

  readonly pageChange = output<number>();

  protected readonly hasPrevious = computed(() => this.currentPage() > 1 && this.totalPages() > 0);
  protected readonly hasNext = computed(() => this.currentPage() < this.totalPages());

  protected requestPrevious(): void {
    if (this.hasPrevious()) this.pageChange.emit(this.currentPage() - 1);
  }

  protected requestNext(): void {
    if (this.hasNext()) this.pageChange.emit(this.currentPage() + 1);
  }
}
