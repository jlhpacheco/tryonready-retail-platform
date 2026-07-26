import type { Metadata } from "next";
import { AdminReviewWorkspace } from "@/components/AdminReviewWorkspace";

export const metadata: Metadata = {
  title: "Admin Review",
};

export default function AdminReviewPage() {
  return <AdminReviewWorkspace />;
}
