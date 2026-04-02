import { NextRequest, NextResponse } from "next/server";
import { clearSessionCookies } from "@/lib/auth";
import { proxyJson, proxyRequest } from "@/lib/api/server";
import { UserResponse } from "@/lib/types";

export async function GET(request: NextRequest) {
  return proxyRequest(request, "user");
}

export async function PUT(request: NextRequest) {
  const upstream = await proxyJson(request, "user");
  const payload = await upstream.text();

  const response = new NextResponse(payload, {
    status: upstream.status,
  });

  const contentType = upstream.headers.get("content-type");
  if (contentType) {
    response.headers.set("content-type", contentType);
  }

  if (upstream.ok && payload) {
    try {
      const user = JSON.parse(payload) as UserResponse;
      response.cookies.set("bg_user_session", encodeURIComponent(JSON.stringify(user)), {
        httpOnly: true,
        sameSite: "lax",
        secure: process.env.NODE_ENV === "production",
        path: "/",
      });
    } catch {
      // Ignore cookie refresh failures and return upstream payload.
    }
  }

  return response;
}

export async function DELETE(request: NextRequest) {
  const upstream = await proxyRequest(request, "user");
  if (upstream.ok) {
    clearSessionCookies(upstream);
  }

  return upstream;
}
