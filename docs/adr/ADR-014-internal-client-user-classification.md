# Internal/Client User Classification

- **Status:** Accepted
- **Date:** 2026-08-18

## Context
LOOP-006 requires that only internal users can publish a document version to
client visibility, and that only authorized client users can see published,
client-visible versions. No service currently distinguishes an internal
(consulting-firm) user from a client-organization user: `TenantMembership`
models per-tenant `Member`/`Admin` roles for a tenant's own members, not a
cross-tenant internal-staff concept, and no service issues or trusts a
user-classification claim today.

A full internal-staff identity model (how consulting-firm staff are
provisioned and authenticated across many client tenants) is out of scope for
the Documents versioning/publication/download seam and belongs to the
Identity/Tenant domain.

## Decision
Each service that needs this distinction trusts a `user_type` claim on the
authenticated principal, exactly as it already trusts the `tenant_id` claim:
validated by the token issuer, never re-derived from another service's
database. A value of `Internal` marks an internal user; anything else
(including the claim's absence) is treated as a client user. This fail-safe
default follows default-deny: a caller is only granted internal, publish-
capable privilege when explicitly asserted.

Documents exposes this as `ITenantContext.IsClientUser`, populated by
`TenantContextMiddleware` alongside `TenantId`/`UserId`.

## Consequences
- Document publication (`DocumentPublishFacade`) is restricted to callers
  where `IsClientUser == false`.
- Document/version download visibility for client users additionally
  requires `Document.Visibility == Client` and the target version's
  `IsPublished == true`.
- How the `user_type` claim is issued (Identity/Tenant token issuance,
  membership-to-claim mapping) is not yet implemented and must be resolved by
  a future Identity/Tenant prompt or a superseding ADR before this is
  production-complete.
