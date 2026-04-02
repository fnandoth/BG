import { getErrorMessage, parseResponse } from "@/lib/utils";

async function parseError(response: Response) {
  try {
    const payload = (await response.json()) as { message?: string; title?: string };
    return payload.message ?? payload.title ?? `Request failed with status ${response.status}`;
  } catch {
    return `Request failed with status ${response.status}`;
  }
}

export async function apiFetch<T>(input: string, init?: RequestInit) {
  const response = await fetch(input, {
    ...init,
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      ...(init?.headers ?? {}),
    },
  });

  if (!response.ok) {
    throw new Error(await parseError(response));
  }

  return parseResponse<T>(response);
}

export function jsonBody(value: unknown) {
  return JSON.stringify(value);
}

export function mutationErrorMessage(error: unknown) {
  return getErrorMessage(error);
}
