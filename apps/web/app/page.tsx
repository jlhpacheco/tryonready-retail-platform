import Link from "next/link";

const workflow = [
  {
    number: "01",
    href: "/boutique-application",
    title: "Boutique Application",
    description:
      "A future mobile-friendly application path for independent shop owners.",
    action: "Open preview",
  },
  {
    number: "02",
    href: "/product-readiness",
    title: "Product Readiness",
    description:
      "Clear garment-image guidance before any provider validation is attempted.",
    action: "Check an image",
  },
  {
    number: "03",
    href: "/admin-review",
    title: "Admin Review",
    description:
      "A deliberate approval checkpoint before a try-on experience is published.",
    action: "Open preview",
  },
  {
    number: "04",
    href: "/consumer-try-on",
    title: "Consumer Try-On",
    description:
      "A planned customer-facing preview that preserves the retailer privacy boundary.",
    action: "Open preview",
  },
];

export default function Home() {
  return (
    <>
      <section className="hero">
        <div className="hero-copy">
          <p className="eyebrow">Built for independent boutiques</p>
          <h1>Virtual try-on, without the enterprise budget.</h1>
          <p className="hero-intro">
            TryOnReady is a mobile-first workflow concept that helps a small
            boutique prepare garments, validate readiness, and publish
            customer-friendly virtual try-on experiences.
          </p>
          <div className="hero-actions">
            <Link className="button button-primary" href="/boutique-application">
              Explore the boutique flow
            </Link>
            <a className="button button-secondary" href="#workflow">
              See how it works
            </a>
          </div>
          <p className="scaffold-note">
            Scaffold preview: provider calls, uploads, and publishing are not
            active yet.
          </p>
        </div>
        <aside className="demo-card" aria-labelledby="demo-title">
          <div className="demo-card-top">
            <span className="status-dot" aria-hidden="true" />
            <span>Fictional demo boutique</span>
          </div>
          <div className="garment-placeholder" aria-hidden="true">
            <span>LT</span>
          </div>
          <div className="demo-card-body">
            <p className="demo-label">Luna &amp; Thread</p>
            <h2 id="demo-title">Meet Elena&apos;s new blazer workflow.</h2>
            <p>
              Elena Rivera runs a three-person boutique. TryOnReady will guide
              her from garment preparation to a reviewed customer experience.
            </p>
            <div className="progress-row" aria-label="Scaffold workflow status">
              <span className="progress-active">Prepare</span>
              <span>Validate</span>
              <span>Review</span>
              <span>Publish</span>
            </div>
          </div>
        </aside>
      </section>

      <section className="trust-strip" aria-label="Product principles">
        <p>
          <strong>Mobile first</strong>
          <span>Designed for the phone already in a boutique owner&apos;s hand.</span>
        </p>
        <p>
          <strong>Privacy bounded</strong>
          <span>Retailers never receive customer source or generated photos.</span>
        </p>
        <p>
          <strong>Human reviewed</strong>
          <span>Publishing remains a deliberate administrative decision.</span>
        </p>
      </section>

      <section className="workflow-section" id="workflow">
        <div className="section-heading">
          <p className="eyebrow">The planned workflow</p>
          <h2>From phone photo to customer-ready preview.</h2>
          <p>
            The first scaffold makes each boundary visible without pretending
            the full integration is complete.
          </p>
        </div>
        <div className="workflow-grid">
          {workflow.map((item) => (
            <Link className="workflow-card" href={item.href} key={item.href}>
              <span className="workflow-number">{item.number}</span>
              <h3>{item.title}</h3>
              <p>{item.description}</p>
              <span className="card-link">
                {item.action} <span aria-hidden="true">→</span>
              </span>
            </Link>
          ))}
        </div>
      </section>

      <section className="privacy-section">
        <div>
          <p className="eyebrow">A clear privacy boundary</p>
          <h2>Useful activity, not private customer imagery.</h2>
        </div>
        <div className="privacy-grid">
          <article>
            <h3>Retailers may eventually see</h3>
            <ul className="check-list">
              <li>Aggregated try-on counts</li>
              <li>Processing success and failure totals</li>
              <li>Product-page visits and outbound clicks</li>
              <li>API-unit consumption</li>
            </ul>
          </article>
          <article className="never-card">
            <h3>Retailers must never see</h3>
            <ul className="never-list">
              <li>Customer source photographs</li>
              <li>Generated customer photographs</li>
              <li>Private customer attributes</li>
              <li>Image-analysis details</li>
            </ul>
          </article>
        </div>
      </section>

      <section className="closing-section">
        <p className="eyebrow">Small-shop technology, intentionally scoped</p>
        <h2>A foundation ready for review—not a claim of finished integration.</h2>
        <p>
          The next phase can connect the server-side Apparel VTO workflow after
          credentials, retention, and provider contracts are reviewed.
        </p>
        <Link className="button button-light" href="/product-readiness">
          Check an image
        </Link>
      </section>
    </>
  );
}
