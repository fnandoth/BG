"use client";

import { useEffect } from "react";
import { SessionUser } from "@/lib/types";
import { useAuthStore } from "@/store/auth-store";

export function AuthHydrator({ user }: { user: SessionUser | null }) {
  const setSession = useAuthStore((state) => state.setSession);
  const clearSession = useAuthStore((state) => state.clearSession);

  useEffect(() => {
    if (user) {
      setSession(user);
      return;
    }

    clearSession();
  }, [clearSession, setSession, user]);

  return null;
}
