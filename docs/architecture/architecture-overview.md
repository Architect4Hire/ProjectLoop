# Architecture Overview

Project Loop is a distributed .NET 10 + Angular 22 system behind YARP. Bounded services own SQL databases; Documents additionally owns Blob Storage references. HTTP serves immediate interactions. Azure Service Bus carries durable business facts and workflow progression. Aspire composes local dependencies; OpenTelemetry provides end-to-end traces.
