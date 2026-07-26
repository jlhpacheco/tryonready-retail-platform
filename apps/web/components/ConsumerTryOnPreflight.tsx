"use client";

import Link from "next/link";
import { ChangeEvent, FormEvent, useEffect, useMemo, useState } from "react";

type Product = {
  id: string;
  boutiqueName: string;
  name: string;
  sku: string;
  brand: string;
  color: string;
  sizeRange: string;
  description: string;
  garmentImageUrl: string;
};

type ImageDetails = {
  fileName: string;
  mediaType: string;
  byteLength: number;
  pixelWidth: number;
  pixelHeight: number;
};

type TryOnJob = {
  id: string;
  productId: string;
  productName: string;
  status: string;
  message: string;
  isDuplicate: boolean;
  duplicateRequestCount: number;
  apiUnitsReserved: number;
  apiUnitsConsumed: number;
  resultUrl: string | null;
};

type WorkflowStatus = {
  providerMode: string;
  liveYouCamIntegration: boolean;
  apiKeyExposedToBrowser: boolean;
};

async function readImage(file: File): Promise<ImageDetails> {
  const url = URL.createObjectURL(file);

  try {
    const dimensions = await new Promise<{
      pixelWidth: number;
      pixelHeight: number;
    }>((resolve, reject) => {
      const image = new Image();
      image.onload = () =>
        resolve({
          pixelWidth: image.naturalWidth,
          pixelHeight: image.naturalHeight,
        });
      image.onerror = () => reject(new Error("The image could not be read."));
      image.src = url;
    });

    return {
      fileName: file.name,
      mediaType: file.type,
      byteLength: file.size,
      ...dimensions,
    };
  } finally {
    URL.revokeObjectURL(url);
  }
}

function problemMessage(problem: {
  errors?: Record<string, string[]>;
}): string {
  return (
    Object.values(problem.errors ?? {})[0]?.[0] ??
    "The secure try-on request could not be started."
  );
}

