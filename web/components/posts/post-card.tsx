"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import Link from "next/link";
import { type ReactNode, useEffect, useState } from "react";
import { toast } from "sonner";
import { PostComposer } from "@/components/posts/post-composer";
import { Avatar } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { apiFetch, mutationErrorMessage } from "@/lib/api/client";
import { PostDetail, PostSummary, PostType, TimelinePost, ToggleLikeResponse } from "@/lib/types";
import { cn, formatRelativeDate } from "@/lib/utils";
import { useAuthStore } from "@/store/auth-store";

type PostCardProps = {
  post: TimelinePost | PostDetail;
  currentUserId?: string;
};

type EmbeddedPostProps = {
  post: PostSummary;
  title: string;
  variant: "quote" | "repost";
};

type IconProps = {
  className?: string;
};

type ActionButtonProps = {
  active?: boolean;
  count: number;
  disabled?: boolean;
  icon: ReactNode;
  label: string;
  onClick: () => void;
};

const postTypeLabels: Record<PostType, string> = {
  [PostType.Post]: "Post",
  [PostType.Reply]: "Reply",
  [PostType.Repost]: "Repost",
  [PostType.Quote]: "Quote",
};

function HeartIcon({ className }: IconProps) {
  return (
    <svg
      aria-hidden="true"
      className={className}
      fill="none"
      stroke="currentColor"
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth="1.6"
      viewBox="0 0 24 24"
    >
      <path d="M12 20.5 4.9 13.8a4.8 4.8 0 0 1 6.8-6.8L12 7.3l.3-.3a4.8 4.8 0 0 1 6.8 6.8Z" />
    </svg>
  );
}

function ReplyIcon({ className }: IconProps) {
  return (
    <svg
      aria-hidden="true"
      className={className}
      fill="none"
      stroke="currentColor"
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth="1.6"
      viewBox="0 0 24 24"
    >
      <path d="M9 8 4 12l5 4" />
      <path d="M5 12h8a6 6 0 0 1 6 6" />
    </svg>
  );
}

function RepostIcon({ className }: IconProps) {
  return (
    <svg
      aria-hidden="true"
      className={className}
      fill="none"
      stroke="currentColor"
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth="1.6"
      viewBox="0 0 24 24"
    >
      <path d="M17 4 21 8l-4 4" />
      <path d="M3 12V9a5 5 0 0 1 5-5h13" />
      <path d="M7 20 3 16l4-4" />
      <path d="M21 12v3a5 5 0 0 1-5 5H3" />
    </svg>
  );
}

function QuoteIcon({ className }: IconProps) {
  return (
    <svg
      aria-hidden="true"
      className={className}
      fill="none"
      stroke="currentColor"
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth="1.6"
      viewBox="0 0 24 24"
    >
      <path d="M7 17h4V9H7z" />
      <path d="M13 17h4V9h-4z" />
      <path d="M7 9V7h4" />
      <path d="M13 9V7h4" />
    </svg>
  );
}

function ArrowUpRightIcon({ className }: IconProps) {
  return (
    <svg
      aria-hidden="true"
      className={className}
      fill="none"
      stroke="currentColor"
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth="1.6"
      viewBox="0 0 24 24"
    >
      <path d="M7 17 17 7" />
      <path d="M9 7h8v8" />
    </svg>
  );
}

function ActionButton({ active, count, disabled, icon, label, onClick }: ActionButtonProps) {
  return (
    <Button
      aria-label={label}
      className={cn(
        "gap-2 rounded-full border border-transparent px-3 normal-case tracking-[0.04em]",
        active && "border-accent/40 bg-accent/10 text-accent",
        !active && "hover:border-border",
      )}
      disabled={disabled}
      onClick={onClick}
      size="sm"
      type="button"
      variant="ghost"
    >
      {icon}
      <span className="sr-only">{label}</span>
      <span className={cn("ui-mono text-[11px] tabular-nums", active ? "text-accent" : "text-muted-foreground")}>
        {count}
      </span>
    </Button>
  );
}

