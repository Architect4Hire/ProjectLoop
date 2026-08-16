import fs from 'node:fs';
import path from 'node:path';
import process from 'node:process';
import { fileURLToPath } from 'node:url';

const repositoryRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../../../..');
const extensions = new Set(['.html', '.ts']);
const defaultRoots = ['src/web/features', 'src/web/app'];
const minimumBundleLength = 8;
const designSystemRoot = path.resolve('src/web/design-system');
const commonUtilities = new Set([
  'absolute', 'block', 'border', 'contents', 'fixed', 'flex', 'grid', 'hidden',
  'inline', 'relative', 'sticky', 'table',
]);

function walk(root) {
  if (!fs.existsSync(root)) return [];
  const relativeToDesignSystem = path.relative(designSystemRoot, path.resolve(root));
  if (relativeToDesignSystem === '' || (!relativeToDesignSystem.startsWith(`..${path.sep}`) && relativeToDesignSystem !== '..')) return [];
  const entries = fs.readdirSync(root, { withFileTypes: true });
  return entries.flatMap((entry) => {
    const target = path.join(root, entry.name);
    return entry.isDirectory() ? walk(target) : extensions.has(path.extname(target)) ? [target] : [];
  });
}

function lineAt(source, offset) {
  return source.slice(0, offset).split(/\r?\n/).length;
}

function isUtility(token) {
  return commonUtilities.has(token) || /[-:[\]/]/.test(token);
}

function normalizedBundles(source) {
  const bundles = [];
  const quoted = /(["'`])([^\r\n]*?)\1/g;
  for (const match of source.matchAll(quoted)) {
    const tokens = match[2].trim().split(/\s+/).filter(Boolean);
    if (tokens.length < minimumBundleLength || !tokens.every(isUtility)) continue;
    bundles.push({ bundle: tokens.join(' '), offset: match.index ?? 0 });
  }
  return bundles;
}

function isPrivateDesignSystemImport(specifier) {
  const normalized = specifier.replaceAll('\\', '/').replace(/\.(?:ts|js)$/, '');
  if (normalized === '@lsd/design-system' || normalized.endsWith('/design-system/public-api')) return false;
  return normalized.includes('/design-system/') || normalized.startsWith('design-system/');
}

export function checkFeatureBoundaries(roots) {
  const files = roots.flatMap((root) => walk(path.resolve(root)));
  const violations = [];
  const occurrences = new Map();

  for (const file of files) {
    const source = fs.readFileSync(file, 'utf8');
    const imports = /(?:from\s*|import\s*\(\s*|require\(\s*)["']([^"']+)["']/g;
    for (const match of source.matchAll(imports)) {
      if (isPrivateDesignSystemImport(match[1])) {
        violations.push({
          file,
          line: lineAt(source, match.index ?? 0),
          rule: 'design-system-public-import',
          message: `Import from the public entry point instead of "${match[1]}".`,
        });
      }
    }

    for (const item of normalizedBundles(source)) {
      const locations = occurrences.get(item.bundle) ?? [];
      locations.push({ file, line: lineAt(source, item.offset) });
      occurrences.set(item.bundle, locations);
    }
  }

  for (const [bundle, locations] of occurrences) {
    if (locations.length < 2) continue;
    for (const location of locations) {
      violations.push({
        ...location,
        rule: 'repeated-tailwind-bundle',
        message: `Repeated ${bundle.split(' ').length}-utility bundle; promote the shared pattern to the design system.`,
      });
    }
  }

  return violations;
}

function runSelfTest() {
  const fixtureRoot = path.join(repositoryRoot, 'test-fixtures/feature-boundaries');
  const passingViolations = checkFeatureBoundaries([path.join(fixtureRoot, 'passing')]);
  const failingViolations = checkFeatureBoundaries([path.join(fixtureRoot, 'failing')]);
  const importViolations = failingViolations.filter((item) => item.rule === 'design-system-public-import');

  if (passingViolations.length !== 0) {
    throw new Error(`Expected the public-entry-point fixture to pass; received ${passingViolations.length} violation(s).`);
  }
  if (failingViolations.length !== 1 || importViolations.length !== 1) {
    throw new Error(`Expected the deep-import fixture to produce one public-import violation; received ${failingViolations.length}.`);
  }
  console.log('Self-test passed: the public import was accepted and the deep import was rejected.');
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const args = process.argv.slice(2);
  if (args.includes('--self-test')) {
    runSelfTest();
  } else {
    const roots = args.length ? args : defaultRoots;
    const violations = checkFeatureBoundaries(roots);
    for (const violation of violations) {
      console.error(`${path.relative(process.cwd(), violation.file)}:${violation.line} [${violation.rule}] ${violation.message}`);
    }
    if (violations.length) process.exitCode = 1;
    else console.log(`Feature boundary check passed (${roots.join(', ')}).`);
  }
}
