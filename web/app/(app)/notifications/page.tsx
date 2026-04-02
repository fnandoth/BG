"use client";

import Link from "next/link";
import { useInfiniteQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/ui/empty-state";
import { Avatar } from "@/components/ui/avatar";
import { Skeleton } from "@/components/ui/skeleton";
import { PAGE_SIZE } from "@/lib/constants";
import { apiFetch, jsonBody, mutationErrorMessage } from "@/lib/api/client";
import { NotificationItem, PagedResult } from "@/lib/types";
import { formatRelativeDate } from "@/lib/utils";
import { useAuthStore } from "@/store/auth-store";

const iconByType: Record<string, string> = {
  like: "❤️",
  reply: "💬",
  repost: "🔁",
  mention: "💬",
  follow: "👤",
  quote: "📝",
};

export default function NotificationsPage() {
  const queryClient = useQueryClient();
  const setUnreadCount = useAuthStore((state) => state.setUnreadCount);

  const notificationsQuery = useInfiniteQuery({
    queryKey: ["notifications", "feed"],
    queryFn: ({ pageParam = 1 }) =>
      apiFetch<PagedResult<NotificationItem>>(
        `/api/notifications?page=${pageParam}&pageSize=${PAGE_SIZE}&onlyUnread=false`,
      ),
    initialPageParam: 1,
    getNextPageParam: (lastPage) => (lastPage.hasNextPage ? lastPage.page + 1 : undefined),
    refetchInterval: 20_000,
  });

  const markReadMutation = useMutation({
    mutationFn: (notificationIds: string[] | null) =>
      apiFetch<void>("/api/notifications/read", {
        method: "PATCH",
        body: jsonBody(notificationIds ? { notificationIds } : null),
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
      queryClient.invalidateQueries({ queryKey: ["notifications", "unread-count"] });
    },
    onError: (error) => {
      toast.error(mutationErrorMessage(error));
    },
  });

  const pages = notificationsQuery.data?.pages ?? [];
  const items = pages.flatMap((page) => page.items);
  const unreadCount = items.filter((item) => !item.isRead).length;

  useEffect(() => {
    setUnreadCount(unreadCount);
  }, [setUnreadCount, unreadCount]);

  return (
    <div className="space-y-4">
      <section className="rounded-[28px] border bg-card p-5">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h1 className="text-xl font-semibold">Notificaciones</h1>
            <p className="mt-2 text-sm text-muted-foreground">
              Polling cada 20 segundos para mantener el badge actualizado.
            </p>
          </div>
          <Button
            disabled={markReadMutation.isPending || unreadCount === 0}
            onClick={() => markReadMutation.mutate(null)}
            variant="secondary"
          >
            Marcar todas como leídas
          </Button>
        </div>
      </section>

      <section className="space-y-3">
        {notificationsQuery.isPending ? (
          <>
            <Skeleton className="h-28 w-full rounded-[28px]" />
            <Skeleton className="h-28 w-full rounded-[28px]" />
            <Skeleton className="h-28 w-full rounded-[28px]" />
          </>
        ) : items.length === 0 ? (
          <EmptyState
            description="Todavía no hay eventos para mostrar."
            title="Todo al día"
          />
        ) : (
          items.map((notification) => {
            const href = notification.post
              ? `/post/${notification.post.postId}`
              : `/${notification.actor.displayName}`;

            return (
              <article
                className="rounded-[28px] border bg-card p-5 transition hover:bg-muted/30"
                key={notification.id}
              >
                <div className="flex items-start gap-3">
                  <div className="text-2xl">{iconByType[notification.type] ?? "🔔"}</div>
                  <Avatar
                    className="h-10 w-10"
                    name={notification.actor.displayName}
                    src={notification.actor.avatarUrl}
                  />
                  <div className="min-w-0 flex-1">
                    <div className="flex flex-wrap items-center gap-2">
                      <p className="font-medium">{notification.actor.displayName}</p>
                      {!notification.isRead ? (
                        <span className="rounded-full bg-accent px-2 py-0.5 text-xs text-accent-foreground">
                          Nuevo
                        </span>
                      ) : null}
                    </div>
                    <p className="mt-1 text-sm text-muted-foreground">
                      {notification.post?.contentPreview || `Actividad de tipo ${notification.type}`}
                    </p>
                    <div className="mt-3 flex items-center justify-between gap-3">
                      <span className="text-xs text-muted-foreground">
                        {formatRelativeDate(notification.createdAt)}
                      </span>
                      <div className="flex items-center gap-3">
                        {!notification.isRead ? (
                          <Button
                            onClick={() => markReadMutation.mutate([notification.id])}
                            size="sm"
                            variant="secondary"
                          >
                            Marcar leída
                          </Button>
                        ) : null}
                        <Link className="text-sm font-medium text-foreground underline" href={href}>
                          Abrir
                        </Link>
                      </div>
                    </div>
                  </div>
                </div>
              </article>
            );
          })
        )}
      </section>

      {notificationsQuery.hasNextPage ? (
        <Button
          className="w-full"
          onClick={() => notificationsQuery.fetchNextPage()}
          variant="secondary"
        >
          {notificationsQuery.isFetchingNextPage ? "Cargando..." : "Cargar más"}
        </Button>
      ) : null}
    </div>
  );
}
