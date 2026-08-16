# System Context

```mermaid
flowchart LR
 Client[Client User] --> Loop[Project Loop]
 Consultant[Consultant/PM] --> Loop
 Loop --> Email[Email Provider]
 Loop --> Finance[Accounting/PSA Source]
 Loop --> Azure[Azure SQL / Blob / Redis / Service Bus / Monitor]
```
