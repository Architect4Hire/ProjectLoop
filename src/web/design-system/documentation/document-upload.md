# Document upload

## Purpose

`DocumentUploadComponent` composes File Picker, Input, Select, Progress, Field Message, and Button APIs into a document upload form. It collects one policy-accepted file and display-ready metadata, then emits caller-handled intent. It never accesses Blob Storage, transfers bytes, generates URLs, or persists document data.

## API and states

- Required `id`, `categoryOptions`, and `visibilityOptions` establish stable controls and caller-supplied metadata choices.
- Required `accept`, `maxFileSizeBytes`, and `filePolicyDescription` communicate and enforce caller MIME/size policy through File Picker. Optional `validateFile` adds caller-owned synchronous policy.
- Caller-owned `state` is `idle | uploading | failed | completed`. Progress and message inputs present transport state without initiating work.
- `uploadIntent` is the only output. Submit emits the accepted `File` and typed title/category/visibility metadata; uploading exposes `cancel`; failure exposes `retry`.
- Required-field checks affect presentation only. The caller and server own authorization, validation, persistence, transport, retry, and cancellation.

```html
<lsd-document-upload
  id="upload"
  accept="application/pdf,.docx"
  [maxFileSizeBytes]="maximumBytes"
  filePolicyDescription="PDF or DOCX up to 25 MB"
  [categoryOptions]="categories"
  [visibilityOptions]="authorizedVisibilityOptions"
  [state]="uploadState()"
  [progressValue]="uploadProgress()"
  (uploadIntent)="handleUploadIntent($event)" />
```

## Accessibility

File Picker retains native keyboard browse, drag/drop enhancement, and assertive policy validation. Every metadata control has a persistent visible label and required/error association. Progress uses native `progress`, visible value text, and polite status semantics. Failures are assertive alerts; completion is polite status text. Cancel and retry are native buttons with visible labels.

## Responsive behavior

Title spans the metadata grid on wider screens with category and visibility below it. Below 37.5rem, fields and actions stack in unchanged DOM and keyboard order. Policy, validation, progress, failure, and selected-file confirmation remain adjacent to File Picker.

## Do / don't

Do supply MIME, byte-size, custom validation, authorized options, localized copy, and caller-owned upload state. Do validate untrusted files again on the server. Don't access storage, upload automatically, generate URLs, infer authorization, or treat client validation as a security boundary.

## Visual coverage

Unit coverage verifies validation display, determinate progress semantics, cancellation intent, and announced failure recovery. Narrow visual coverage should include idle, uploading, failed, completed, validation-error, and long-file-name presentations.

## Public import

```ts
import { DocumentUploadComponent, type DocumentUploadIntent } from 'src/web/design-system/public-api';
```
