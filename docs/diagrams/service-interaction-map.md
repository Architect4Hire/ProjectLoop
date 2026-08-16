# Service Interaction Map

Solid arrows are immediate HTTP; dashed arrows are events.

```mermaid
flowchart LR
 Gateway --> Documents
 Gateway --> Approvals
 Gateway --> Engagement
 Approvals -->|HTTP query| Documents
 Documents -. DocumentPublished .-> Approvals
 Approvals -. ApprovalGranted .-> Engagement
 Approvals -. ApprovalRequested .-> Notification
 Documents -. facts .-> Audit
 Approvals -. facts .-> Audit
```
