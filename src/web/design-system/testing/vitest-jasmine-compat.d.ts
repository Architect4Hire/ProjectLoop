import 'vitest';

declare module 'vitest' {
  interface Assertion<T = any> {
    toBeTrue(): T;
    toBeFalse(): T;
    toHaveSize(expected: number): T;
  }
}

declare global {
  const jasmine: {
    stringContaining(expected: string): any;
  };

  function spyOn<T extends object, K extends keyof T>(target: T, method: K): {
    and: {
      callFake(implementation: (...args: any[]) => any): unknown;
    };
  };
}

export {};
