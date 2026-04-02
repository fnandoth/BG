import { NextResponse } from "next/server";
import { applySessionCookies, getGatewayUrl } from "@/lib/auth";
import { AuthResponse } from "@/lib/types";

export async function POST(request: Request) {
  const body = await request.json();

  const upstream = await fetch(`${getGatewayUrl()}/api/user/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!upstream.ok) {
    const payload = await upstream.text();
    return new NextResponse(payload, {
      status: upstream.status,
      headers: {
        "content-type": upstream.headers.get("content-type") ?? "application/json",
      },
    });
  }

  const auth = (await upstream.json()) as AuthResponse;
  const response = NextResponse.json({
    user: auth.user,
    expiresAt: auth.expiresAt,
  });

  applySessionCookies(response, auth);
  return response;
}
