import Image from "next/image";
import Link from "next/link";

const workflow = [
  {
    number: "01",
    href: "/sign-in",
    title: "Boutique Application",
    description:
      "The retailer starts a guided application with prefilled demo data.",
    action: "Choose the retailer role",
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
          <p className="eyebrow">Retail technology for independent boutiques</p>
          <h1>Virtual try-on, built for the shop floor.</h1>
          <p className="hero-intro">
            Prepare and approve each garment once. Let shoppers try it
            privately without an account. Control valid YouCam try-ons and
            learn which garments attract interest—without seeing customer
            photos.
          </p>
          <p className="hero-positioning">
            Others help one shopper choose a look. TryOnReady helps a small
            boutique operate virtual try-on safely for every guest.
          </p>
          <div className="hero-actions">
            <Link className="button button-primary" href="/sign-in">
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
        <aside className="operations-receipt" aria-labelledby="operations-title">
          <header className="operations-header">
            <div>
              <p>Boutique operations proof</p>
              <span>One completed guided demo path</span>
            </div>
            <span className="operations-live">YouCam-ready</span>
          </header>

          <div className="operations-product">
            <Image
              alt="Terracotta Moonlight Blazer"
              height={1254}
              priority
              src="/demo/synthetic-terracotta-blazer.png"
              unoptimized
              width={1254}
            />
            <div>
              <p>Luna &amp; Thread</p>
              <h2 id="operations-title">Moonlight Blazer</h2>
              <span>SYN-BLZ-001 · Terracotta · XS–XL</span>
            </div>
          </div>

          <dl className="operations-checks">
            <div>
              <dt>Image readiness</dt>
              <dd className="check-passed">Passed</dd>
            </div>
            <div>
              <dt>Administrator approval</dt>
              <dd className="check-passed">Approved</dd>
            </div>
            <div>
              <dt>Guest account required</dt>
              <dd>No</dd>
            </div>
            <div>
              <dt>Completed try-ons</dt>
              <dd>1</dd>
            </div>
            <div>
              <dt>Usage controls active</dt>
              <dd>Yes</dd>
            </div>
            <div>
              <dt>Shopper photos in retailer view</dt>
              <dd>0</dd>
            </div>
          </dl>

          <div className="operations-flow" aria-label="TryOnReady operating path">
            <span>Apply</span>
            <span>Prepare</span>
            <span>Approve</span>
            <span>Private try-on</span>
          </div>

          <footer className="operations-footer">
            <span>Secret keys stay hidden</span>
            <strong>Retailer-safe aggregate signals</strong>
          </footer>
        </aside>
      </section>

      <section className="trust-strip" aria-label="Product principles">
        <p>
          <span className="trust-number">01</span>
          <strong>Ready before paid use</strong>
          <span>Check the garment before a shopper can start a live try-on.</span>
        </p>
        <p>
          <span className="trust-number">02</span>
          <strong>Private, no-account guests</strong>
          <span>Shoppers consent; retailers never receive their source or generated photos.</span>
        </p>
        <p>
          <span className="trust-number">03</span>
          <strong>Controlled and measurable</strong>
          <span>Stop unchanged duplicates and track completed try-ons.</span>
        </p>
      </section>

      <section className="people-story" aria-labelledby="people-story-title">
        <div className="people-story-heading">
          <p className="eyebrow">Two people. One useful moment.</p>
          <h2 id="people-story-title">
            Elena runs the shop. Marisol wants to see the blazer.
          </h2>
        </div>
        <div className="people-story-copy">
          <p>
            Elena Rivera has three employees, a physical boutique, and no
            integration team. She prepares the Moonlight Blazer once and
            decides when it is ready for shoppers.
          </p>
          <p>
            Marisol Lopez visits as a guest. She sees the limits, gives
            consent, and receives her private result without opening an account.
          </p>
          <strong>
            Neither person needs to understand an API for the experience to
            work.
          </strong>
        </div>
        <figure className="people-story-team">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img
            src="/demo/luna-and-thread-team.png"
            alt="Three fictional adult members of the Luna and Thread boutique team"
            width="1672"
            height="943"
          />
          <figcaption>
            Synthetic boutique team — Elena Rivera centered, with one fictional
            adult family member and one fictional adult close friend. Guest
            shopper Marisol Lopez is not pictured and has no likeness overlap.
          </figcaption>
        </figure>
      </section>

      <section className="workflow-section" id="workflow">
        <div className="section-heading">
          <p className="eyebrow">The operating collection</p>
          <h2>One garment. Four controlled steps.</h2>
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
              <li>Completed try-ons and stopped duplicates</li>
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
          YouCam use, garment-interest actions, and deletion compliance.
          Conversion and return improvements remain hypotheses for a real
          boutique pilot.
        </p>
        <Link className="button button-light" href="/sign-in/">
          Start the guided demo
        </Link>
      </section>
    </>
  );
}
