# Project Loop Documentation

The documentation hierarchy is intentional:

1. `requirements/` defines what the product must do.
2. `architecture/` defines system boundaries and integration principles.
3. `design/` explains subsystem designs.
4. `adr/` records durable architecture decisions.
5. `diagrams/` provides focused Mermaid views.
6. `prompts/` contains SCRUB implementation guidance; prompts must not redefine architecture.

When code and docs disagree, determine whether implementation drifted or an ADR intentionally changed the design. Do not silently choose one.
