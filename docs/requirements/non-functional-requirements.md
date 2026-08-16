# Non-Functional Requirements

- **Security:** default deny; no public blob containers; server-side resource authorization.
- **Isolation:** every tenant-owned read/write is scoped and cross-tenant access is tested.
- **Reliability:** source-state change plus event publication uses an outbox; consumers tolerate duplicates.
- **Availability:** notification or downstream projection failure cannot undo completed approvals.
- **Performance:** avoid deep synchronous fan-out; introduce projections only from measured need.
- **Observability:** end-to-end tracing across HTTP, SQL, Redis, Blob, outbox, Service Bus and consumers.
- **Maintainability:** preserve bounded ownership, onion layering, design-system reuse and ADR governance.
- **Accessibility:** client portal meets semantic HTML and keyboard/screen-reader expectations.
