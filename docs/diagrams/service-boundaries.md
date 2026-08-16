# Service Boundaries

```mermaid
flowchart TB
 Identity[Identity/Tenant] --- Engagement[Engagement]
 Engagement --- Documents[Documents]
 Documents --- Approvals[Approvals]
 Approvals --- Notification[Notification]
 Engagement --- Commercial[Commercial Read]
 Audit[Audit] -. consumes facts .- Documents
 Audit -. consumes facts .- Approvals
```
