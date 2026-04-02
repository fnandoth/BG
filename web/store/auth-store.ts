"use client";

import { create } from "zustand";
import { SessionUser } from "@/lib/types";

interface KnownUser {
  displayName: string;
  id?: string;
  avatarUrl?: string | null;
}

interface AuthState {
  user: SessionUser | null;
  unreadCount: number;
  knownUsers: Record<string, KnownUser>;
  setSession: (user: SessionUser | null) => void;
  clearSession: () => void;
  setUnreadCount: (count: number) => void;
  rememberUsers: (users: KnownUser[]) => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  unreadCount: 0,
  knownUsers: {},
  setSession: (user) =>
    set((state) => ({
      user,
      knownUsers: user
        ? {
            ...state.knownUsers,
            [user.displayName]: {
              displayName: user.displayName,
              id: user.id,
              avatarUrl: user.avatarUrl,
            },
          }
        : state.knownUsers,
    })),
  clearSession: () => set({ user: null, unreadCount: 0 }),
  setUnreadCount: (count) => set({ unreadCount: count }),
  rememberUsers: (users) =>
    set((state) => {
      const knownUsers = { ...state.knownUsers };

      for (const user of users) {
        if (!user.displayName) {
          continue;
        }

        knownUsers[user.displayName] = {
          ...knownUsers[user.displayName],
          ...user,
        };
      }

      return { knownUsers };
    }),
}));
