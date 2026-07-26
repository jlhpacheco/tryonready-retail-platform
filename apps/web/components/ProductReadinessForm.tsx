"use client";

import { ChangeEvent, FormEvent, useState } from "react";

type ImageDetails = {
  fileName: string;
  mediaType: string;
  byteLength: number;
  pixelWidth: number;
  pixelHeight: number;
};

type ReadinessIssue = {
  code: string;
  message: string;
  severity: number;
};

type ReadinessResult = {
  isReady: boolean;
  issues: ReadinessIssue[];
};

const demoProductId = "bac012b4-fc08-4f74-a587-4b42fb791906";
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
      image.onerror = () => reject(new Error("The selected image could not be read."));
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

export function ProductReadinessForm() {
  const [imageDetails, setImageDetails] = useState<ImageDetails | null>(null);
  const [result, setResult] = useState<ReadinessResult | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [isReading, setIsReading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

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

    if (!imageDetails) {
      setMessage("Choose a garment image before checking readiness.");
      return;
    }

    setIsSubmitting(true);

    try {
      const response = await fetch("/api/readiness/assess", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          productId: demoProductId,
          ...imageDetails,
        }),
      });

      if (!response.ok) {
        throw new Error("The readiness service could not check this image.");
      }

      setResult((await response.json()) as ReadinessResult);
    } catch (error) {
      setMessage(
        error instanceof Error
          ? error.message
          : "The readiness service could not check this image.",
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form className="readiness-form" onSubmit={handleSubmit}>
      <div className="form-heading">
        <p className="eyebrow">Working readiness check</p>
        <h1>Check a garment image before using a paid API unit.</h1>
        <p>
          Enter the garment details and choose an image. TryOnReady reads the
          file metadata in your browser and sends only the file name, type,
          size, and dimensions to the local API.
        </p>
      </div>

      <div className="form-grid">
        <label>
          <span>Garment name</span>
          <input
            name="productName"
            type="text"
            placeholder="Example: Moonlight Blazer"
            required
          />
        </label>

        <label>
          <span>Product number</span>
          <input
            name="sku"
            type="text"
            placeholder="Example: BLZ-001"
            required
          />
        </label>

        <label>
          <span>Garment category</span>
          <select name="category" defaultValue="" required>
            <option value="" disabled>
              Choose a category
            </option>
            <option value="top">Top</option>
            <option value="bottom">Bottom</option>
            <option value="full_body">Full-body outfit</option>
          </select>
        </label>

        <label className="file-field">
          <span>Garment image</span>
          <input
            name="garmentImage"
            type="file"
            accept="image/jpeg,image/png,image/webp"
            onChange={handleFileChange}
            required
          />
          <small>JPEG, PNG, or WebP. Maximum 15 MB. At least 1024 × 1024.</small>
        </label>
      </div>

      {isReading ? (
        <p className="form-message" role="status">
          Reading image details…
        </p>
      ) : null}

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
        disabled={isReading || isSubmitting}
      >
        {isSubmitting ? "Checking…" : "Check image readiness"}
      </button>

      <p className="privacy-note">
        Privacy note: this readiness check does not upload or store the
        photograph.
      </p>

      {message ? (
        <div className="readiness-result readiness-error" role="alert">
          <h2>We could not complete the check.</h2>
          <p>{message}</p>
        </div>
      ) : null}

      {result ? (
        <div
          className={`readiness-result ${
            result.isReady ? "readiness-ready" : "readiness-needs-work"
          }`}
          role="status"
          aria-live="polite"
        >
          <p className="result-label">
            {result.isReady ? "Ready for the next step" : "Needs a better image"}
          </p>
          <h2>
            {result.isReady
              ? "This image passes the scaffold readiness checks."
              : "Fix these items and check the image again."}
          </h2>
          {result.issues.length > 0 ? (
            <ul>
              {result.issues.map((issue) => (
                <li key={issue.code}>{issue.message}</li>
              ))}
            </ul>
          ) : (
            <p>
              Live YouCam validation is the next reviewed phase and is not
              called by this check.
            </p>
          )}
        </div>
      ) : null}
    </form>
  );
}
