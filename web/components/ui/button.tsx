"use client";

import * as React from "react";
import { cn } from "@/lib/utils";

const variants = {
  primary:
    "border-transparent bg-foreground text-background hover:bg-foreground/88 disabled:bg-foreground/45",
  secondary:
    "border-border bg-transparent text-foreground hover:border-foreground/25 hover:bg-muted/35 disabled:border-border disabled:text-muted-foreground",
  ghost:
    "border-transparent bg-transparent text-muted-foreground hover:text-foreground disabled:text-muted-foreground/70",
  danger:
    "border-accent bg-transparent text-accent hover:bg-accent/10 disabled:border-accent/40 disabled:text-accent/50",
};

const sizes = {
  md: "h-11 px-5 text-[11px]",
  sm: "h-10 px-4 text-[11px]",
  icon: "h-10 w-10 text-[11px]",
};

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: keyof typeof variants;
  size?: keyof typeof sizes;
};

export function Button({
  className,
  variant = "primary",
  size = "md",
  ...props
}: ButtonProps) {
  return (
    <button
      className={cn(
        "ui-mono inline-flex items-center justify-center rounded-full border uppercase tracking-[0.14em] transition-colors focus:outline-none focus:ring-2 focus:ring-foreground/20 disabled:cursor-not-allowed",
        variants[variant],
        sizes[size],
        className,
      )}
      {...props}
    />
  );
}
