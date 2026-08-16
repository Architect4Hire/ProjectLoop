# Long-Lived Workflows

Approval/reminder/escalation workflows are durable process state when they exceed a single request. Start/validate over HTTP, persist workflow intent + outbox, progress with Service Bus, expose status over HTTP. Human waiting periods are state, not sleeping workers.
