# Approval Workflow Design

ApprovalRequest targets an exact resource/version. Allowed states: Requested, Approved, Rejected, Cancelled, Expired where policy requires. Decisions are immutable records. ApprovalRequested/Granted/Rejected are published through outbox. Email and milestone updates are consumers, so their failure cannot roll back the approval.
