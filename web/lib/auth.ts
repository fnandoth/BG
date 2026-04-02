import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import { DEFAULT_GATEWAY_URL, AUTH_COOKIE_NAME, USER_COOKIE_NAME } from "@/lib/constants";
import { AuthResponse, SessionResponse, SessionUser } from "@/lib/types";

function encodeCookieValue(value: unknown) {
  return encodeURIComponent(JSON.stringify(value));
}

function decodeCookieValue<T>(value?: string): T | null {
  if (!value) {
    return null;
  }

  try {
    return JSON.parse(decodeURIComponent(value)) as T;
  } catch {
    return null;
  }
}

export function getGatewayUrl() {
  return process.env.GATEWAY_URL ?? DEFAULT_GATEWAY_URL;
}

export async function getAuthToken() {
  const store = await cookies();
  return store.get(AUTH_COOKIE_NAME)?.value ?? null;
}

export async function getStoredSessionUser() {
  const store = await cookies();
  return decodeCookieValue<SessionUser>(store.get(USER_COOKIE_NAME)?.value);
}

export function applySessionCookies(response: NextResponse, auth: AuthResponse) {
  const isProduction = process.env.NODE_ENV === "production";
  const maxAge = Math.max(
    60,
    Math.floor((new Date(auth.expiresAt).getTime() - Date.now()) / 1000),
  );

  response.cookies.set(AUTH_COOKIE_NAME, auth.accessToken, {
    httpOnly: true,
    sameSite: "lax",
    secure: isProduction,
    path: "/",
    maxAge,
  });

  response.cookies.set(USER_COOKIE_NAME, encodeCookieValue(auth.user), {
    httpOnly: true,
    sameSite: "lax",
    secure: isProduction,
    path: "/",
    maxAge,
  });
}

export function clearSessionCookies(response: NextResponse) {
  response.cookies.set(AUTH_COOKIE_NAME, "", {
    httpOnly: true,
    sameSite: "lax",
    secure: process.env.NODE_ENV === "production",
    path: "/",
    maxAge: 0,
  });

  response.cookies.set(USER_COOKIE_NAME, "", {
    httpOnly: true,
    sameSite: "lax",
    secure: process.env.NODE_ENV === "production",
    path: "/",
    maxAge: 0,
  });
}

export async function getServerSession(): Promise<SessionResponse> {
  const token = await getAuthToken();
  if (!token) {
    return { authenticated: false, user: null, expiresAt: null };
  }

  const storedUser = await getStoredSessionUser();
  if (storedUser) {
    return { authenticated: true, user: storedUser };
  }

  try {
    const response = await fetch(`${getGatewayUrl()}/api/user/me`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    });

    if (!response.ok) {
      return { authenticated: false, user: null, expiresAt: null };
    }

    const payload = (await response.json()) as SessionUser;
    return {
      authenticated: true,
      user: {
        ...payload,
        displayName: payload.displayName,
        avatarUrl: payload.avatarUrl ?? "",
      },
    };
  } catch {
    return { authenticated: false, user: null, expiresAt: null };
  }
}
