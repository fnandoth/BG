import { AppShell } from "@/components/app/app-shell";
import { getServerSession } from "@/lib/auth";

export default async function ProtectedLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const session = await getServerSession();

  return <AppShell sessionUser={session.user}>{children}</AppShell>;
}
