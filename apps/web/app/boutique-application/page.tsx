import type { Metadata } from "next";
import { BoutiqueApplicationForm } from "@/components/BoutiqueApplicationForm";

export const metadata: Metadata = {
  title: "Boutique Application",
};

export default function BoutiqueApplicationPage() {
  return <BoutiqueApplicationForm />;
}
