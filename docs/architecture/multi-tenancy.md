# Multi-Tenancy Architecture

Tenant membership is authorization data, not a UI filter. Every tenant-owned record, cache key, blob reference, event and audit fact carries tenant context. Caller-supplied TenantId never proves access. Cross-tenant admin operations require explicit policies and audit.
