import { NextRequest } from "next/server";
import { proxyRequest } from "@/lib/api/server";

export async function GET(
  request: NextRequest,
  context: { params: Promise<{ path: string[] }> },
) {
  const { path } = await context.params;
  return proxyRequest(request, "posts", path);
}

export async function POST(
  request: NextRequest,
  context: { params: Promise<{ path: string[] }> },
) {
  console.log("Received POST request for posts with path:", request);
  const { path } = await context.params;
  return proxyRequest(request, "posts", path);
}

export async function DELETE(
  request: NextRequest,
  context: { params: Promise<{ path: string[] }> },
) {
  const { path } = await context.params;
  return proxyRequest(request, "posts", path);
}
