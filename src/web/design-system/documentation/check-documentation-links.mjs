import { existsSync, readFileSync, readdirSync } from 'node:fs';
import { dirname, extname, join, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const directory = dirname(fileURLToPath(import.meta.url));
const documents = readdirSync(directory).filter((name) => extname(name) === '.md');
const failures = [];
const linkPattern = /(?<!!)\[[^\]]*\]\(([^)\s]+)(?:\s+['"][^'"]*['"])?\)/g;

for (const document of documents) {
  const source = readFileSync(join(directory, document), 'utf8');
  for (const match of source.matchAll(linkPattern)) {
    const target = match[1];
    if (/^(?:https?:|mailto:|#)/i.test(target)) continue;
    const path = decodeURIComponent(target.split('#', 1)[0]);
    if (!path || existsSync(resolve(directory, path))) continue;
    failures.push(`${document}: missing ${target}`);
  }
}

if (failures.length) {
  console.error(`Broken local documentation links (${failures.length}):\n- ${failures.join('\n- ')}`);
  process.exitCode = 1;
} else {
  console.log(`Documentation link check passed for ${documents.length} Markdown files.`);
}

