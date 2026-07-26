import type { Metadata } from "next";
import { ConsumerTryOnPreflight } from "@/components/ConsumerTryOnPreflight";

export const metadata: Metadata = {
  title: "Consumer Try-On",
};

export default function ConsumerTryOnPage() {
  return <ConsumerTryOnPreflight />;
}
