import { NextResponse } from "next/server";
import { applySessionCookies, getGatewayUrl } from "@/lib/auth";
import { AuthResponse } from "@/lib/types";

export async function POST(request: Request) {
  const body = await request.json();

  const registerResponse = await fetch(`${getGatewayUrl()}/api/user/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!registerResponse.ok) {
    const payload = await registerResponse.text();
    return new NextResponse(payload, {
      status: registerResponse.status,
      headers: {
        "content-type": registerResponse.headers.get("content-type") ?? "application/json",
      },
    });
  }

  const loginResponse = await fetch(`${getGatewayUrl()}/api/user/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      userName: body.userName,
      password: body.password,
    }),
    cache: "no-store",
  });

  if (!loginResponse.ok) {
    return NextResponse.json(
      {
        registered: true,
      },
      { status: 201 },
    );
  }

  const auth = (await loginResponse.json()) as AuthResponse;
  const response = NextResponse.json(
    {
      user: auth.user,
      expiresAt: auth.expiresAt,
      registered: true,
    },
    { status: 201 },
  );

  applySessionCookies(response, auth);
  return response;
}
