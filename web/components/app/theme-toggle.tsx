"use client";

import { useState, useEffect } from "react";
import { useTheme } from "next-themes";
import { Button } from "@/components/ui/button";

export function ThemeToggle() {
  const { setTheme, resolvedTheme } = useTheme();
  const [mounted, setMounted] = useState(false);

  useEffect(() => setMounted(true), []);

  const nextTheme = resolvedTheme === "dark" ? "light" : "dark";
  const label = resolvedTheme === "dark" ? "Modo claro" : "Modo oscuro";

  return (
    <Button
      className="w-full justify-center"
      onClick={() => setTheme(nextTheme)}
      type="button"
      variant="secondary"
      disabled={!mounted}
    >
      {mounted ? label : null}
    </Button>
  );
}