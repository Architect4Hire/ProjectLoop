# Requirements Matrix

| ID | Requirement | Primary Domain | Verification |
|---|---|---|---|
| LOOP-001 | Tenant-isolated portal access | Identity/Tenant | auth + cross-tenant tests |
| LOOP-002 | Project dashboard | Engagement | API/UI tests |
| LOOP-003 | Document catalog | Documents | API/UI tests |
| LOOP-004 | Blob binary + SQL metadata | Documents | integration tests |
| LOOP-005 | Immutable document versions | Documents | domain/integration tests |
| LOOP-006 | Client publication visibility | Documents | authorization tests |
| LOOP-007 | Version-bound approval request | Approvals | domain tests |
| LOOP-008 | Immutable approval decision history | Approvals/Audit | integration tests |
| LOOP-009 | Async approval notifications | Notification | messaging tests |
| LOOP-010 | Async milestone propagation | Engagement | event/idempotency tests |
| LOOP-011 | Commercial summary display | Commercial Read Model | contract/UI tests |
| LOOP-012 | Distributed trace correlation | Platform | observability verification |
