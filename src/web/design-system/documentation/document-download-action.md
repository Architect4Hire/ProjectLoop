# Document download action

## Purpose

`DocumentDownloadActionComponent` presents authorized document retrieval as ready, preparing, downloading, failed, or unavailable. It emits intent only: the caller authorizes and performs retrieval. Its public API accepts no URL, Blob URL, response, or transport client, and the component makes no HTTP request.

## API and states

- Required `id`, display-safe `documentLabel`, and caller-owned `state` identify the presentation.
- `progressValue` and `progressText` provide determinate or indeterminate Progress presentation while downloading.
- `failureMessage` and `unavailableMessage` provide display-ready state copy.
- `actionIntent` emits `{ type: 'download' }` only from ready and `{ type: 'retry' }` only from failed. Callers reauthorize every intent and own retrieval, cancellation, browser save behavior, and temporary object-URL lifecycle outside this API.

```html
<lsd-document-download-action
  id="download-v4"
  documentLabel="Architecture plan v4"
  [state]="downloadState()"
  [progressValue]="downloadProgress()"
  (actionIntent)="handleAuthorizedDownloadIntent($event)" />
```

## Accessibility

Ready and retry actions use native Button semantics and contextual accessible names. Preparing, downloading, and unavailable changes are polite status announcements; failure is an assertive alert. Downloading composes native Progress with visible label/value text. Disabled states remain visible and noninteractive rather than disappearing.

## Responsive behavior

The presenter remains compact on wider layouts. Below 30rem, its button and progress presentation fill the available width while state text wraps without truncating the document label.

## Do / don't

Do reauthorize each download/retry intent, retrieve through application infrastructure, and revoke any temporary object URL after use. Don't pass permanent Blob URLs, signed URLs, credentials, or HTTP clients into this component; don't infer availability or perform retrieval here.

## Visual coverage

Unit coverage verifies state announcements, downloading progress semantics, ready intent, retry intent, and the absence of anchor/URL presentation. Narrow visual coverage should include every state and long document labels.

## Public import

```ts
import { DocumentDownloadActionComponent, type DocumentDownloadIntent } from 'src/web/design-system/public-api';
```