function EmbeddedPost({ post, title, variant }: EmbeddedPostProps) {
  return (
    <Link
      className={cn(
        "group block rounded-[1.5rem] border p-4 transition-colors",
        variant === "quote"
          ? "border-white/12 bg-background/75 hover:border-foreground/20"
          : "border-border bg-background/85 hover:border-foreground/20",
      )}
      href={`/post/${post.id}`}
    >
      <div className="flex items-center justify-between gap-3">
        <p className="panel-label">{title}</p>
        <ArrowUpRightIcon className="h-4 w-4 text-muted-foreground transition-colors group-hover:text-foreground" />
      </div>

      <div className="mt-4 flex items-start gap-3">
        <Avatar className="h-10 w-10" name={post.author.displayName} src={post.author.avatarUrl} />
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-x-2 gap-y-1">
            <span className="truncate text-[15px] font-medium">{post.author.displayName}</span>
            <span className="ui-mono text-[11px] uppercase tracking-[0.12em] text-muted-foreground">
              @{post.author.username}
            </span>
          </div>
          <p className="mt-3 whitespace-pre-wrap text-sm leading-6 text-muted-foreground">
            {post.content?.trim() || "Sin texto"}
          </p>
        </div>
      </div>
    </Link>
  );
}

export function PostCard({ post, currentUserId }: PostCardProps) {
  const [showReply, setShowReply] = useState(false);
  const [showQuote, setShowQuote] = useState(false);
  const [likedState, setLikedState] = useState<boolean | null>(null);
  const [likesCount, setLikesCount] = useState(post.likesCount);
  const queryClient = useQueryClient();
  const rememberUsers = useAuthStore((state) => state.rememberUsers);

  useEffect(() => {
    rememberUsers([
      {
        displayName: post.author.displayName,
        id: post.author.userId,
        avatarUrl: post.author.avatarUrl,
      },
    ]);
  }, [post.author.avatarUrl, post.author.displayName, post.author.userId, rememberUsers]);

  const likedQuery = useQuery({
    queryKey: ["like-status", post.id, currentUserId],
    queryFn: () => apiFetch<{ isLiked: boolean }>(`/api/likes/${post.id}/status/${currentUserId}`),
    enabled: Boolean(currentUserId),
  });

  const likeMutation = useMutation({
    mutationFn: () =>
      apiFetch<ToggleLikeResponse>(`/api/likes/${post.id}`, {
        method: "POST",
      }),
    onMutate: async () => {
      const currentlyLiked = likedState ?? likedQuery.data?.isLiked ?? false;
      setLikedState(!currentlyLiked);
      setLikesCount((count) => count + (currentlyLiked ? -1 : 1));
    },
    onSuccess: (data) => {
      setLikedState(data.liked);
      queryClient.invalidateQueries({ queryKey: ["like-status", post.id, currentUserId] });
    },
    onError: (error) => {
      const original = likedQuery.data?.isLiked ?? false;
      setLikedState(original);
      setLikesCount(post.likesCount);
      toast.error(mutationErrorMessage(error));
    },
  });

  const repostMutation = useMutation({
    mutationFn: () =>
      apiFetch<TimelinePost>(`/api/posts/${post.id}/repost`, {
        method: "POST",
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["timeline"] });
      queryClient.invalidateQueries({ queryKey: ["profile-posts"] });
      toast.success("Repost realizado.");
    },
    onError: (error) => {
      toast.error(mutationErrorMessage(error));
    },
  });

  const deleteMutation = useMutation({
    mutationFn: () =>
      apiFetch<void>(`/api/posts/${post.id}`, {
        method: "DELETE",
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["timeline"] });
      queryClient.invalidateQueries({ queryKey: ["post-detail"] });
      queryClient.invalidateQueries({ queryKey: ["profile-posts"] });
      toast.success("Post eliminado.");
    },
    onError: (error) => {
      toast.error(mutationErrorMessage(error));
    },
  });

  const isOwnPost = currentUserId === post.author.userId;
  const isLiked = likedState ?? likedQuery.data?.isLiked ?? false;
  const quotedPost = "quotedPost" in post ? post.quotedPost ?? null : null;
  const repostedPost = "repostedPost" in post ? post.repostedPost ?? null : null;
  const isRepostCard = post.postType === PostType.Repost && Boolean(repostedPost);
  const isQuoteCard = post.postType === PostType.Quote && Boolean(quotedPost);

  return (
    <article
      className={cn(
        "panel-card rounded-[2rem] border p-5 sm:p-6",
        isRepostCard
          ? "border-white/15 bg-muted/65"
          : isQuoteCard
            ? "border-white/12 bg-card"
            : "bg-card",
      )}
    >
      {isRepostCard && repostedPost ? (
        <div>
          <div className="mb-4 flex flex-wrap items-center gap-2">
            <RepostIcon className="h-4 w-4 text-muted-foreground" />
            <span className="panel-label">Reposted by</span>
            <span className="text-sm font-medium text-foreground">{post.author.displayName}</span>
          </div>

          <EmbeddedPost post={repostedPost} title="Original Post" variant="repost" />
        </div>
      ) : (
        <div className="flex items-start gap-4">
          <Link href={`/${post.author.displayName}`}>
            <Avatar name={post.author.displayName} src={post.author.avatarUrl} />
          </Link>

          <div className="min-w-0 flex-1">
            {isQuoteCard ? (
              <div className="mb-3 flex flex-wrap items-center gap-2 text-muted-foreground">
                <QuoteIcon className="h-4 w-4" />
                <span className="panel-label">Quote</span>
              </div>
            ) : null}

            <div className="flex flex-wrap items-center gap-x-2 gap-y-1">
              <Link className="truncate text-[15px] font-medium hover:text-foreground/80" href={`/${post.author.displayName}`}>
                {post.author.displayName}
              </Link>
              <span className="ui-mono text-[11px] uppercase tracking-[0.12em] text-muted-foreground">
                @{post.author.username}
              </span>
            </div>

            <div className="mt-2 flex flex-wrap items-center gap-2 text-muted-foreground">
              <span className="panel-label">{postTypeLabels[post.postType]}</span>
              <span className="h-1 w-1 rounded-full bg-muted-foreground/70" />
              <span className="ui-mono text-[11px] uppercase tracking-[0.12em]">
                {formatRelativeDate(post.createdAt)}
              </span>
            </div>

            {post.content?.trim() ? (
              <p className="mt-4 whitespace-pre-wrap text-[15px] leading-7">{post.content}</p>
            ) : null}

            {quotedPost ? <div className="mt-5"><EmbeddedPost post={quotedPost} title="Quoted Post" variant="quote" /></div> : null}
          </div>
        </div>
      )}

      <div className="mt-5 flex flex-wrap items-center gap-2 border-t border-border pt-4">
        <ActionButton
          active={isLiked}
          count={likesCount}
          disabled={!currentUserId || likeMutation.isPending}
          icon={<HeartIcon className={cn("h-4 w-4", isLiked ? "text-accent" : "text-foreground")} />}
          label="Like"
          onClick={() => likeMutation.mutate()}
        />
        <ActionButton
          count={post.repliesCount}
          disabled={false}
          icon={<ReplyIcon className="h-4 w-4 text-foreground" />}
          label="Reply"
          onClick={() => setShowReply((value) => !value)}
        />
        <ActionButton
          count={post.repostsCount}
          disabled={!currentUserId || repostMutation.isPending}
          icon={<RepostIcon className="h-4 w-4 text-foreground" />}
          label="Repost"
          onClick={() => repostMutation.mutate()}
        />
        <ActionButton
          count={post.quotesCount}
          disabled={false}
          icon={<QuoteIcon className="h-4 w-4 text-foreground" />}
          label="Quote"
          onClick={() => setShowQuote((value) => !value)}
        />

        <Link
          className="ui-mono inline-flex h-10 items-center gap-2 rounded-full border border-border px-3 text-[11px] uppercase tracking-[0.14em] text-muted-foreground transition hover:border-foreground/25 hover:text-foreground"
          href={`/post/${post.id}`}
        >
          <ArrowUpRightIcon className="h-4 w-4" />
          Ver detalle
        </Link>

        {isOwnPost ? (
          <Button
            onClick={() => {
              if (window.confirm("Eliminar este post?")) {
                deleteMutation.mutate();
              }
            }}
            size="sm"
            variant="danger"
          >
            Eliminar
          </Button>
        ) : null}
      </div>

      {showReply ? (
        <div className="mt-4">
          <PostComposer
            onSuccess={() => setShowReply(false)}
            parentPostId={post.id}
            placeholder="Escribe tu respuesta"
            submitLabel="Responder"
            type={PostType.Reply}
          />
        </div>
      ) : null}

      {showQuote ? (
        <div className="mt-4">
          <PostComposer
            onSuccess={() => setShowQuote(false)}
            placeholder="Agrega contexto a tu cita"
            quotedPostId={post.id}
            submitLabel="Citar"
            type={PostType.Quote}
          />
        </div>
      ) : null}
    </article>
  );
}
