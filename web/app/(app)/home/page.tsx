"use client";

import { useInfiniteQuery } from "@tanstack/react-query";
import { PostCard } from "@/components/posts/post-card";
import { PostComposer } from "@/components/posts/post-composer";
import { EmptyState } from "@/components/ui/empty-state";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { PAGE_SIZE } from "@/lib/constants";
import { apiFetch } from "@/lib/api/client";
import { PostType, TimelinePost } from "@/lib/types";
import { useAuthStore } from "@/store/auth-store";

export default function HomePage() {
  const user = useAuthStore((state) => state.user);

  const timelineQuery = useInfiniteQuery({
    queryKey: ["timeline", user?.id],
    queryFn: ({ pageParam = 1 }) =>
      apiFetch<TimelinePost[]>(
        `/api/posts/timeline/${user?.displayName}?page=${pageParam}&pageSize=${PAGE_SIZE}`,
      ),
    initialPageParam: 1,
    enabled: Boolean(user?.displayName),
    getNextPageParam: (lastPage, _pages, lastPageParam) =>
      lastPage.length === PAGE_SIZE ? lastPageParam + 1 : undefined,
  });

  const posts = timelineQuery.data?.pages.flatMap((page) => page) ?? [];

  return (
    <div className="space-y-4">
     

      <PostComposer placeholder="¿Qué está pasando?" submitLabel="Publicar" type={PostType.Post} />

      {timelineQuery.isPending ? (
        <div className="space-y-4">
          <Skeleton className="h-44 w-full rounded-[28px]" />
          <Skeleton className="h-44 w-full rounded-[28px]" />
          <Skeleton className="h-44 w-full rounded-[28px]" />
        </div>
      ) : posts.length === 0 ? (
        <EmptyState
          description="Aún no hay publicaciones en tu timeline. Realiza algunas publicaciones o sigue a otros usuarios para ver su contenido aquí"
          title="Feed vacío"
        />
      ) : (
        <div className="space-y-4">
          {posts.map((post) => (
            <PostCard currentUserId={user?.id} key={post.id} post={post} />
          ))}
        </div>
      )}

      {timelineQuery.hasNextPage ? (
        <Button
          className="w-full"
          onClick={() => timelineQuery.fetchNextPage()}
          variant="secondary"
        >
          {timelineQuery.isFetchingNextPage ? "Cargando..." : "Cargar más"}
        </Button>
      ) : null}
    </div>
  );
}
