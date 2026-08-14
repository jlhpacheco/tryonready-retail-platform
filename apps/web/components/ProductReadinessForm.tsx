"use client";

import Link from "next/link";
import { ChangeEvent, FormEvent, useEffect, useState } from "react";

type BoutiqueApplication = {
  id: string;
  boutiqueName: string;
  status: string;
};

type ImageDetails = {
  fileName: string;
  mediaType: string;
  byteLength: number;
  pixelWidth: number;
  pixelHeight: number;
};

type ProductResult = {
  id: string;
  name: string;
  sku: string;
  status: string;
  readinessPassed: boolean;
};

const supportedMediaTypes = new Set(["image/jpeg", "image/png", "image/webp"]);

async function readImageDetails(file: File): Promise<ImageDetails> {
  const imageUrl = URL.createObjectURL(file);

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
      image.onerror = () =>
        reject(new Error("The selected image could not be read."));
      image.src = imageUrl;
    });

    return {
      fileName: file.name,
      mediaType: file.type,
      byteLength: file.size,
      ...dimensions,
    };
  } finally {
    URL.revokeObjectURL(imageUrl);
  }
}

function formatBytes(bytes: number): string {
  if (bytes < 1_024) {
    return `${bytes} bytes`;
  }

  if (bytes < 1_024 * 1_024) {
    return `${(bytes / 1_024).toFixed(1)} KB`;
  }

  return `${(bytes / (1_024 * 1_024)).toFixed(1)} MB`;
}

function getProblemMessage(problem: {
  errors?: Record<string, string[]>;
}): string {
  return (
    Object.values(problem.errors ?? {})[0]?.[0] ??
    "The garment could not be saved. Check the required fields and image."
  );
}

