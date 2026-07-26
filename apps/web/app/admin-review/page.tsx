import type { Metadata } from "next";
import { PlaceholderPage } from "@/components/PlaceholderPage";

export const metadata: Metadata = {
  title: "Admin Review",
};

export default function AdminReviewPage() {
  return (
    <PlaceholderPage
      eyebrow="Admin review"
      title="A human checkpoint before anything is published."
      description="The planned admin workspace will separate product readiness, provider processing outcomes, and publishing decisions while keeping sensitive customer imagery out of the retailer workflow."
      plannedCapabilities={[
        "Application and product review queues",
        "Provider-processing status without secret values",
        "Approve, request changes, or decline decisions",
        "Auditable events with no image content in logs",
      ]}
    />
  );
}
