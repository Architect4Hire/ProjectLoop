# Event-Driven Architecture

Events are past-tense completed facts. Producers own schemas and use outbox when state+event must be atomic. Consumers are idempotent and use an inbox when durable side effects need duplicate protection. Dead-letter handling is operational work, not a queue to ignore.
