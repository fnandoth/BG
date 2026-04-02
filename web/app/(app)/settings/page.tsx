"use client";

import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { FormEvent, useEffect, useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/ui/empty-state";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { apiFetch, jsonBody, mutationErrorMessage } from "@/lib/api/client";
import { UserResponse } from "@/lib/types";
import { useAuthStore } from "@/store/auth-store";

export default function SettingsPage() {
  const router = useRouter();
  const user = useAuthStore((state) => state.user);
  const setSession = useAuthStore((state) => state.setSession);
  const clearSession = useAuthStore((state) => state.clearSession);
  const [deletePassword, setDeletePassword] = useState("");
  const [form, setForm] = useState({
    userName: "",
    email: "",
    password: "",
    displayName: "",
    bio: "",
    avatarUrl: "",
    isPrivate: false,
  });

  useEffect(() => {
    if (!user) {
      return;
    }

    setForm((current) => ({
      ...current,
      userName: user.userName ?? "",
      email: user.email ?? "",
      displayName: user.displayName,
      bio: user.bio ?? "",
      avatarUrl: user.avatarUrl,
      isPrivate: user.isPrivate ?? false,
    }));
  }, [user]);

  const updateMutation = useMutation({
    mutationFn: () =>
      apiFetch<UserResponse>("/api/user", {
        method: "PUT",
        body: jsonBody({
          ...form,
          updatedAt: new Date().toISOString(),
        }),
      }),
    onSuccess: (updatedUser) => {
      setSession(updatedUser);
      toast.success("Perfil actualizado.");
      router.refresh();
    },
    onError: (error) => {
      toast.error(mutationErrorMessage(error));
    },
  });

  const deleteMutation = useMutation({
    mutationFn: () =>
      apiFetch<void>("/api/user", {
        method: "DELETE",
        body: jsonBody({
          userName: form.userName,
          password: deletePassword,
        }),
      }),
    onSuccess: () => {
      clearSession();
      router.replace("/login");
      router.refresh();
    },
    onError: (error) => {
      toast.error(mutationErrorMessage(error));
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!form.password.trim()) {
      toast.error("Debes indicar la contraseña para guardar cambios.");
      return;
    }

    updateMutation.mutate();
  }

  if (!user) {
    return (
      <EmptyState
        description="Necesitas una sesión activa para editar tu perfil."
        title="Sesión no disponible"
      />
    );
  }

  return (
    <div className="space-y-4">
      <section className="rounded-[28px] border bg-card p-5">
        <h1 className="text-xl font-semibold">Settings</h1>
        <p className="mt-2 text-sm text-muted-foreground">
          Edición de perfil respaldada por el endpoint `PUT /api/user`.
        </p>
      </section>

      <form className="space-y-4 rounded-[28px] border bg-card p-5" onSubmit={handleSubmit}>
        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <label className="mb-2 block text-sm font-medium">User name</label>
            <Input
              onChange={(event) =>
                setForm((current) => ({ ...current, userName: event.target.value }))
              }
              value={form.userName}
            />
          </div>
          <div>
            <label className="mb-2 block text-sm font-medium">Email</label>
            <Input
              onChange={(event) =>
                setForm((current) => ({ ...current, email: event.target.value }))
              }
              type="email"
              value={form.email}
            />
          </div>
          <div>
            <label className="mb-2 block text-sm font-medium">Display name</label>
            <Input
              onChange={(event) =>
                setForm((current) => ({ ...current, displayName: event.target.value }))
              }
              value={form.displayName}
            />
          </div>
          <div>
            <label className="mb-2 block text-sm font-medium">Avatar URL</label>
            <Input
              onChange={(event) =>
                setForm((current) => ({ ...current, avatarUrl: event.target.value }))
              }
              value={form.avatarUrl}
            />
          </div>
        </div>

        <div>
          <label className="mb-2 block text-sm font-medium">Bio</label>
          <Textarea
            className="min-h-32"
            onChange={(event) =>
              setForm((current) => ({ ...current, bio: event.target.value }))
            }
            value={form.bio}
          />
        </div>

        <div className="grid gap-4 md:grid-cols-[1fr_auto] md:items-end">
          <div>
            <label className="mb-2 block text-sm font-medium">Contraseña</label>
            <Input
              onChange={(event) =>
                setForm((current) => ({ ...current, password: event.target.value }))
              }
              placeholder="Requerida para guardar"
              type="password"
              value={form.password}
            />
          </div>
          <label className="flex h-11 items-center gap-2 rounded-2xl border px-4 text-sm">
            <input
              checked={form.isPrivate}
              onChange={(event) =>
                setForm((current) => ({ ...current, isPrivate: event.target.checked }))
              }
              type="checkbox"
            />
            Cuenta privada
          </label>
        </div>

        <Button disabled={updateMutation.isPending} type="submit">
          {updateMutation.isPending ? "Guardando..." : "Guardar cambios"}
        </Button>
      </form>

      <section className="rounded-[28px] border border-red-500/40 bg-card p-5">
        <h2 className="text-lg font-semibold">Eliminar cuenta</h2>
        <p className="mt-2 text-sm text-muted-foreground">
          Esta acción llama a `DELETE /api/user` y limpia la cookie de sesión.
        </p>
        <div className="mt-4 flex flex-col gap-3 md:flex-row">
          <Input
            onChange={(event) => setDeletePassword(event.target.value)}
            placeholder="Confirma tu contraseña"
            type="password"
            value={deletePassword}
          />
          <Button
            disabled={deleteMutation.isPending || deletePassword.length === 0}
            onClick={() => {
              if (window.confirm("¿Seguro que quieres eliminar la cuenta?")) {
                deleteMutation.mutate();
              }
            }}
            variant="danger"
          >
            Eliminar cuenta
          </Button>
        </div>
      </section>
    </div>
  );
}
