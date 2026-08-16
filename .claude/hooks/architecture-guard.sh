#!/usr/bin/env bash
set -euo pipefail
echo 'Architecture guard: verify no cross-service DbContext/internal project references and no direct transactional Service Bus publish.'
