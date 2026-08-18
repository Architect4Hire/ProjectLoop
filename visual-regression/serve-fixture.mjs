import { spawn } from 'node:child_process';
import { createReadStream } from 'node:fs';
import { stat } from 'node:fs/promises';
import { createServer } from 'node:http';
import { extname, join, normalize } from 'node:path';

const host = '127.0.0.1';
const port = 4207;
const parentPid = process.ppid;
let shuttingDown = false;
let server;

const build = spawn(process.execPath, ['node_modules/@angular/cli/bin/ng.js', 'build', 'design-system-visual'], {
  stdio: 'inherit',
  windowsHide: true,
});

async function stopBuild() {
  if (build.exitCode !== null || build.pid === undefined) return;
  if (process.platform === 'win32') {
    const taskkill = spawn('taskkill.exe', ['/pid', String(build.pid), '/T', '/F'], {
      stdio: 'ignore',
      windowsHide: true,
    });
    await new Promise((resolve) => taskkill.once('exit', resolve));
    return;
  }
  build.kill('SIGTERM');
}

async function shutdown(exitCode = 0) {
  if (shuttingDown) return;
  shuttingDown = true;
  if (server?.listening) {
    server.closeAllConnections();
    await new Promise((resolve) => server.close(resolve));
  }
  await stopBuild();
  process.exit(exitCode);
}

for (const signal of ['SIGINT', 'SIGTERM']) {
  process.once(signal, () => void shutdown(0));
}

// Playwright launches webServer commands through cmd.exe on Windows. If that
// shell is force-terminated, Node receives no POSIX signal, so observe the
// original parent and close the server instead of becoming an orphan.
setInterval(() => {
  try {
    process.kill(parentPid, 0);
  } catch {
    void shutdown(0);
  }
}, 250).unref();

const exitCode = await new Promise((resolve, reject) => {
  build.once('error', reject);
  build.once('exit', resolve);
}).catch((error) => {
  console.error(`Visual fixture build failed to start: ${error.message}`);
  return 1;
});
if (exitCode !== 0) process.exit(exitCode ?? 1);

const root = join(process.cwd(), 'dist/design-system-visual/browser');
const mimeTypes = new Map([
  ['.css', 'text/css; charset=utf-8'],
  ['.html', 'text/html; charset=utf-8'],
  ['.js', 'text/javascript; charset=utf-8'],
  ['.json', 'application/json; charset=utf-8'],
  ['.svg', 'image/svg+xml'],
]);

server = createServer(async (request, response) => {
  const pathname = new URL(request.url ?? '/', 'http://127.0.0.1').pathname;
  if (request.method === 'POST' && pathname === '/__shutdown') {
    response.writeHead(204).end();
    setImmediate(() => void shutdown(0));
    return;
  }
  const relativePath = normalize(pathname).replace(/^(\.\.(\/|\\|$))+/, '').replace(/^[/\\]+/, '');
  let file = join(root, relativePath || 'index.html');
  try {
    if ((await stat(file)).isDirectory()) file = join(file, 'index.html');
  } catch {
    file = join(root, 'index.html');
  }
  response.setHeader('Content-Type', mimeTypes.get(extname(file)) ?? 'application/octet-stream');
  createReadStream(file).pipe(response);
});

server.once('error', (error) => {
  console.error(`Visual fixture server failed on http://${host}:${port}: ${error.message}`);
  void shutdown(1);
});

server.listen(port, host, () => {
  console.log(`Visual fixture server listening at http://${host}:${port}`);
});
