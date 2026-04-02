import { NextRequest, NextResponse } from "next/server";
import { getGatewayUrl } from "@/lib/auth";
import { AUTH_COOKIE_NAME } from "@/lib/constants";

function buildUpstreamUrl(service: string, pathSegments: string[], search: string) {
  const suffix =
    pathSegments.length > 0
      ? `/${pathSegments.map((segment) => encodeURIComponent(segment)).join("/")}`
      : "";
  return `${getGatewayUrl()}/api/${service}${suffix}${search}`;
}

function forwardHeaders(request: NextRequest) {
  const headers = new Headers();
  const contentType = request.headers.get("content-type");
  if (contentType) {
    headers.set("content-type", contentType);
  }

  const token = request.cookies.get(AUTH_COOKIE_NAME)?.value;
  if (token) {
    headers.set("authorization", `Bearer ${token}`);
  }

  return headers;
}

export async function proxyRequest(request: NextRequest, service: string, pathSegments: string[] = []) {
  const body =
    request.method === "GET" || request.method === "HEAD" ? undefined : await request.text();

  const upstream = await fetch(buildUpstreamUrl(service, pathSegments, request.nextUrl.search), {
    method: request.method,
    headers: forwardHeaders(request),
    body,
    cache: "no-store",
  });

  const response = new NextResponse(upstream.body, {
    status: upstream.status,
  });

  const contentType = upstream.headers.get("content-type");
  if (contentType) {
    response.headers.set("content-type", contentType);
  }

  return response;
}

export async function proxyJson(request: NextRequest, service: string, pathSegments: string[] = []) {
  const body =
    request.method === "GET" || request.method === "HEAD" ? undefined : await request.text();

  return fetch(buildUpstreamUrl(service, pathSegments, request.nextUrl.search), {
    method: request.method,
    headers: forwardHeaders(request),
    body,
    cache: "no-store",
  });
}
