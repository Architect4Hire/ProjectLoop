# Deployment Design

Local composition uses .NET Aspire. Azure deployment should preserve logical service boundaries, private SQL/Blob access where practical, managed identity, Key Vault, Azure Service Bus, Redis, App Insights/Azure Monitor and a single YARP-facing browser/API edge.
