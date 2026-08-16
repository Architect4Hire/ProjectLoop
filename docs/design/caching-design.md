# Caching Design

Redis may cache expensive reads and projection data. Keys are domain/tenant/version scoped, TTL is explicit, invalidation follows writes, and correctness survives eviction. Do not cache raw sensitive document content casually.
