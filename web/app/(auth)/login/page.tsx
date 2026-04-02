"use client";

import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { FormEvent, Suspense, useState } from "react";
import { toast } from "sonner";
import { AuthLayout } from "@/components/auth/auth-layout";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { apiFetch, jsonBody, mutationErrorMessage } from "@/lib/api/client";
import { SessionUser } from "@/lib/types";
import { useAuthStore } from "@/store/auth-store";

function LoginContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const setSession = useAuthStore((state) => state.setSession);
  const [form, setForm] = useState({
    userName: "",
    password: "",
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!form.userName.trim() || !form.password.trim()) {
      toast.error("Completa usuario y contraseña.");
      return;
    }

    try {
      setIsSubmitting(true);
      const payload = await apiFetch<{ user: SessionUser }>("/api/auth/login", {
        method: "POST",
        body: jsonBody(form),
      });
      setSession(payload.user);
      router.replace(searchParams.get("next") ?? "/home");
      router.refresh();
    } catch (error) {
      toast.error(mutationErrorMessage(error));
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthLayout
      description="Ingresa para ver tu timeline, notificaciones y perfil."
      footer={
        <>
          ¿No tienes cuenta?{" "}
          <Link className="text-foreground underline" href="/register">
            Regístrate
          </Link>
        </>
      }
      title="Bienvenido"
    >
      <form className="space-y-4" onSubmit={handleSubmit}>
        <div>
          <label className="mb-2 block text-sm font-medium">Usuario</label>
          <Input
            onChange={(event) =>
              setForm((current) => ({ ...current, userName: event.target.value }))
            }
            placeholder="tu_usuario"
            value={form.userName}
          />
        </div>
        <div>
          <label className="mb-2 block text-sm font-medium">Contraseña</label>
          <Input
            onChange={(event) =>
              setForm((current) => ({ ...current, password: event.target.value }))
            }
            placeholder="••••••••"
            type="password"
            value={form.password}
          />
        </div>
        <Button className="w-full" disabled={isSubmitting} type="submit">
          {isSubmitting ? "Ingresando..." : "Entrar"}
        </Button>
      </form>
    </AuthLayout>
  );
}

export default function LoginPage() {
  return (
    <Suspense fallback={null}>
      <LoginContent />
    </Suspense>
  );
}
