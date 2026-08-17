import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';

import { FilterActionBarComponent } from '../../patterns/filter-action-bar/filter-action-bar.component';
import { ButtonComponent } from '../../primitives/button/button.component';
import { SelectComponent, type SelectOption } from '../../primitives/select/select.component';

export type DocumentFilterOption = SelectOption<string>;

export interface DocumentFilterValues {
  readonly project: string | null;
  readonly category: string | null;
  readonly status: string | null;
  readonly visibility: string | null;
}

export interface DocumentFilterChangeIntent {
  readonly source: 'submit' | 'reset';
  readonly filters: DocumentFilterValues;
}

@Component({
  selector: 'lsd-document-filters',
  standalone: true,
  imports: [ButtonComponent, FilterActionBarComponent, SelectComponent],
  templateUrl: './document-filters.component.html',
  styleUrl: './document-filters.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentFiltersComponent {
  readonly id = input.required<string>();
  readonly accessibleName = input('Document filters');
  readonly projectOptions = input.required<readonly DocumentFilterOption[]>();
  readonly categoryOptions = input.required<readonly DocumentFilterOption[]>();
  readonly statusOptions = input.required<readonly DocumentFilterOption[]>();
  readonly visibilityOptions = input.required<readonly DocumentFilterOption[]>();

  readonly filterChange = output<DocumentFilterChangeIntent>();

  protected readonly project = signal<string | null>(null);
  protected readonly category = signal<string | null>(null);
  protected readonly status = signal<string | null>(null);
  protected readonly visibility = signal<string | null>(null);
  protected readonly activeFilterCount = computed(() =>
    [this.project(), this.category(), this.status(), this.visibility()].filter((value) => value !== null).length,
  );

  protected submit(event: SubmitEvent): void {
    event.preventDefault();
    this.emitIntent('submit');
  }

  protected reset(): void {
    this.project.set(null);
    this.category.set(null);
    this.status.set(null);
    this.visibility.set(null);
    this.emitIntent('reset');
  }

  private emitIntent(source: DocumentFilterChangeIntent['source']): void {
    this.filterChange.emit({
      source,
      filters: {
        project: this.project(),
        category: this.category(),
        status: this.status(),
        visibility: this.visibility(),
      },
    });
  }
}
