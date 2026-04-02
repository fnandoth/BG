import { NextResponse } from "next/server";
import { getServerSession } from "@/lib/auth";

export async function GET() {
  return NextResponse.json(await getServerSession());
}
