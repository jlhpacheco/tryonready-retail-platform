"use client";

import Link from "next/link";
import { FormEvent, useState } from "react";

type ApplicationResult = {
  id: string;
  boutiqueName: string;
  status: string;
};

export function BoutiqueApplicationForm() {
  const [result, setResult] = useState<ApplicationResult | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setResult(null);
    setMessage(null);
    setIsSubmitting(true);

    const form = new FormData(event.currentTarget);
    const payload = {
      boutiqueName: form.get("boutiqueName"),
      ownerName: form.get("ownerName"),
      email: form.get("email"),
      employeeCount: Number(form.get("employeeCount")),
      primarySalesChannel: form.get("primarySalesChannel"),
      website: form.get("website") || null,
      certifiesImageRights: form.get("certifiesImageRights") === "on",
    };

    try {
      const response = await fetch("/api/boutique-applications", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        throw new Error("Please check every required field and try again.");
      }

      setResult((await response.json()) as ApplicationResult);
    } catch (error) {
      setMessage(
        error instanceof Error
          ? error.message
          : "The application could not be submitted.",
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="application-shell">
      <form className="application-form" onSubmit={handleSubmit}>
        <div className="form-heading">
          <p className="eyebrow">Boutique application</p>
          <h1>Tell us about your independent shop.</h1>
          <p>
            Submit the basic information needed for the private demonstration.
            No payment details are requested.
          </p>
        </div>

        <div className="form-grid">
          <label>
            <span>Boutique name</span>
            <input
              name="boutiqueName"
              defaultValue="Luna & Thread"
              required
            />
          </label>
          <label>
            <span>Owner name</span>
            <input name="ownerName" defaultValue="Elena Rivera" required />
          </label>
          <label>
            <span>Business email</span>
            <input
              name="email"
              type="email"
              defaultValue="elena@luna-thread.example.invalid"
              required
            />
          </label>
          <label>
            <span>Number of employees</span>
            <input
              name="employeeCount"
              type="number"
              min="1"
              max="250"
              defaultValue="3"
              required
            />
          </label>
          <label>
            <span>Primary sales channel</span>
            <select
              name="primarySalesChannel"
              defaultValue="Physical store"
              required
            >
              <option>Physical store</option>
              <option>Online shop</option>
              <option>Social media</option>
              <option>Markets and pop-ups</option>
            </select>
          </label>
          <label>
            <span>Website (optional)</span>
            <input
              name="website"
              type="url"
              placeholder="https://example.invalid"
            />
          </label>
        </div>

        <label className="consent-field">
          <input name="certifiesImageRights" type="checkbox" required />
          <span>
            I confirm that this boutique will use only photographs it owns or
            has permission to use.
          </span>
        </label>

        <button
          className="button button-primary form-action"
          type="submit"
          disabled={isSubmitting}
        >
          {isSubmitting ? "Submitting…" : "Submit application"}
        </button>

        {message ? (
          <div className="readiness-result readiness-error" role="alert">
            <h2>Application not submitted</h2>
            <p>{message}</p>
          </div>
        ) : null}

        {result ? (
          <div className="readiness-result readiness-ready" role="status">
            <p className="result-label">Application submitted</p>
            <h2>{result.boutiqueName} is ready for administrative review.</h2>
            <p>
              Status: <strong>{result.status}</strong>. Reference:{" "}
              <code>{result.id}</code>
            </p>
            <Link className="inline-link" href="/product-readiness">
              Continue to Product Readiness →
            </Link>
          </div>
        ) : null}
      </form>

      <aside className="application-help">
        <p className="eyebrow">What happens next</p>
        <h2>A small, reviewable first step.</h2>
        <ol>
          <li>Submit the boutique information.</li>
          <li>Prepare an authorized garment image.</li>
          <li>An administrator reviews the application and product.</li>
        </ol>
        <p>
          This local demonstration keeps applications in memory. They reset
          when the API restarts.
        </p>
      </aside>
    </section>
  );
}