export function ProductReadinessForm() {
  const [applications, setApplications] = useState<BoutiqueApplication[]>([]);
  const [applicationId, setApplicationId] = useState("");
  const [imageDetails, setImageDetails] = useState<ImageDetails | null>(null);
  const [result, setResult] = useState<ProductResult | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isReading, setIsReading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    let isCurrent = true;

    void fetch("/api/boutique-applications")
      .then(async (response) => {
        if (!response.ok) {
          throw new Error("Boutique applications could not be loaded.");
        }

        const loaded = (await response.json()) as BoutiqueApplication[];
        if (!isCurrent) {
          return;
        }

        setApplications(loaded);
        const remembered =
          window.sessionStorage.getItem("tryonready.applicationId") ?? "";
        setApplicationId(
          loaded.some((item) => item.id === remembered)
            ? remembered
            : (loaded[0]?.id ?? ""),
        );
      })
      .catch((error: unknown) => {
        if (isCurrent) {
          setMessage(
            error instanceof Error
              ? error.message
              : "Boutique applications could not be loaded.",
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

  async function handleFileChange(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];
    setResult(null);
    setMessage(null);
    setImageDetails(null);

    if (!file) {
      return;
    }

    if (!supportedMediaTypes.has(file.type)) {
      setMessage("Choose a JPEG, PNG, or WebP garment image.");
      event.target.value = "";
      return;
    }

    if (file.size > 10 * 1_024 * 1_024) {
      setMessage("Choose a garment image no larger than 10 MB.");
      event.target.value = "";
      return;
    }

    setIsReading(true);
    try {
      setImageDetails(await readImageDetails(file));
    } catch (error) {
      setMessage(
        error instanceof Error
          ? error.message
          : "The selected image could not be read.",
      );
      event.target.value = "";
    } finally {
      setIsReading(false);
    }
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setResult(null);
    setMessage(null);

    if (!applicationId) {
      setMessage("Submit a boutique application before adding a garment.");
      return;
    }

    if (!imageDetails) {
      setMessage("Choose a garment image before saving the product.");
      return;
    }

    const form = new FormData(event.currentTarget);
    form.set("boutiqueApplicationId", applicationId);
    setIsSubmitting(true);

    try {
      const response = await fetch("/api/products", {
        method: "POST",
        headers: { "X-TryOnReady-Request": "judge-demo" },
        body: form,
      });

      if (!response.ok) {
        throw new Error(
          getProblemMessage(
            (await response.json()) as {
              errors?: Record<string, string[]>;
            },
          ),
        );
      }

      const saved = (await response.json()) as ProductResult;
      setResult(saved);
      window.sessionStorage.setItem("tryonready.productId", saved.id);
    } catch (error) {
      setMessage(
        error instanceof Error
          ? error.message
          : "The garment could not be saved.",
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form className="readiness-form" onSubmit={handleSubmit}>
      <div className="form-heading">
        <p className="eyebrow">Step 2 · Add a garment</p>
        <h1>Save the product and check its image.</h1>
        <p>
          TryOnReady checks and stores the garment image before an
          administrator reviews it. That keeps unsuitable files out of the
          shopper experience.
        </p>
      </div>

      {isLoading ? <p role="status">Loading boutique applications…</p> : null}

      {applications.length === 0 && !isLoading ? (
        <div className="readiness-result readiness-needs-work">
          <p className="result-label">Step 1 is required</p>
          <h2>Submit the boutique application first.</h2>
          <Link className="inline-link" href="/boutique-application/">
            Go to Boutique Application →
          </Link>
        </div>
      ) : null}

      <div className="form-grid">
        <label>
          <span>Boutique</span>
          <select
            name="boutiqueApplicationId"
            value={applicationId}
            onChange={(event) => {
              setApplicationId(event.target.value);
              window.sessionStorage.setItem(
                "tryonready.applicationId",
                event.target.value,
              );
            }}
            required
          >
            <option value="" disabled>
              Choose the boutique
            </option>
            {applications.map((application) => (
              <option key={application.id} value={application.id}>
                {application.boutiqueName} · {application.status}
              </option>
            ))}
          </select>
        </label>

        <label>
          <span>Garment name</span>
          <input name="name" defaultValue="Moonlight Blazer" required />
        </label>

        <label>
          <span>Product number</span>
          <input name="sku" defaultValue="SYN-BLZ-001" required />
        </label>

        <label>
          <span>Category</span>
          <select name="category" defaultValue="top" required>
            <option value="top">Top</option>
            <option value="bottom">Bottom</option>
            <option value="full_body">Full-body outfit</option>
          </select>
        </label>

        <label>
          <span>Brand</span>
          <input name="brand" defaultValue="Luna & Thread" required />
        </label>

        <label>
          <span>Color</span>
          <input name="color" defaultValue="Terracotta" required />
        </label>

        <label>
          <span>Material</span>
          <input name="material" defaultValue="Cotton blend" required />
        </label>

        <label>
          <span>Size range</span>
          <input name="sizeRange" defaultValue="XS–XL" required />
        </label>

        <label>
          <span>Price (optional)</span>
          <input
            name="price"
            type="number"
            min="0"
            step="0.01"
            defaultValue="89.00"
          />
        </label>

        <label>
          <span>Currency</span>
          <input name="currency" defaultValue="USD" maxLength={3} required />
        </label>

        <label className="wide-field">
          <span>Product description</span>
          <textarea
            name="description"
            defaultValue="A structured synthetic demo blazer for an independent boutique virtual try-on."
            rows={3}
            required
          />
        </label>

        <label className="wide-field">
          <span>Product page (optional)</span>
          <input
            name="productUrl"
            type="url"
            placeholder="https://shop.example/product"
          />
        </label>

        <label className="file-field">
          <span>Garment photograph</span>
          <input
            name="garmentImage"
            type="file"
            accept="image/jpeg,image/png,image/webp"
            onChange={handleFileChange}
            required
          />
          <small>JPEG, PNG, or WebP. Maximum 10 MB. At least 1024 × 1024.</small>
        </label>
      </div>

      {isReading ? <p role="status">Reading image details…</p> : null}

      {imageDetails ? (
        <dl className="image-details" aria-label="Selected image details">
          <div>
            <dt>File</dt>
            <dd>{imageDetails.fileName}</dd>
          </div>
          <div>
            <dt>Type</dt>
            <dd>{imageDetails.mediaType}</dd>
          </div>
          <div>
            <dt>Size</dt>
            <dd>{formatBytes(imageDetails.byteLength)}</dd>
          </div>
          <div>
            <dt>Dimensions</dt>
            <dd>
              {imageDetails.pixelWidth} × {imageDetails.pixelHeight}
            </dd>
          </div>
        </dl>
      ) : null}

      <button
        className="button button-primary readiness-submit"
        type="submit"
        disabled={
          isLoading ||
          isReading ||
          isSubmitting ||
          applications.length === 0
        }
      >
        {isSubmitting ? "Saving and checking…" : "Save garment and check image"}
      </button>

      <p className="privacy-note">
        The garment is stored privately. It becomes available to consumers only
        after administrator approval.
      </p>

      {message ? (
        <div className="readiness-result readiness-error" role="alert">
          <h2>Garment not saved</h2>
          <p>{message}</p>
        </div>
      ) : null}

      {result ? (
        <div className="readiness-result readiness-ready" role="status">
          <p className="result-label">Image ready · Product saved</p>
          <h2>{result.name} is waiting for administrator approval.</h2>
          <p>
            Product <strong>{result.sku}</strong> passed the image check.
            Status: <strong>{result.status}</strong>.
          </p>
          <Link className="inline-link" href="/admin-review/">
            Continue to Admin Review →
          </Link>
        </div>
      ) : null}
    </form>
  );
}
