import * as React from "react";
import { cn } from "@/lib/utils";

export function Input({ className, ...props }: React.InputHTMLAttributes<HTMLInputElement>) {
  return (
    <input
      className={cn(
        "h-12 w-full rounded-[1.25rem] border bg-background/80 px-4 text-[15px] text-foreground outline-none placeholder:text-muted-foreground focus:border-foreground/40",
        className,
      )}
      {...props}
    />
  );
}
