import Link from "next/link";

const workflow = [
  {
    number: "01",
    href: "/boutique-application",
    title: "Boutique Application",
    description:
      "The retailer starts a guided application with prefilled demo data.",
    action: "Start the demo",
  },
  {
    number: "02",
    href: "/product-readiness",
    title: "Product Readiness",
    description:
      "The retailer saves catalog details and validates the garment image.",
    action: "Add a garment",
  },
  {
    number: "03",
    href: "/admin-review",
    title: "Admin Review",
    description:
      "An administrator approves the boutique and garment before customer use.",
    action: "Review and approve",
  },
  {
    number: "04",
    href: "/consumer-try-on",
    title: "Consumer Try-On",
    description:
      "A guest shopper consents, uploads a photo, and receives the generated result.",
    action: "Try the garment",
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
            TryOnReady lets an independent boutique offer customers virtual
            try-on as a free courtesy and convenience. The retailer pays for
            the service and gains an enterprise-style experience without an
            enterprise implementation.
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
            Guided judge demo: application → garment → approval → guest
            try-on → result.
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
              Elena Rivera runs a three-person boutique. TryOnReady guides her
              from garment preparation to a reviewed customer experience.
            </p>
            <div className="progress-row" aria-label="Demo workflow">
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
          <p className="eyebrow">The guided workflow</p>
          <h2>From phone photo to customer-ready preview.</h2>
          <p>
            Every screen explains the next action so a retailer, administrator,
            shopper, or judge can complete the journey without guessing.
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
            <h3>Retailers may see</h3>
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
        <h2>One clear journey from garment setup to customer convenience.</h2>
        <p>
          The retailer manages the garment and provider usage. The shopper pays
          nothing and receives a private, convenient virtual try-on result.
        </p>
        <Link className="button button-light" href="/boutique-application/">
          Start the guided demo
        </Link>
      </section>
    </>
  );
}
