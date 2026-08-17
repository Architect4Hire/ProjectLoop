import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';
import { FilePickerComponent, type FilePickerSelection, type FileValidator } from '../../components/file-picker/file-picker.component';
import { ButtonComponent } from '../../primitives/button/button.component';
import { FieldMessageComponent } from '../../primitives/field-message/field-message.component';
import { InputComponent } from '../../primitives/input/input.component';
import { ProgressComponent } from '../../primitives/progress/progress.component';
import { SelectComponent, type SelectOption } from '../../primitives/select/select.component';

export type DocumentUploadState = 'idle' | 'uploading' | 'failed' | 'completed';
export type DocumentUploadOption = SelectOption<string>;
export interface DocumentUploadMetadata { readonly title: string; readonly category: string; readonly visibility: string; }
export type DocumentUploadIntent =
  | Readonly<{ type: 'submit'; file: File; metadata: DocumentUploadMetadata }>
  | Readonly<{ type: 'cancel' }>
  | Readonly<{ type: 'retry' }>;

@Component({
  selector: 'lsd-document-upload', standalone: true,
  imports: [ButtonComponent, FieldMessageComponent, FilePickerComponent, InputComponent, ProgressComponent, SelectComponent],
  templateUrl: './document-upload.component.html', styleUrl: './document-upload.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentUploadComponent {
  readonly id = input.required<string>();
  readonly categoryOptions = input.required<readonly DocumentUploadOption[]>();
  readonly visibilityOptions = input.required<readonly DocumentUploadOption[]>();
  readonly accept = input.required<string>();
  readonly maxFileSizeBytes = input.required<number>();
  readonly validateFile = input<FileValidator>(() => undefined);
  readonly filePolicyDescription = input.required<string>();
  readonly state = input<DocumentUploadState>('idle');
  readonly progressValue = input<number | undefined>(undefined);
  readonly progressText = input<string | undefined>(undefined);
  readonly failureMessage = input<string | undefined>(undefined);
  readonly completedMessage = input('Upload completed');
  readonly uploadIntent = output<DocumentUploadIntent>();

  protected readonly title = signal('');
  protected readonly category = signal<string | null>(null);
  protected readonly visibility = signal<string | null>(null);
  protected readonly selectedFile = signal<File | null>(null);
  protected readonly submitted = signal(false);
  protected readonly controlsDisabled = computed(() => this.state() === 'uploading');
  protected readonly titleError = computed(() => this.submitted() && !this.title().trim() ? 'Enter a document title.' : undefined);
  protected readonly categoryError = computed(() => this.submitted() && this.category() === null ? 'Select a document category.' : undefined);
  protected readonly visibilityError = computed(() => this.submitted() && this.visibility() === null ? 'Select document visibility.' : undefined);
  protected readonly fileError = computed(() => this.submitted() && this.selectedFile() === null ? 'Select a file that meets the upload policy.' : undefined);

  protected selectFiles(selection: FilePickerSelection): void { this.selectedFile.set(selection.accepted[0] ?? null); }
  protected submit(event: SubmitEvent): void {
    event.preventDefault();
    if (this.controlsDisabled()) return;
    this.submitted.set(true);
    const file = this.selectedFile(); const title = this.title().trim();
    const category = this.category(); const visibility = this.visibility();
    if (!file || !title || category === null || visibility === null) return;
    this.uploadIntent.emit({ type: 'submit', file, metadata: { title, category, visibility } });
  }
  protected cancel(): void { if (this.state() === 'uploading') this.uploadIntent.emit({ type: 'cancel' }); }
  protected retry(): void { if (this.state() === 'failed') this.uploadIntent.emit({ type: 'retry' }); }
}
