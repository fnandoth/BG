import { NextRequest } from "next/server";
import { proxyRequest } from "@/lib/api/server";

export async function GET(request: NextRequest) {
  return proxyRequest(request, "notifications");
}
