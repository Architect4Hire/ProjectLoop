import { spawn } from 'node:child_process';
import { createReadStream } from 'node:fs';
import { stat } from 'node:fs/promises';
import { createServer } from 'node:http';
import { extname, join, normalize } from 'node:path';

const build = spawn(process.execPath, ['node_modules/@angular/cli/bin/ng.js', 'build', 'design-system-visual'], {
  stdio: 'inherit',
});

const exitCode = await new Promise((resolve) => build.once('exit', resolve));
if (exitCode !== 0) process.exit(exitCode ?? 1);

const root = join(process.cwd(), 'dist/design-system-visual/browser');
const mimeTypes = new Map([
  ['.css', 'text/css; charset=utf-8'],
  ['.html', 'text/html; charset=utf-8'],
  ['.js', 'text/javascript; charset=utf-8'],
  ['.json', 'application/json; charset=utf-8'],
  ['.svg', 'image/svg+xml'],
]);

createServer(async (request, response) => {
  const pathname = new URL(request.url ?? '/', 'http://127.0.0.1').pathname;
  const relativePath = normalize(pathname).replace(/^(\.\.(\/|\\|$))+/, '').replace(/^[/\\]+/, '');
  let file = join(root, relativePath || 'index.html');
  try {
    if ((await stat(file)).isDirectory()) file = join(file, 'index.html');
  } catch {
    file = join(root, 'index.html');
  }
  response.setHeader('Content-Type', mimeTypes.get(extname(file)) ?? 'application/octet-stream');
  createReadStream(file).pipe(response);
}).listen(4207, '127.0.0.1');
