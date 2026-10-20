import type { HTMLAttributes } from "react";

interface ContainerProps extends HTMLAttributes<HTMLDivElement> {
  className?: string;
}

export default function Container({ children, className, ...props }: ContainerProps) {
  return (
    <div className={`px-4 mx-auto xl:max-w-7xl xl:px-0 ${className ?? ""}`} {...props}>
      {children}
    </div>
  )
}