export function ConsumerTryOnPreflight() {
  const [products, setProducts] = useState<Product[]>([]);
  const [productId, setProductId] = useState("");
  const [workflowStatus, setWorkflowStatus] =
    useState<WorkflowStatus | null>(null);
  const [imageDetails, setImageDetails] = useState<ImageDetails | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [job, setJob] = useState<TryOnJob | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isBusy, setIsBusy] = useState(false);

  const selectedProduct = useMemo(
    () => products.find((product) => product.id === productId) ?? null,
    [productId, products],
  );

  useEffect(() => {
    let isCurrent = true;

    void Promise.all([
      fetch("/api/products?status=Approved"),
      fetch("/api/status"),
    ])
      .then(async ([productsResponse, statusResponse]) => {
        if (!productsResponse.ok || !statusResponse.ok) {
          throw new Error("The approved garment catalog could not be loaded.");
        }

        const loadedProducts = (await productsResponse.json()) as Product[];
        const loadedStatus = (await statusResponse.json()) as WorkflowStatus;
        if (!isCurrent) {
          return;
        }

        setProducts(loadedProducts);
        setWorkflowStatus(loadedStatus);
        const remembered =
          window.sessionStorage.getItem("tryonready.productId") ?? "";
        setProductId(
          loadedProducts.some((product) => product.id === remembered)
            ? remembered
            : (loadedProducts[0]?.id ?? ""),
        );
      })
      .catch((error: unknown) => {
        if (isCurrent) {
          setMessage(
            error instanceof Error
              ? error.message
              : "The approved garment catalog could not be loaded.",
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

  useEffect(() => {
    if (!job || !["Pending", "Submitting", "Processing"].includes(job.status)) {
      return;
    }

    let isCurrent = true;
    const timer = window.setInterval(() => {
      void fetch(`/api/try-on-jobs/${job.id}`, { cache: "no-store" })
        .then(async (response) => {
          if (!response.ok) {
            throw new Error("The try-on status could not be refreshed.");
          }

          const updated = (await response.json()) as TryOnJob;
          if (isCurrent) {
            setJob(updated);
          }
        })
        .catch((error: unknown) => {
          if (isCurrent) {
            setMessage(
              error instanceof Error
                ? error.message
                : "The try-on status could not be refreshed.",
            );
          }
        });
    }, 750);

    return () => {
      isCurrent = false;
      window.clearInterval(timer);
    };
  }, [job]);

  useEffect(
    () => () => {
      if (previewUrl) {
        URL.revokeObjectURL(previewUrl);
      }
    },
    [previewUrl],
  );

  async function handleImageChange(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];
    setJob(null);
    setMessage(null);
    setImageDetails(null);

    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
      setPreviewUrl(null);
    }

    if (!file) {
      return;
    }

    if (!["image/jpeg", "image/png", "image/webp"].includes(file.type)) {
      setMessage("Choose a JPEG, PNG, or WebP image.");
      event.target.value = "";
      return;
    }

    if (file.size > 10 * 1_024 * 1_024) {
      setMessage("Choose an image no larger than 10 MB.");
      event.target.value = "";
      return;
    }

    setIsBusy(true);
    try {
      setImageDetails(await readImage(file));
      setPreviewUrl(URL.createObjectURL(file));
    } catch (error) {
      setMessage(
        error instanceof Error ? error.message : "The image could not be read.",
      );
      event.target.value = "";
    } finally {
      setIsBusy(false);
    }
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setJob(null);
    setMessage(null);

    if (!productId) {
      setMessage("An administrator must approve a garment first.");
      return;
    }

    if (!imageDetails) {
      setMessage("Choose an authorized person image first.");
      return;
    }

    const form = new FormData(event.currentTarget);
    form.set("productId", productId);
    form.set("consentAccepted", "true");
    setIsBusy(true);

    try {
      const response = await fetch("/api/try-on-jobs", {
        method: "POST",
        body: form,
      });

      if (!response.ok) {
        throw new Error(
          problemMessage(
            (await response.json()) as {
              errors?: Record<string, string[]>;
            },
          ),
        );
      }

      setJob((await response.json()) as TryOnJob);
    } catch (error) {
      setMessage(
        error instanceof Error
          ? error.message
          : "The secure try-on request could not be started.",
      );
    } finally {
      setIsBusy(false);
    }
  }

  return (
    <section className="consumer-shell">
      <div className="consumer-product">
        <p className="eyebrow">Step 4 · Guest customer</p>
        {selectedProduct ? (
          <>
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img
              src={selectedProduct.garmentImageUrl}
              alt={`${selectedProduct.name} approved garment`}
            />
            <h1>{selectedProduct.name}</h1>
            <p>
              {selectedProduct.boutiqueName} · {selectedProduct.sku}
            </p>
            <p>{selectedProduct.description}</p>
          </>
        ) : (
          <div className="readiness-result readiness-needs-work">
            <p className="result-label">Approval required</p>
            <h2>No approved garment is available yet.</h2>
            <Link className="inline-link" href="/admin-review/">
              Go to Admin Review →
            </Link>
          </div>
        )}
      </div>

      <form className="consumer-form" onSubmit={handleSubmit}>
        <div className="form-heading">
          <p className="eyebrow">Free customer convenience</p>
          <h2>See the approved garment on your photo.</h2>
          <p>
            The boutique pays for this service. The guest customer does not
            create an account or pay a fee.
          </p>
        </div>

        <div className="provider-proof" role="status">
          <span>Provider mode: {workflowStatus?.providerMode ?? "Loading"}</span>
          <span>API key in browser: Never</span>
        </div>

        <label className="upload-field">
          <span>Approved garment</span>
          <select
            name="productId"
            value={productId}
            onChange={(event) => {
              setProductId(event.target.value);
              window.sessionStorage.setItem(
                "tryonready.productId",
                event.target.value,
              );
            }}
            required
          >
            <option value="" disabled>
              Choose a garment
            </option>
            {products.map((product) => (
              <option key={product.id} value={product.id}>
                {product.name} · {product.color} · {product.sizeRange}
              </option>
            ))}
          </select>
        </label>

        <label className="upload-field">
          <span>Person image</span>
          <input
            name="personImage"
            type="file"
            accept="image/jpeg,image/png,image/webp"
            onChange={handleImageChange}
            required
          />
          <small>JPEG, PNG, or WebP. Maximum 10 MB.</small>
        </label>

        {previewUrl ? (
          <div className="consumer-preview">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img src={previewUrl} alt="Selected private try-on preview" />
            <p>
              {imageDetails?.pixelWidth} × {imageDetails?.pixelHeight} ·{" "}
              {imageDetails?.fileName}
            </p>
          </div>
        ) : null}

        <div className="privacy-box">
          <h3>Your photo stays private</h3>
          <ul>
            <li>The boutique never receives your source or generated photo.</li>
            <li>No image content is written to application logs.</li>
            <li>Images follow the published automatic deletion schedule.</li>
            <li>Virtual try-on does not guarantee physical fit or sizing.</li>
          </ul>
        </div>

        <label className="consent-field">
          <input name="consent" type="checkbox" required />
          <span>
            I consent to processing this image to generate my virtual try-on
            result.
          </span>
        </label>

        <button
          className="button button-primary form-action"
          type="submit"
          disabled={isLoading || isBusy || products.length === 0}
        >
          {isBusy ? "Starting secure try-on…" : "Generate virtual try-on"}
        </button>

        {message ? (
          <div className="readiness-result readiness-error" role="alert">
            <h3>Try-on could not continue</h3>
            <p>{message}</p>
          </div>
        ) : null}

        {job ? (
          <div
            className={`readiness-result ${
              job.status === "Failed" ? "readiness-error" : "readiness-ready"
            }`}
            role="status"
            aria-live="polite"
          >
            <p className="result-label">Try-on status · {job.status}</p>
            <h3>{job.message}</h3>
            {job.isDuplicate ? (
              <p>
                Duplicate request prevented. No second provider task was
                created.
              </p>
            ) : null}
            {job.status === "Succeeded" && job.resultUrl ? (
              <div className="generated-result">
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img
                  src={`${job.resultUrl}?v=${encodeURIComponent(job.id)}`}
                  alt={`Generated virtual try-on result for ${job.productName}`}
                />
                <p>
                  Result ready · API units used: {job.apiUnitsConsumed}
                </p>
                <Link className="inline-link" href="/admin-review/">
                  View the retailer results dashboard →
                </Link>
              </div>
            ) : null}
          </div>
        ) : null}
      </form>
    </section>
  );
}
