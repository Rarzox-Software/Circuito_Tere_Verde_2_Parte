/// <reference types="vite/client" />

declare module "*.png" {
  const value: string;
  export default value;
}

declare module "*.jpg" {
  const value: string;
  export default value;
}

declare module "*.jpeg" {
  const value: string;
  export default value;
}

declare module "*.svg" {
  const value: string;
  export default value;
}

declare module "*.svg?react" {
  import type { FC, SVGProps } from "react";

  const component: FC<SVGProps<SVGSVGElement>>;
  export default component;
}

