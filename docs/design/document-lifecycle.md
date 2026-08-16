# Document Lifecycle

```mermaid
stateDiagram-v2
  [*] --> Draft
  Draft --> Quarantined: upload
  Quarantined --> Available: scan passed
  Quarantined --> Rejected: scan failed
  Available --> Published: publish
  Published --> Superseded: newer published version
  Available --> Retained: retention action
  Superseded --> Retained: retention action
```
