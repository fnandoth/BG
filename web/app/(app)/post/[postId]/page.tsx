"use client";

import { useInfiniteQuery, useQuery } from "@tanstack/react-query";
import { use } from "react";
import { PostCard } from "@/components/posts/post-card";
import { PostComposer } from "@/components/posts/post-composer";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/ui/empty-state";
import { Skeleton } from "@/components/ui/skeleton";
import { PAGE_SIZE } from "@/lib/constants";
import { apiFetch } from "@/lib/api/client";
import { PostDetail, PostType } from "@/lib/types";
import { useAuthStore } from "@/store/auth-store";

export default function PostDetailPage({
  params,
}: {
  params: Promise<{ postId: string }>;
}) {
  const { postId } = use(params);
  const user = useAuthStore((state) => state.user);

  const postQuery = useQuery({
    queryKey: ["post-detail", postId],
    queryFn: () => apiFetch<PostDetail>(`/api/posts/${postId}`),
  });

  const repliesQuery = useInfiniteQuery({
    queryKey: ["replies", postId],
    queryFn: ({ pageParam = 1 }) =>
      apiFetch<PostDetail[]>(
        `/api/posts/${postId}/replies?page=${pageParam}&pageSize=${PAGE_SIZE}`,
      ),
    initialPageParam: 1,
    getNextPageParam: (lastPage, _pages, lastPageParam) =>
      lastPage.length === PAGE_SIZE ? lastPageParam + 1 : undefined,
  });

  const replies = repliesQuery.data?.pages.flatMap((page) => page) ?? [];

  if (postQuery.isPending) {
    return <Skeleton className="h-80 w-full rounded-[28px]" />;
  }

  if (postQuery.isError || !postQuery.data) {
    return <EmptyState description="No pudimos cargar este post." title="Post no disponible" />;
  }

  return (
    <div className="space-y-4">
      <PostCard currentUserId={user?.id} post={postQuery.data} />

      {user ? (
        <PostComposer
          parentPostId={postId}
          placeholder="Responde a esta publicacion"
          submitLabel="Responder"
          type={PostType.Reply}
        />
      ) : null}

      <section className="panel-card rounded-[2rem] border bg-card p-5">
        <p className="panel-label">Replies</p>
        <h2 className="mt-4 text-xl">Conversation</h2>

        <div className="mt-4 space-y-4">
          {repliesQuery.isPending ? (
            <>
              <Skeleton className="h-36 w-full rounded-[28px]" />
              <Skeleton className="h-36 w-full rounded-[28px]" />
            </>
          ) : replies.length === 0 ? (
            <EmptyState
              description="Todavia no hay respuestas para este post."
              title="Sin replies"
            />
          ) : (
            replies.map((reply) => (
              <PostCard currentUserId={user?.id} key={reply.id} post={reply} />
            ))
          )}
        </div>
      </section>

      {repliesQuery.hasNextPage ? (
        <Button
          className="w-full"
          onClick={() => repliesQuery.fetchNextPage()}
          variant="secondary"
        >
          {repliesQuery.isFetchingNextPage ? "Cargando..." : "Cargar mas replies"}
        </Button>
      ) : null}
    </div>
  );
}
