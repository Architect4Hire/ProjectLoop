# High-Level Design

Angular 22 browser -> YARP -> domain APIs. Each service owns SQL. Documents owns Blob binary references. Redis accelerates reads. Service Bus propagates durable facts. OpenTelemetry spans both HTTP and messaging paths.
