"use client";

import { useEffect, useState } from "react";

type BoutiqueApplication = {
  id: string;
  boutiqueName: string;
  ownerName: string;
  email: string;
  employeeCount: number;
  primarySalesChannel: string;
  status: string;
};

type ProductReview = {
  id: string;
  boutiqueName: string;
  productName: string;
  sku: string;
  category: string;
  status: string;
  readinessPassed: boolean;
  providerStatus: string;
};

export function AdminReviewWorkspace() {
  const [applications, setApplications] = useState<BoutiqueApplication[]>([]);
  const [reviews, setReviews] = useState<ProductReview[]>([]);
  const [notes, setNotes] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    let isCurrent = true;

    void Promise.all([
      fetch("/api/boutique-applications"),
      fetch("/api/admin/reviews"),
    ])
      .then(async ([applicationsResponse, reviewsResponse]) => {
        if (!applicationsResponse.ok || !reviewsResponse.ok) {
          throw new Error("The review queues could not be loaded.");
        }

        const loadedApplications =
          (await applicationsResponse.json()) as BoutiqueApplication[];
        const loadedReviews = (await reviewsResponse.json()) as ProductReview[];

        if (isCurrent) {
          setApplications(loadedApplications);
          setReviews(loadedReviews);
        }
      })
      .catch((error: unknown) => {
        if (isCurrent) {
          setMessage(
            error instanceof Error
              ? error.message
              : "The review queues could not be loaded.",
          );
        }
      })
      .finally(() => {
        if (isCurrent) {
          setIsLoading(false);
        }
      });

    return () => {
      isCurrent = false;
    };
  }, []);

  async function decideApplication(applicationId: string, decision: string) {
    setIsSaving(true);
    setMessage(null);

    try {
      const response = await fetch(
        `/api/boutique-applications/${applicationId}/decision`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ decision }),
        },
      );

      if (!response.ok) {
        throw new Error("The application decision could not be saved.");
      }

      const updated = (await response.json()) as BoutiqueApplication;
      setApplications((current) =>
        current.map((item) => (item.id === updated.id ? updated : item)),
      );
      setMessage(`Boutique application saved: ${updated.status}.`);
    } catch (error) {
      setMessage(
        error instanceof Error ? error.message : "The decision could not be saved.",
      );
    } finally {
      setIsSaving(false);
    }
  }

  async function decideProduct(reviewId: string, decision: string) {
    setIsSaving(true);
    setMessage(null);

    try {
      const response = await fetch(
        `/api/admin/reviews/${reviewId}/decision`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ decision, notes }),
        },
      );

      if (!response.ok) {
        throw new Error(
          decision === "ChangesRequested" || decision === "Decline"
            ? "Add a decision note before requesting changes or declining."
            : "The product decision could not be saved.",
        );
      }

      const updated = (await response.json()) as ProductReview;
      setReviews((current) =>
        current.map((item) => (item.id === updated.id ? updated : item)),
      );
      setMessage(`Product review saved: ${updated.status}.`);
    } catch (error) {
      setMessage(
        error instanceof Error ? error.message : "The decision could not be saved.",
      );
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <section className="admin-shell">
      <header className="admin-heading">
        <p className="eyebrow">Admin review</p>
        <h1>Review the boutique and product before consumer try-on.</h1>
        <p>
          This workspace shows business details, readiness, and provider status.
          It never shows a consumer photograph or secret value.
        </p>
      </header>

      {isLoading ? <p role="status">Loading review queues…</p> : null}
      {message ? (
        <p className="admin-message" role="status">
          {message}
        </p>
      ) : null}

      <div className="admin-grid">
        <section className="admin-panel" aria-labelledby="applications-title">
          <div className="panel-heading">
            <p className="eyebrow">Queue 01</p>
            <h2 id="applications-title">Boutique applications</h2>
          </div>

          {applications.length === 0 ? (
            <p className="empty-state">
              No submitted applications yet. Submit the Boutique Application
              form first.
            </p>
          ) : (
            applications.map((application) => (
              <article className="review-card" key={application.id}>
                <div className="review-card-heading">
                  <div>
                    <h3>{application.boutiqueName}</h3>
                    <p>{application.ownerName}</p>
                  </div>
                  <span className="status-pill">{application.status}</span>
                </div>
                <dl className="review-facts">
                  <div>
                    <dt>Email</dt>
                    <dd>{application.email}</dd>
                  </div>
                  <div>
                    <dt>Employees</dt>
                    <dd>{application.employeeCount}</dd>
                  </div>
                  <div>
                    <dt>Sales channel</dt>
                    <dd>{application.primarySalesChannel}</dd>
                  </div>
                </dl>
                <div className="decision-row">
                  <button
                    type="button"
                    onClick={() => decideApplication(application.id, "Approve")}
                    disabled={isSaving}
                  >
                    Approve boutique
                  </button>
                  <button
                    className="button-danger"
                    type="button"
                    onClick={() => decideApplication(application.id, "Decline")}
                    disabled={isSaving}
                  >
                    Decline boutique
                  </button>
                </div>
              </article>
            ))
          )}
        </section>

        <section className="admin-panel" aria-labelledby="products-title">
          <div className="panel-heading">
            <p className="eyebrow">Queue 02</p>
            <h2 id="products-title">Product reviews</h2>
          </div>

          {reviews.map((review) => (
            <article className="review-card" key={review.id}>
              <div className="review-card-heading">
                <div>
                  <h3>{review.productName}</h3>
                  <p>
                    {review.boutiqueName} · {review.sku}
                  </p>
                </div>
                <span className="status-pill">{review.status}</span>
              </div>
              <dl className="review-facts">
                <div>
                  <dt>Category</dt>
                  <dd>{review.category}</dd>
                </div>
                <div>
                  <dt>Readiness</dt>
                  <dd>{review.readinessPassed ? "Passed" : "Needs work"}</dd>
                </div>
                <div>
                  <dt>YouCam</dt>
                  <dd>{review.providerStatus}</dd>
                </div>
              </dl>
              <label className="notes-field">
                <span>Decision notes</span>
                <textarea
                  value={notes}
                  onChange={(event) => setNotes(event.target.value)}
                  placeholder="Required for changes or decline"
                  rows={3}
                />
              </label>
              <div className="decision-row decision-row-three">
                <button
                  type="button"
                  onClick={() => decideProduct(review.id, "Approve")}
                  disabled={isSaving}
                >
                  Approve product
                </button>
                <button
                  type="button"
                  onClick={() => decideProduct(review.id, "ChangesRequested")}
                  disabled={isSaving}
                >
                  Request changes
                </button>
                <button
                  className="button-danger"
                  type="button"
                  onClick={() => decideProduct(review.id, "Decline")}
                  disabled={isSaving}
                >
                  Decline product
                </button>
              </div>
            </article>
          ))}
        </section>
      </div>
    </section>
  );
}
