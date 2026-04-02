"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { apiFetch, jsonBody, mutationErrorMessage } from "@/lib/api/client";
import { CreatePostRequest, PostDetail, PostType } from "@/lib/types";

interface PostComposerProps {
  placeholder: string;
  submitLabel: string;
  type?: PostType;
  parentPostId?: string;
  quotedPostId?: string;
  onSuccess?: () => void;
}

export function PostComposer({
  placeholder,
  submitLabel,
  type = PostType.Post,
  parentPostId,
  quotedPostId,
  onSuccess,
}: PostComposerProps) {
  const [content, setContent] = useState("");
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: async () => {
      const payload: CreatePostRequest = {
        content,
        postType: type,
        parentPostId,
        quotedPostId,
      };

      if (type === PostType.Quote && quotedPostId) {
        return apiFetch<PostDetail>(`/api/posts/${quotedPostId}/quote`, {
          method: "POST",
          body: jsonBody(payload),
        });
      }

      return apiFetch<PostDetail>("/api/posts", {
        method: "POST",
        body: jsonBody(payload),
      });
    },
    onSuccess: () => {
      setContent("");
      queryClient.invalidateQueries({ queryKey: ["timeline"] });
      queryClient.invalidateQueries({ queryKey: ["post-detail"] });
      queryClient.invalidateQueries({ queryKey: ["profile-posts"] });
      queryClient.invalidateQueries({ queryKey: ["replies"] });
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
      toast.success("Publicacion enviada.");
      onSuccess?.();
    },
    onError: (error) => {
      toast.error(mutationErrorMessage(error));
    },
  });

  return (
    <div className="panel-card rounded-[2rem] border bg-card p-5">
      <div className="mb-4 flex items-center justify-between gap-3">
        <p className="panel-label">Compose</p>
        <p className="ui-mono text-[11px] uppercase tracking-[0.14em] text-muted-foreground">
          {submitLabel}
        </p>
      </div>

      <Textarea
        maxLength={280}
        onChange={(event) => setContent(event.target.value)}
        placeholder={placeholder}
        value={content}
      />

      <div className="mt-3 flex items-center justify-between gap-3">
        <p className="ui-mono text-[11px] uppercase tracking-[0.14em] text-muted-foreground">
          {content.length}/280
        </p>
        <Button
          disabled={mutation.isPending || content.trim().length === 0}
          onClick={() => mutation.mutate()}
        >
          {mutation.isPending ? "Enviando..." : submitLabel}
        </Button>
      </div>
    </div>
  );
}
