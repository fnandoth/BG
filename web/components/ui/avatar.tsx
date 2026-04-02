/* eslint-disable @next/next/no-img-element */

import { getInitials, cn } from "@/lib/utils";

interface AvatarProps {
  name: string;
  src?: string | null;
  className?: string;
}

export function Avatar({ name, src, className }: AvatarProps) {
  if (src) {
    return (
      <img
        alt={name}
        className={cn("h-11 w-11 rounded-full border border-white/10 bg-muted object-cover", className)}
        src={src}
      />
    );
  }

  return (
    <div
      className={cn(
        "flex h-11 w-11 items-center justify-center rounded-full border border-white/10 bg-muted font-mono text-[11px] uppercase tracking-[0.12em] text-muted-foreground",
        className,
      )}
    >
      {getInitials(name)}
    </div>
  );
}
