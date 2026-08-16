import {
  ButtonComponent,
  IconComponent,
  spacingTokens,
  type IconName,
} from '@lsd/design-system';

/** Compile-only proof that application code can resolve public values and types. */
export const publicAliasValues = [ButtonComponent, IconComponent, spacingTokens] as const;
export type PublicAliasIconName = IconName;
