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
          <h1>Virtual try-on that a small boutique can actually run.</h1>
          <p className="hero-intro">
            Prepare and approve each garment once. Let shoppers try it
            privately without an account. Control valid provider requests and
            learn which garments attract interest—without seeing customer
            photos.
          </p>
          <div className="hero-actions">
            <Link className="button button-primary" href="/boutique-application">
              Start the Luna &amp; Thread demo
            </Link>
            <a className="button button-secondary" href="#workflow">
              See how it works
            </a>
          </div>
          <p className="scaffold-note">
            Retailer-paid and guest-friendly: application → garment readiness
            → approval → private try-on → aggregate results.
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
          <strong>Ready before paid use</strong>
          <span>Check the garment before a shopper request can use a provider unit.</span>
        </p>
        <p>
          <strong>Private, no-account guests</strong>
          <span>Shoppers consent; retailers never receive their source or generated photos.</span>
        </p>
        <p>
          <strong>Controlled and measurable</strong>
          <span>Stop unchanged duplicates and track completed work and API units.</span>
        </p>
      </section>

      <section className="workflow-section" id="workflow">
        <div className="section-heading">
          <p className="eyebrow">The guided workflow</p>
          <h2>One garment setup. One private guest journey.</h2>
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
              <li>Approved garment catalog details</li>
              <li>Aggregated try-on counts</li>
              <li>Processing success and failure totals</li>
              <li>API units and stopped duplicates</li>
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
        <p className="eyebrow">A lean pilot, not an unproven promise</p>
        <h2>Run one garment safely before scaling the catalog.</h2>
        <p>
          TryOnReady measures reliable results, retailer effort, controlled
          units, garment-interest actions, and deletion compliance. Conversion
          and return improvements remain hypotheses for a real boutique pilot.
        </p>
        <Link className="button button-light" href="/boutique-application/">
          Start the guided demo
        </Link>
      </section>
    </>
  );
}
