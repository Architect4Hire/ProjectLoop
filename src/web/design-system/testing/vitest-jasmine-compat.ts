import { expect, vi } from 'vitest';

expect.extend({
  toBeTrue(received: unknown) {
    return { pass: received === true, message: () => `expected ${String(received)} to be true` };
  },
  toBeFalse(received: unknown) {
    return { pass: received === false, message: () => `expected ${String(received)} to be false` };
  },
  toHaveSize(received: { length?: number; size?: number }, expected: number) {
    const actual = received.length ?? received.size;
    return { pass: actual === expected, message: () => `expected size ${expected}, received ${String(actual)}` };
  },
});

Object.assign(globalThis, {
  jasmine: { stringContaining: expect.stringContaining },
  spyOn<T extends object, K extends keyof T>(target: T, method: K) {
    const spy = vi.spyOn(target, method as never);
    return Object.assign(spy, {
      and: {
        callFake(implementation: (...args: unknown[]) => unknown) {
          spy.mockImplementation(implementation as never);
          return spy;
        },
      },
    });
  },
});
