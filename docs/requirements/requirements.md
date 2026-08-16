# Project Loop Requirements

## Product intent
Provide a secure, multi-tenant client portal for consulting engagements that reduces reliance on separate client-portal/document-approval SaaS products.

## Functional requirements
- Clients can authenticate and access only authorized organizations/projects.
- Dashboard shows project health, milestones, meetings, decisions, deliverables, invoices, hours consumed and retainer remaining.
- Users can browse documents by project, category, status and visibility.
- Documents support immutable version history.
- Internal users can publish client-visible document versions.
- Authorized clients can approve architecture, milestones, change requests and deliverables.
- Approval decisions bind to an exact target version and are auditable.
- Approval requests generate asynchronous notifications.
- Approval outcomes may update project/milestone state asynchronously.
- Audit history captures security-sensitive and business-significant actions.

## Non-functional themes
- tenant isolation;
- least-privilege authorization;
- durable event processing;
- traceability;
- document confidentiality;
- idempotent asynchronous consumers;
- accessible Angular UX;
- resilient cache-independent correctness.
