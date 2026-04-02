import * as React from "react";
import { cn } from "@/lib/utils";

export function Textarea({
  className,
  ...props
}: React.TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return (
    <textarea
      className={cn(
        "min-h-32 w-full rounded-[1.5rem] border bg-background/80 px-4 py-4 text-[15px] leading-6 text-foreground outline-none placeholder:text-muted-foreground focus:border-foreground/40",
        className,
      )}
      {...props}
    />
  );
}
