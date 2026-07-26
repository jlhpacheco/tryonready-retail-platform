import type { Metadata } from "next";
import { PlaceholderPage } from "@/components/PlaceholderPage";

export const metadata: Metadata = {
  title: "Consumer Try-On",
};

export default function ConsumerTryOnPage() {
  return (
    <PlaceholderPage
      eyebrow="Consumer try-on"
      title="A simple product experience with a strict privacy boundary."
      description="The planned customer page will present an approved garment and explain image handling before a shopper chooses to begin a virtual try-on experience."
      plannedCapabilities={[
        "Approved synthetic product presentation",
        "Plain-language consent and image-handling notice",
        "Server-mediated provider workflow",
        "Aggregated activity with no retailer access to customer photos",
      ]}
    />
  );
}
