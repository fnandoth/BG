import { NextRequest } from "next/server";
import { proxyRequest } from "@/lib/api/server";

export async function POST(request: NextRequest) {
  return proxyRequest(request, "posts");
}
