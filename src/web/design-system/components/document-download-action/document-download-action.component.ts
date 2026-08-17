import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

import { ButtonComponent } from '../../primitives/button/button.component';
import { ProgressComponent } from '../../primitives/progress/progress.component';

export type DocumentDownloadState = 'ready' | 'preparing' | 'downloading' | 'failed' | 'unavailable';
export type DocumentDownloadIntent = Readonly<{ type: 'download' | 'retry' }>;

@Component({
  selector: 'lsd-document-download-action',
  standalone: true,
  imports: [ButtonComponent, ProgressComponent],
  templateUrl: './document-download-action.component.html',
  styleUrl: './document-download-action.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentDownloadActionComponent {
  readonly id = input.required<string>();
  readonly documentLabel = input.required<string>();
  readonly state = input.required<DocumentDownloadState>();
  readonly progressValue = input<number | undefined>(undefined);
  readonly progressText = input<string | undefined>(undefined);
  readonly failureMessage = input('Download failed.');
  readonly unavailableMessage = input('Download unavailable.');

  readonly actionIntent = output<DocumentDownloadIntent>();

  protected readonly announcementRole = computed(() => this.state() === 'failed' ? 'alert' : 'status');
  protected readonly announcementLive = computed(() => this.state() === 'failed' ? 'assertive' : 'polite');

  protected requestDownload(): void {
    if (this.state() === 'ready') this.actionIntent.emit({ type: 'download' });
  }

  protected retry(): void {
    if (this.state() === 'failed') this.actionIntent.emit({ type: 'retry' });
  }
}
