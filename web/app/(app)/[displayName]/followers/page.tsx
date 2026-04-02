"use client";

import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { use } from "react";
import { Avatar } from "@/components/ui/avatar";
import { EmptyState } from "@/components/ui/empty-state";
import { Skeleton } from "@/components/ui/skeleton";
import { apiFetch } from "@/lib/api/client";
import { UserPlain } from "@/lib/types";
import { formatRelativeDate } from "@/lib/utils";

export default function FollowersPage({
  params,
}: {
  params: Promise<{ displayName: string }>;
}) {
  const { displayName: routeDisplayName } = use(params);
  const displayName = decodeURIComponent(routeDisplayName);

  const followersQuery = useQuery({
    queryKey: ["followers", displayName, "list"],
    queryFn: () => apiFetch<UserPlain[]>(`/api/user/${displayName}/followers`),
  });

  return (
    <div className="space-y-4">
      <section className="rounded-[28px] border bg-card p-5">
        <h1 className="text-xl font-semibold">Seguidores de {displayName}</h1>
      </section>

      {followersQuery.isPending ? (
        <>
          <Skeleton className="h-24 w-full rounded-[28px]" />
          <Skeleton className="h-24 w-full rounded-[28px]" />
        </>
      ) : followersQuery.data?.length ? (
        <div className="space-y-3">
          {followersQuery.data.map((follower) => (
            <Link
              className="flex items-center gap-4 rounded-[28px] border bg-card p-5 transition hover:bg-muted/30"
              href={`/${follower.displayName}`}
              key={follower.displayName}
            >
              <Avatar name={follower.displayName} src={follower.avatarUrl} />
              <div>
                <p className="font-medium">{follower.displayName}</p>
                <p className="text-sm text-muted-foreground">{follower.bio || "Sin bio"}</p>
                <p className="mt-1 text-xs text-muted-foreground">
                  Desde {formatRelativeDate(follower.createdAt)}
                </p>
              </div>
            </Link>
          ))}
        </div>
      ) : (
        <EmptyState
          description="Este usuario todavía no tiene seguidores."
          title="Lista vacía"
        />
      )}
    </div>
  );
}
