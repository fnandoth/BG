import { ReactNode } from "react";

export function EmptyState({
  title,
  description,
  action,
}: {
  title: string;
  description: string;
  action?: ReactNode;
}) {
  return (
    <div className="panel-card rounded-[2rem] border bg-card p-8">
      <p className="panel-label">No Signal</p>
      <h3 className="mt-5 text-xl">{title}</h3>
      <p className="mt-3 max-w-xl text-sm leading-6 text-muted-foreground">{description}</p>
      {action ? <div className="mt-5">{action}</div> : null}
    </div>
  );
}
