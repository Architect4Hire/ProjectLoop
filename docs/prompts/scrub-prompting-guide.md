# Project Loop SCRUB Prompting Guide

Each microprompt should change one seam only.

- **Scope:** one feature, boundary, document, contract or test seam.
- **Constraints:** cite `CLAUDE.md`, applicable rule/skill and requirement/ADR.
- **Restrictions:** forbid unrelated refactors and architecture invention.
- **Usage:** state where the change belongs and expected call direction.
- **Behavior:** require tests, observability and documentation updates appropriate to that seam.

Prompts implement architecture; they do not silently redefine it.
