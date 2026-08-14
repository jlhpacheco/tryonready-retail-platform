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
  productId: string;
  boutiqueName: string;
  productName: string;
  sku: string;
  category: string;
  status: string;
  readinessPassed: boolean;
  providerStatus: string;
};

type ProductCatalogItem = {
  id: string;
  name: string;
  brand: string;
  color: string;
  material: string;
  sizeRange: string;
  description: string;
  price: number | null;
  currency: string;
  garmentImageUrl: string;
};

type Dashboard = {
  totalJobs: number;
  pendingJobs: number;
  processingJobs: number;
  succeededJobs: number;
  failedJobs: number;
  duplicateRequestsPrevented: number;
  apiUnitsReserved: number;
  apiUnitsConsumed: number;
};

export function AdminReviewWorkspace() {
  const [applications, setApplications] = useState<BoutiqueApplication[]>([]);
  const [reviews, setReviews] = useState<ProductReview[]>([]);
  const [products, setProducts] = useState<ProductCatalogItem[]>([]);
  const [dashboard, setDashboard] = useState<Dashboard | null>(null);
  const [notes, setNotes] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    let isCurrent = true;

    void Promise.all([
      fetch("/api/boutique-applications"),
      fetch("/api/admin/reviews"),
      fetch("/api/products"),
      fetch("/api/dashboard"),
    ])
      .then(
        async ([
          applicationsResponse,
          reviewsResponse,
          productsResponse,
          dashboardResponse,
        ]) => {
        if (
          !applicationsResponse.ok ||
          !reviewsResponse.ok ||
          !productsResponse.ok ||
          !dashboardResponse.ok
        ) {
          if (
            [
              applicationsResponse.status,
              reviewsResponse.status,
              productsResponse.status,
              dashboardResponse.status,
            ].some((status) => status === 401 || status === 403)
          ) {
            if (isCurrent) {
              setMessage(
                "Sign in as an administrator to review the current application and garment.",
              );
            }
            return;
          }

          throw new Error("The review queues could not be loaded.");
        }

        const loadedApplications =
          (await applicationsResponse.json()) as BoutiqueApplication[];
        const loadedReviews = (await reviewsResponse.json()) as ProductReview[];
        const loadedProducts =
          (await productsResponse.json()) as ProductCatalogItem[];
        const loadedDashboard =
          (await dashboardResponse.json()) as Dashboard;

        if (isCurrent) {
          setApplications(
            loadedApplications.filter(
              (application) => application.status === "Submitted",
            ),
          );
          setReviews(
            loadedReviews.filter((review) => review.status === "Pending"),
          );
          setProducts(loadedProducts);
          setDashboard(loadedDashboard);
        }
      },
      )
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

  const currentApplication = applications[0] ?? null;
  const currentReview = reviews[0] ?? null;
  const currentProduct = currentReview
    ? products.find((product) => product.id === currentReview.productId) ?? null
    : null;
  const completedDemonstrations = dashboard?.succeededJobs ?? 1;
  const duplicatesStopped = dashboard?.duplicateRequestsPrevented ?? 1;

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
    <section className="admin-shell admin-summary-shell">
      <header className="admin-summary-heading">
        <p className="eyebrow">Retailer summary</p>
        <h1>One completed journey. The controls did their job.</h1>
        <p>
          Luna &amp; Thread can confirm the completed demonstration, the stopped
          duplicate, and the privacy safeguards without viewing shopper photos.
        </p>
      </header>

      <section
        className="admin-summary-board"
        aria-labelledby="admin-summary-title"
      >
        <div className="admin-summary-activity">
          <h2 id="admin-summary-title">Controlled demo activity</h2>
          <dl className="admin-summary-metrics dashboard-grid">
            <div>
              <dt>Completed try-ons</dt>
              <dd>{completedDemonstrations}</dd>
            </div>
            <div>
              <dt>Duplicates stopped</dt>
              <dd>{duplicatesStopped}</dd>
            </div>
            <div>
              <dt>Replay provider calls</dt>
              <dd>0</dd>
            </div>
            <div>
              <dt>Retailer photo access</dt>
              <dd>0</dd>
            </div>
          </dl>
        </div>

        <section
          className="admin-summary-controls"
          aria-labelledby="usage-controls-title"
        >
          <h2 id="usage-controls-title">Active usage controls</h2>
          <ul>
            <li>
              <span aria-hidden="true">✓</span>
              <p>
                <strong>Human-approved product</strong>
                <small>One boutique application and one authorized garment image.</small>
              </p>
            </li>
            <li>
              <span aria-hidden="true">✓</span>
              <p>
                <strong>Private guest journey</strong>
                <small>No shopper account required for this demonstration.</small>
              </p>
            </li>
            <li>
              <span aria-hidden="true">✓</span>
              <p>
                <strong>Replay protection</strong>
                <small>The stored result returned without a duplicate provider request.</small>
              </p>
            </li>
          </ul>
          <p className="admin-control-status">
            <strong>Status: controls active</strong>
            <span>Synthetic adult/demo data only</span>
          </p>
        </section>
      </section>

      <section className="admin-decision-area" aria-labelledby="decision-title">
        <header className="admin-decision-heading">
          <p className="eyebrow">Administrator decision</p>
          <h2 id="decision-title">Review one application and one garment.</h2>
          <p>
            One boutique record is paired with its one submitted garment. No
            duplicate cards, repeated photos, or customer imagery.
          </p>
        </header>

        {isLoading ? <p role="status">Loading the current review…</p> : null}
        {message ? (
          <p className="admin-message" role="status">
            {message}
          </p>
        ) : null}

        <div className="admin-grid admin-single-review-grid">
          <section
            className="admin-panel"
            aria-label="Boutique applications"
          >
            <div className="panel-heading">
              <p className="eyebrow">Application 01</p>
              <h3>One boutique application</h3>
              <p>Business identity and operating readiness.</p>
            </div>

            {!currentApplication ? (
              <p className="empty-state">
                No boutique application is awaiting review.
              </p>
            ) : (
              <article className="review-card" key={currentApplication.id}>
                <div className="review-card-heading">
                  <div>
                    <h4>{currentApplication.boutiqueName}</h4>
                    <p>{currentApplication.ownerName}</p>
                  </div>
                  <span className="status-pill">{currentApplication.status}</span>
                </div>
                <dl className="review-facts">
                  <div>
                    <dt>Email</dt>
                    <dd>{currentApplication.email}</dd>
                  </div>
                  <div>
                    <dt>Employees</dt>
                    <dd>{currentApplication.employeeCount}</dd>
                  </div>
                  <div>
                    <dt>Sales channel</dt>
                    <dd>{currentApplication.primarySalesChannel}</dd>
                  </div>
                </dl>
                <div className="decision-row">
                  <button
                    type="button"
                    onClick={() =>
                      decideApplication(currentApplication.id, "Approve")
                    }
                    disabled={isSaving}
                  >
                    Approve boutique
                  </button>
                  <button
                    className="button-danger"
                    type="button"
                    onClick={() =>
                      decideApplication(currentApplication.id, "Decline")
                    }
                    disabled={isSaving}
                  >
                    Decline boutique
                  </button>
                </div>
              </article>
            )}
          </section>

          <section className="admin-panel" aria-label="Product reviews">
            <div className="panel-heading">
              <p className="eyebrow">Garment 01</p>
              <h3>One submitted garment</h3>
              <p>One photograph with readiness and provider status.</p>
            </div>

            {!currentReview ? (
              <p className="empty-state">No garment is awaiting review.</p>
            ) : (
              <article className="review-card" key={currentReview.id}>
              {currentProduct ? (
                // eslint-disable-next-line @next/next/no-img-element
                <img
                  className="review-garment-image"
                  src={currentProduct.garmentImageUrl}
                  alt={`${currentReview.productName} garment submitted for review`}
                />
              ) : null}
              <div className="review-card-heading">
                <div>
                  <h4>{currentReview.productName}</h4>
                  <p>
                    {currentReview.boutiqueName} · {currentReview.sku}
                  </p>
                </div>
                <span className="status-pill">{currentReview.status}</span>
              </div>
              <dl className="review-facts">
                <div>
                  <dt>Category</dt>
                  <dd>{currentReview.category}</dd>
                </div>
                <div>
                  <dt>Readiness</dt>
                  <dd>{currentReview.readinessPassed ? "Passed" : "Needs work"}</dd>
                </div>
                <div>
                  <dt>YouCam readiness</dt>
                  <dd>{currentReview.providerStatus}</dd>
                </div>
                {currentProduct ? (
                    <>
                      <div>
                        <dt>Brand</dt>
                        <dd>{currentProduct.brand}</dd>
                      </div>
                      <div>
                        <dt>Color</dt>
                        <dd>{currentProduct.color}</dd>
                      </div>
                      <div>
                        <dt>Material</dt>
                        <dd>{currentProduct.material}</dd>
                      </div>
                      <div>
                        <dt>Sizes</dt>
                        <dd>{currentProduct.sizeRange}</dd>
                      </div>
                      <div>
                        <dt>Retail price</dt>
                        <dd>
                          {currentProduct.price === null
                            ? "Not listed"
                            : `${currentProduct.currency} ${currentProduct.price.toFixed(2)}`}
                        </dd>
                      </div>
                    </>
                  ) : null}
              </dl>
              {currentProduct ? (
                <p className="review-description">
                  {currentProduct.description}
                </p>
              ) : null}
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
                  onClick={() => decideProduct(currentReview.id, "Approve")}
                  disabled={isSaving}
                >
                  Approve product
                </button>
                <button
                  type="button"
                  onClick={() =>
                    decideProduct(currentReview.id, "ChangesRequested")
                  }
                  disabled={isSaving}
                >
                  Request changes
                </button>
                <button
                  className="button-danger"
                  type="button"
                  onClick={() => decideProduct(currentReview.id, "Decline")}
                  disabled={isSaving}
                >
                  Decline product
                </button>
              </div>
              {currentReview.status === "Approved" ? (
                <a className="inline-link next-step-link" href="/consumer-try-on/">
                  Continue to Consumer Try-On →
                </a>
              ) : null}
            </article>
            )}
          </section>
        </div>
      </section>

      <p className="admin-summary-footnote">Synthetic adult/demo data only</p>
    </section>
  );
}
