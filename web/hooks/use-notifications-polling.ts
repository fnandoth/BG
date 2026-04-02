"use client";

import { useQuery } from "@tanstack/react-query";
import { useEffect } from "react";
import { apiFetch } from "@/lib/api/client";
import { PagedResult, NotificationItem } from "@/lib/types";
import { useAuthStore } from "@/store/auth-store";

export function useNotificationsPolling(enabled: boolean) {
  const setUnreadCount = useAuthStore((state) => state.setUnreadCount);
  const rememberUsers = useAuthStore((state) => state.rememberUsers);
  const query = useQuery({
    queryKey: ["notifications", "unread-count"],
    queryFn: () =>
      apiFetch<PagedResult<NotificationItem>>(
        "/api/notifications?page=1&pageSize=100&onlyUnread=true",
      ),
    enabled,
    refetchInterval: 20_000,
    staleTime: 15_000,
  });

  useEffect(() => {
    if (!query.data) {
      if (!enabled) {
        setUnreadCount(0);
      }
      return;
    }

    setUnreadCount(query.data.totalCount);
    rememberUsers(
      query.data.items.map((item) => ({
        displayName: item.actor.displayName,
        id: item.actor.userId,
        avatarUrl: item.actor.avatarUrl,
      })),
    );
  }, [enabled, query.data, rememberUsers, setUnreadCount]);

  return query;
}
