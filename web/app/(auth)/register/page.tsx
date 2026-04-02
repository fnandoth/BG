"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useState } from "react";
import { toast } from "sonner";
import { AuthLayout } from "@/components/auth/auth-layout";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { apiFetch, jsonBody, mutationErrorMessage } from "@/lib/api/client";
import { SessionUser } from "@/lib/types";
import { useAuthStore } from "@/store/auth-store";

export default function RegisterPage() {
  const router = useRouter();
  const setSession = useAuthStore((state) => state.setSession);
  const [form, setForm] = useState({
    userName: "",
    email: "",
    displayName: "",
    password: "",
    avatarUrl: "",
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (Object.values(form).some((value) => value.trim().length === 0)) {
      toast.error("Todos los campos son obligatorios.");
      return;
    }

    if (!form.email.includes("@")) {
      toast.error("Ingresa un email válido.");
      return;
    }

    if (form.password.length < 6) {
      toast.error("La contraseña debe tener al menos 6 caracteres.");
      return;
    }

    try {
      setIsSubmitting(true);
      const payload = await apiFetch<{ user?: SessionUser; registered: boolean }>("/api/auth/register", {
        method: "POST",
        body: jsonBody(form),
      });

      if (payload.user) {
        setSession(payload.user);
        router.replace("/home");
      } else {
        router.replace("/login");
      }
      router.refresh();
    } catch (error) {
      toast.error(mutationErrorMessage(error));
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthLayout
      description="Crea tu cuenta y empieza a publicar desde el primer minuto."
      footer={
        <>
          ¿Ya tienes cuenta?{" "}
          <Link className="text-foreground underline" href="/login">
            Inicia sesión
          </Link>
        </>
      }
      title="Crear cuenta"
    >
      <form className="space-y-4" onSubmit={handleSubmit}>
        <div>
          <label className="mb-2 block text-sm font-medium">Usuario</label>
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
            placeholder="https://..."
            value={form.avatarUrl}
          />
        </div>
        <div>
          <label className="mb-2 block text-sm font-medium">Contraseña</label>
          <Input
            onChange={(event) =>
              setForm((current) => ({ ...current, password: event.target.value }))
            }
            type="password"
            value={form.password}
          />
        </div>
        <Button className="w-full" disabled={isSubmitting} type="submit">
          {isSubmitting ? "Creando..." : "Crear cuenta"}
        </Button>
      </form>
    </AuthLayout>
  );
}
