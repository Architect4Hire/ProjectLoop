# Integration Architecture

Use HTTP for immediate answers and short commands. Use Service Bus for facts, fan-out, retryable work, temporal decoupling and workflow progression. A local business transaction cannot be made atomic with an HTTP call to another service.
