import type { Metadata } from "next";
import { ProductReadinessForm } from "@/components/ProductReadinessForm";

export const metadata: Metadata = {
  title: "Product Readiness",
};

export default function ProductReadinessPage() {
  return (
    <section className="readiness-shell">
      <ProductReadinessForm />
      <aside className="readiness-help">
        <p className="eyebrow">Photo tips</p>
        <h2>Give the garment its best chance.</h2>
        <ol>
          <li>Show the complete garment.</li>
          <li>Use bright, even lighting.</li>
          <li>Keep the background simple.</li>
          <li>Avoid hands, hangers, and clutter covering the garment.</li>
        </ol>
        <p>
          Passing this check does not guarantee a YouCam result. It prevents
          obvious image problems before a shopper starts a live try-on.
        </p>
      </aside>
    </section>
  );
}
