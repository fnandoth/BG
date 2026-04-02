"use client";

import { useInfiniteQuery, useMutation, useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { use, useEffect, useState } from "react";
import { toast } from "sonner";
import { Avatar } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/ui/empty-state";
import { Skeleton } from "@/components/ui/skeleton";
import { PostCard } from "@/components/posts/post-card";
import { PAGE_SIZE } from "@/lib/constants";
import { apiFetch, mutationErrorMessage } from "@/lib/api/client";
import { PostType, TimelinePost, UserPlain } from "@/lib/types";
import { formatRelativeDate } from "@/lib/utils";
import { useAuthStore } from "@/store/auth-store";

export default function ProfilePage({
  params,
}: {
  params: Promise<{ displayName: string }>;
}) {
  const routeParams = use(params);
  const [tab, setTab] = useState<"posts" | "likes" | "reposts">("posts");
  const user = useAuthStore((state) => state.user);
  const knownUsers = useAuthStore((state) => state.knownUsers);
  const rememberUsers = useAuthStore((state) => state.rememberUsers);
  const displayName = decodeURIComponent(routeParams.displayName);

  const profileQuery = useQuery({
    queryKey: ["profile", displayName],
    queryFn: () => apiFetch<UserPlain>(`/api/user/${displayName}`),
  });

  const followersQuery = useQuery({
    queryKey: ["followers", displayName],
    queryFn: () => apiFetch<UserPlain[]>(`/api/user/${displayName}/followers`),
  });

  const isOwnProfile = user?.displayName === displayName;
  // const resolvedUserId = isOwnProfile ? user?.id : knownUsers[displayName]?.id;
  const isFollowing =
    followersQuery.data?.some((follower) => follower.displayName === user?.displayName) ?? false;

  useEffect(() => {
    if (user) {
      rememberUsers([
        {
          displayName: user.displayName,
          id: user.id,
          avatarUrl: user.avatarUrl,
        },
      ]);
    }
  }, [rememberUsers, user]);

  const postsQuery = useInfiniteQuery({
    queryKey: ["profile-posts", displayName, displayName, tab],
    queryFn: ({ pageParam = 1 }) => {
      if (!displayName) {
        return Promise.resolve([] as TimelinePost[]);
      }

      if (tab === "likes") {
        return apiFetch<TimelinePost[]>(
          `/api/likes/user/${displayName}?page=${pageParam}&pageSize=${PAGE_SIZE}`,
        );
      }

      return apiFetch<TimelinePost[]>(
        `/api/posts/user/${displayName}?page=${pageParam}&pageSize=${PAGE_SIZE}`,
      );
    },
    initialPageParam: 1,
    enabled: Boolean(displayName) && (tab !== "likes" || Boolean(user)),
    getNextPageParam: (lastPage, _pages, lastPageParam) =>
      lastPage.length === PAGE_SIZE ? lastPageParam + 1 : undefined,
  });

  const followMutation = useMutation({
    mutationFn: async () => {
      const method = isFollowing ? "DELETE" : "POST";
      const path = isFollowing ? "unfollow" : "follow";
      return apiFetch<{ message: string }>(`/api/user/${path}/${displayName}`, {
        method,
      });
    },
    onSuccess: () => {
      followersQuery.refetch();
      toast.success(isFollowing ? "Dejaste de seguir al usuario." : "Ahora sigues a este usuario.");
    },
    onError: (error) => {
      toast.error(mutationErrorMessage(error));
    },
  });

  useEffect(() => {
    rememberUsers(
      (followersQuery.data ?? []).map((follower) => ({
        displayName: follower.displayName,
        avatarUrl: follower.avatarUrl,
      })),
    );
  }, [followersQuery.data, rememberUsers]);

  const posts = (postsQuery.data?.pages.flatMap((page) => page) ?? []).filter((post) =>
    tab === "reposts" ? post.postType === PostType.Repost : true,
  );

  if (profileQuery.isPending) {
    return <Skeleton className="h-80 w-full rounded-[28px]" />;
  }

  if (profileQuery.isError || !profileQuery.data) {
    return (
      <EmptyState
        description="No encontramos ese perfil."
        title="Perfil no disponible"
      />
    );
  }

  return (
    <div className="space-y-4">
      <section className="rounded-[28px] border bg-card p-6">
        <div className="flex flex-col gap-5 md:flex-row md:items-start md:justify-between">
          <div className="flex items-start gap-4">
            <Avatar
              className="h-16 w-16"
              name={profileQuery.data.displayName}
              src={profileQuery.data.avatarUrl}
            />
            <div>
              <h1 className="text-2xl font-semibold">{profileQuery.data.displayName}</h1>
              <p className="mt-2 max-w-xl text-sm text-muted-foreground">
                {profileQuery.data.bio || "Este usuario no ha escrito una bio todavía."}
              </p>
              <p className="mt-3 text-xs uppercase tracking-[0.18em] text-muted-foreground">
                Desde {formatRelativeDate(profileQuery.data.createdAt)}
              </p>
              <Link
                className="mt-3 inline-flex text-sm underline"
                href={`/${displayName}/followers`}
              >
                {followersQuery.data?.length ?? 0} seguidores
              </Link>
            </div>
          </div>

          <div className="flex flex-wrap gap-3">
            {isOwnProfile ? (
              <Link
                className="inline-flex h-10 items-center rounded-full bg-accent px-4 text-sm font-medium text-accent-foreground"
                href="/settings"
              >
                Editar perfil
              </Link>
            ) : user ? (
              <Button
                disabled={followMutation.isPending}
                onClick={() => followMutation.mutate()}
                variant={isFollowing ? "secondary" : "primary"}
              >
                {isFollowing ? "Dejar de seguir" : "Seguir"}
              </Button>
            ) : null}
          </div>
        </div>
      </section>

      <section className="rounded-[28px] border bg-card p-4">
        <div className="flex flex-wrap gap-2">
          {[
            { key: "posts", label: "Posts" },
            { key: "likes", label: "Likes" },
            { key: "reposts", label: "Reposts" },
          ].map((item) => (
            <Button
              key={item.key}
              onClick={() => setTab(item.key as "posts" | "likes" | "reposts")}
              variant={tab === item.key ? "primary" : "secondary"}
            >
              {item.label}
            </Button>
          ))}
        </div>
      </section>

      {!displayName && !isOwnProfile ? (
        <EmptyState
          description="El backend no expone el id público del usuario. Si llegas desde un post o notificación, el frontend podrá resolverlo desde caché y cargar sus publicaciones."
          title="Posts limitados por contrato actual"
        />
      ) : tab === "likes" && !user ? (
        <EmptyState
          description="El backend exige sesión para consultar likes de un usuario."
          title="Likes privados"
        />
      ) : postsQuery.isPending ? (
        <div className="space-y-4">
          <Skeleton className="h-40 w-full rounded-[28px]" />
          <Skeleton className="h-40 w-full rounded-[28px]" />
        </div>
      ) : posts.length === 0 ? (
        <EmptyState
          description="No hay publicaciones disponibles para esta pestaña."
          title="Sin contenido"
        />
      ) : (
        <div className="space-y-4">
          {posts.map((post) => (
            <PostCard currentUserId={user?.id} key={post.id} post={post} />
          ))}
        </div>
      )}

      {postsQuery.hasNextPage ? (
        <Button
          className="w-full"
          onClick={() => postsQuery.fetchNextPage()}
          variant="secondary"
        >
          {postsQuery.isFetchingNextPage ? "Cargando..." : "Cargar más"}
        </Button>
      ) : null}
    </div>
  );
}
