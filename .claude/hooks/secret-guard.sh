#!/usr/bin/env bash
set -euo pipefail
if grep -RInE '(AccountKey=|SharedAccessKey=|BEGIN (RSA |EC )?PRIVATE KEY|api[_-]?key\s*[:=]\s*[\"\x27][^\"\x27]+)' --exclude-dir=.git --exclude-dir=node_modules .; then echo 'Potential secret found'; exit 1; fi
