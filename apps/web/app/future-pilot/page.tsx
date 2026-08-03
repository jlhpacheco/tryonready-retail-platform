import type { Metadata } from "next";
import Link from "next/link";

export const metadata: Metadata = {
  title: "Future Pilot",
  description:
    "TryOnReady's measured boutique pilot plan for reliability, operator effort, controlled provider use, shopper clarity, and privacy.",
};

const pilotPhases = [
  {
    number: "01",
    timing: "Weeks 0–2",
    title: "Prepare and baseline",
    description:
      "Enroll qualified boutiques, assist the first garment, verify rights and readiness, and establish honest operating baselines before setting improvement targets.",
  },
  {
    number: "02",
    timing: "Weeks 3–8",
    title: "Run and learn",
    description:
      "Operate controlled guest journeys, review reliability and provider units weekly, and listen closely to owners, staff, and shoppers.",
  },
  {
    number: "03",
    timing: "Weeks 9–12",
    title: "Decide with evidence",
    description:
      "Compare the cohort against its baseline, document gaps, and make a human go, revise, or stop decision before expanding.",
  },
];

const measures = [
  ["Activation", "Time and support required to reach the first approved garment."],
  ["Reliability", "Completion and latency for valid provider-accepted live jobs."],
  ["Operator effort", "Admin decision time and the learning curve after garment one."],
  ["Usage control", "Units per success and duplicate requests prevented."],
  ["Shopper clarity", "Consent, upload, submit, result, and explicit interest actions."],
  ["Commercial signal", "Repeat retailer use and willingness to continue or pay."],
];

const hardGates = [
  "No provider credential or secret reaches the browser.",
  "Retailers and administrators cannot access shopper photographs.",
  "An unchanged duplicate never creates a second provider task.",
  "Every eligible local source and result meets its deletion deadline.",
  "Appearance visualization is never presented as physical fit or sizing.",
];

export default function FuturePilotPage() {
  return (
    <>
      <section className="pilot-environment" aria-label="Current environment boundary">
        <strong>Current public site: controlled synthetic demonstration only.</strong>
        <span>
          It rejects real customer media, uses a stored result, and makes zero
          live provider requests. Real shopper testing begins only after
          consent, access-control, retention and deletion, provider-handling,
          audit, and incident-response gates pass.
        </span>
      </section>

      <section className="pilot-hero" aria-labelledby="pilot-title">
        <div className="pilot-hero-copy">
          <p className="eyebrow">The work continues beyond the hackathon</p>
          <h1 id="pilot-title">The pilot is next.</h1>
          <p className="pilot-declaration">
            Regardless of the hackathon result, we are moving TryOnReady toward
            a measured boutique pilot—carefully, transparently, and with real
            operators.
          </p>
          <p className="pilot-intro">
            The demonstration proved a controlled journey can be assembled. The
            pilot will test whether independent boutiques can operate it
            reliably, whether shoppers understand it, and whether the economics
            and privacy controls hold up before expansion.
          </p>
          <div className="pilot-actions">
            <a className="button button-primary" href="#pilot-plan">
              See the operating plan
            </a>
            <Link className="button button-secondary" href="/sign-in/">
              Review the working demo
            </Link>
          </div>
        </div>

        <aside className="pilot-commitment" aria-label="Pilot commitment">
          <header>
            <p>Pilot commitment</p>
            <strong>Proposed operating scope</strong>
          </header>
          <dl>
            <div>
              <dt>Duration</dt>
              <dd>8–12 weeks</dd>
            </div>
            <div>
              <dt>Boutique cohort</dt>
              <dd>10–25</dd>
            </div>
            <div>
              <dt>Approved garments</dt>
              <dd>5–20 per boutique</dd>
            </div>
            <div>
              <dt>Baseline period</dt>
              <dd>First 2 weeks</dd>
            </div>
          </dl>
          <footer>
            <span aria-hidden="true">✓</span>
            <p>
              <strong>Human decision gate</strong>
              Expand, revise, or stop based on evidence—not momentum.
            </p>
          </footer>
        </aside>
      </section>

      <section className="pilot-plan" id="pilot-plan" aria-labelledby="pilot-plan-title">
        <div className="pilot-section-heading">
          <p className="eyebrow">A disciplined path forward</p>
          <h2 id="pilot-plan-title">Prepare. Run. Measure. Decide.</h2>
          <p>
            The plan starts deliberately small so reliability, effort, cost,
            and privacy can be examined before the catalog or cohort grows.
          </p>
        </div>
        <ol className="pilot-phases">
          {pilotPhases.map((phase) => (
            <li key={phase.number}>
              <div className="pilot-phase-number">{phase.number}</div>
              <p className="pilot-phase-timing">{phase.timing}</p>
              <h3>{phase.title}</h3>
              <p>{phase.description}</p>
            </li>
          ))}
        </ol>
      </section>

      <section className="pilot-evidence" aria-labelledby="pilot-evidence-title">
        <div className="pilot-evidence-copy">
          <p className="eyebrow">Evidence before scale</p>
          <h2 id="pilot-evidence-title">We will measure the operating truth.</h2>
          <p>
            Starting thresholds will be refined after the two-week baseline.
            Conversion improvement, return reduction, and willingness to pay
            remain hypotheses until the pilot produces credible evidence.
          </p>
        </div>
        <dl className="pilot-measures">
          {measures.map(([term, description]) => (
            <div key={term}>
              <dt>{term}</dt>
              <dd>{description}</dd>
            </div>
          ))}
        </dl>
      </section>

      <section className="pilot-guardrails" aria-labelledby="pilot-guardrails-title">
        <div>
          <p className="eyebrow">Non-negotiable from day one</p>
          <h2 id="pilot-guardrails-title">Growth never outranks the safeguards.</h2>
          <p>
            Privacy, secret handling, duplicate prevention, and deletion
            compliance are hard gates—not metrics we average away. Provider
            retention and deletion are measured separately; local deletion
            never proves provider deletion.
          </p>
        </div>
        <ul>
          {hardGates.map((gate) => (
            <li key={gate}>
              <span aria-hidden="true">✓</span>
              {gate}
            </li>
          ))}
        </ul>
      </section>

      <section className="pilot-cadence" aria-labelledby="pilot-cadence-title">
        <div className="pilot-cadence-lead">
          <p className="eyebrow">Every week</p>
          <h2 id="pilot-cadence-title">Operate visibly. Learn honestly.</h2>
        </div>
        <div className="pilot-cadence-list">
          <p><strong>Reliability review</strong><span>Outcomes, latency, failures, and retry behavior.</span></p>
          <p><strong>Privacy audit</strong><span>Access boundaries, deletion deadlines, and incident review.</span></p>
          <p><strong>Retailer feedback</strong><span>Setup effort, staff confidence, and repeat use.</span></p>
          <p><strong>Metric review</strong><span>Units per success, duplicates stopped, and shopper completion.</span></p>
        </div>
      </section>

      <section className="pilot-closing">
        <p className="eyebrow">Our commitment</p>
        <h2>We are serious about earning the next step.</h2>
        <p>
          A prize would accelerate the work. It does not determine whether the
          work continues. We will use the evidence to improve, expand, or stop
          responsibly—and we will be honest about what the pilot proves.
        </p>
        <div className="pilot-actions">
          <Link className="button button-light" href="/admin-review/">
            Review the controlled workflow
          </Link>
          <Link className="button button-dark-outline" href="/">
            Return to TryOnReady
          </Link>
        </div>
      </section>
    </>
  );
}
