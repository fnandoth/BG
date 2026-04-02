import Link from "next/link";
import { ThemeToggle } from "@/components/app/theme-toggle";

export function AuthLayout({
  title,
  description,
  children,
  footer,
}: {
  title: string;
  description: string;
  children: React.ReactNode;
  footer: React.ReactNode;
}) {
  return (
    <div className="flex min-h-screen items-center justify-center px-4 py-10">
      <div className="panel-card w-full max-w-md rounded-[2rem] border bg-card p-8">
        <div className="mb-8 flex items-start justify-between gap-4">
          <div>
            <Link className="panel-label" href="/">
              BG Social
            </Link>
            <h1 className="mt-5 text-3xl">{title}</h1>
            <p className="mt-3 max-w-sm text-sm leading-6 text-muted-foreground">{description}</p>
          </div>
          <div className="w-32">
            <ThemeToggle />
          </div>
        </div>
        {children}
        <div className="mt-6 text-sm leading-6 text-muted-foreground">{footer}</div>
      </div>
    </div>
  );
}
