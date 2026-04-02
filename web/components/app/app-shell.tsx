"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { toast } from "sonner";
import { AuthHydrator } from "@/components/auth-hydrator";
import { ThemeToggle } from "@/components/app/theme-toggle";
import { Avatar } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { useNotificationsPolling } from "@/hooks/use-notifications-polling";
import { SessionUser } from "@/lib/types";
import { cn } from "@/lib/utils";
import { useAuthStore } from "@/store/auth-store";

const navItems = [
  { href: "/home", label: "Inicio" },
  { href: "/notifications", label: "Notificaciones" },
  { href: "/settings", label: "Settings" },
];

export function AppShell({
  children,
  sessionUser,
}: {
  children: React.ReactNode;
  sessionUser: SessionUser | null;
}) {
  const router = useRouter();
  const pathname = usePathname();
  const user = useAuthStore((state) => state.user) ?? sessionUser;
  const unreadCount = useAuthStore((state) => state.unreadCount);

  useNotificationsPolling(Boolean(user));

  async function handleLogout() {
    const response = await fetch("/api/auth/logout", {
      method: "POST",
    });

    if (!response.ok) {
      toast.error("No se pudo cerrar la sesion.");
      return;
    }

    router.replace("/login");
    router.refresh();
  }

  if (!user) {
    return (
      <>
        <AuthHydrator user={null} />
        <div className="min-h-screen bg-background">
          <header className="border-b border-border bg-background/95">
            <div className="mx-auto flex max-w-5xl items-center justify-between px-6 py-4">
              <Link className="panel-label" href="/">
                BG Social
              </Link>
              <div className="flex items-center gap-3">
                <ThemeToggle />
                <Link
                  className="ui-mono inline-flex h-11 items-center rounded-full bg-foreground px-5 text-[11px] uppercase tracking-[0.14em] text-background"
                  href="/login"
                >
                  Ingresar
                </Link>
              </div>
            </div>
          </header>
          <main className="mx-auto max-w-5xl px-4 py-8 md:px-6">{children}</main>
        </div>
      </>
    );
  }

  return (
    <>
      <AuthHydrator user={user} />
      <div className="mx-auto min-h-screen max-w-[1440px] px-3 py-4 md:px-6">
        <div className="grid gap-4 lg:grid-cols-[280px_minmax(0,1fr)_300px]">
          <aside className="hidden lg:block">
            <div className="panel-card sticky top-4 space-y-5 rounded-[2rem] border bg-card p-6">
              <p className="panel-label">Navigation</p>
              <Link className="text-2xl tracking-[-0.03em]" href="/home">
                BG Social
              </Link>
              <nav className="space-y-2">
                {navItems.map((item) => (
                  <Link
                    key={item.href}
                    className={cn(
                      "ui-mono flex items-center justify-between rounded-[1.25rem] border border-transparent px-4 py-3 text-[11px] uppercase tracking-[0.14em] text-muted-foreground transition hover:border-border hover:bg-muted/35 hover:text-foreground",
                      pathname === item.href && "border-border bg-muted/55 text-foreground",
                    )}
                    href={item.href}
                  >
                    <span>{item.label}</span>
                    {item.href === "/notifications" && unreadCount > 0 ? (
                      <span className="rounded-full border border-accent px-2 py-1 text-[10px] text-accent">
                        {unreadCount}
                      </span>
                    ) : null}
                  </Link>
                ))}
                <Link
                  className={cn(
                    "ui-mono flex items-center rounded-[1.25rem] border border-transparent px-4 py-3 text-[11px] uppercase tracking-[0.14em] text-muted-foreground transition hover:border-border hover:bg-muted/35 hover:text-foreground",
                    pathname === `/${user.displayName}` && "border-border bg-muted/55 text-foreground",
                  )}
                  href={`/${user.displayName}`}
                >
                  Perfil
                </Link>
              </nav>
              <ThemeToggle />
              <div className="rounded-[1.5rem] border border-border bg-background/70 p-4">
                <p className="panel-label">Session</p>
                <div className="mt-3 flex items-center gap-3">
                  <Avatar name={user.displayName} src={user.avatarUrl} />
                  <div className="min-w-0">
                    <p className="truncate text-[15px] font-medium">{user.displayName}</p>
                    <p className="ui-mono truncate text-[11px] uppercase tracking-[0.12em] text-muted-foreground">
                      {user.userName ? `@${user.userName}` : "Sesion activa"}
                    </p>
                  </div>
                </div>
                <Button className="mt-4 w-full" onClick={handleLogout} variant="secondary">
                  Cerrar sesion
                </Button>
              </div>
            </div>
          </aside>

          <main className="min-w-0">{children}</main>

          <aside className="hidden lg:block">
            <div className="panel-card sticky top-4 rounded-[2rem] border bg-card p-6">
              <p className="panel-label">Discovery</p>
              <h2 className="mt-5 text-xl">Sugerencias</h2>
              <p className="mt-3 text-sm leading-6 text-muted-foreground">
                Espacio reservado para descubrimiento de usuarios y tendencias.
              </p>
            </div>
          </aside>
        </div>
      </div>
    </>
  );
}
