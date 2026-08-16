# Integration Event Catalog

| Event | Producer | Example Consumers |
|---|---|---|
| DocumentUploaded | Documents | Audit |
| DocumentVersionCreated | Documents | Audit |
| DocumentPublished | Documents | Approvals, Audit |
| DocumentSuperseded | Documents | Approvals, Audit |
| ApprovalRequested | Approvals | Notification, Audit |
| ApprovalGranted | Approvals | Engagement, Notification, Audit |
| ApprovalRejected | Approvals | Engagement, Notification, Audit |
| MilestoneCompleted | Engagement | Notification, Audit |
| InvoicePublished | Commercial Read | Notification |
| RetainerThresholdReached | Commercial Read | Notification |
| ClientUserInvited | Identity/Tenant | Notification, Audit |
