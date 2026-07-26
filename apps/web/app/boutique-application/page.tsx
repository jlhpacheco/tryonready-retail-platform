import type { Metadata } from "next";
import { PlaceholderPage } from "@/components/PlaceholderPage";

export const metadata: Metadata = {
  title: "Boutique Application",
};

export default function BoutiqueApplicationPage() {
  return (
    <PlaceholderPage
      eyebrow="Boutique application"
      title="A welcoming first step for small retailers."
      description="The planned application flow will let an independent boutique owner describe the shop, confirm basic eligibility, and save progress from a phone, tablet, or desktop."
      plannedCapabilities={[
        "Accessible, mobile-friendly application fields",
        "Save-and-return application progress",
        "Clear review status without exposing internal notes",
        "Validated contact and business information",
      ]}
    />
  );
}
