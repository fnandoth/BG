import { formatDistanceToNow } from "date-fns";
import { es } from "date-fns/locale";

export function cn(...classes: Array<string | false | null | undefined>) {
  return classes.filter(Boolean).join(" ");
}

export function getInitials(value: string) {
  return value
    .split(" ")
    .map((part) => part[0]?.toUpperCase() ?? "")
    .join("")
    .slice(0, 2);
}

export function formatRelativeDate(value: string) {
  return formatDistanceToNow(new Date(value), {
    addSuffix: true,
    locale: es,
  });
}

export async function parseResponse<T>(response: Response): Promise<T> {
  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export function getErrorMessage(error: unknown) {
  if (error instanceof Error) {
    return error.message;
  }

  return "Ocurrió un error inesperado.";
}
