"use client";

import { ChangeEvent, FormEvent, useEffect, useState } from "react";

type ImageDetails = {
  fileName: string;
  mediaType: string;
  byteLength: number;
  pixelWidth: number;
  pixelHeight: number;
};

type PreflightResult = {
  sessionId: string;
  status: string;
  productName: string;
  providerEnabled: boolean;
  message: string;
};

const productId = "bac012b4-fc08-4f74-a587-4b42fb791906";

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

export function ConsumerTryOnPreflight() {
  const [imageDetails, setImageDetails] = useState<ImageDetails | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [result, setResult] = useState<PreflightResult | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [isBusy, setIsBusy] = useState(false);

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
    setResult(null);
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
    setResult(null);
    setMessage(null);

    if (!imageDetails) {
      setMessage("Choose an authorized synthetic person image first.");
      return;
    }

    const form = new FormData(event.currentTarget);
    setIsBusy(true);

    try {
      const response = await fetch("/api/consumer/try-on/preflight", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          productId,
          ...imageDetails,
          consentAccepted: form.get("consent") === "on",
        }),
      });

      if (!response.ok) {
        const problem = (await response.json()) as {
          errors?: Record<string, string[]>;
          issues?: { message: string }[];
        };
        const firstError =
          Object.values(problem.errors ?? {})[0]?.[0] ??
          problem.issues?.[0]?.message;
        throw new Error(
          firstError ??
            "The try-on preflight could not be completed. Approve the product first.",
        );
      }

      setResult((await response.json()) as PreflightResult);
    } catch (error) {
      setMessage(
        error instanceof Error
          ? error.message
          : "The try-on preflight could not be completed.",
      );
    } finally {
      setIsBusy(false);
    }
  }

  return (
    <section className="consumer-shell">
      <div className="consumer-product">
        <p className="eyebrow">Approved demo garment</p>
        {/* eslint-disable-next-line @next/next/no-img-element */}
        <img
          src="/demo/synthetic-terracotta-blazer.png"
          alt="Synthetic terracotta tailored blazer"
        />
        <h1>Sunset Tailored Blazer</h1>
        <p>Luna & Thread · SYN-BLZ-001</p>
      </div>

      <form className="consumer-form" onSubmit={handleSubmit}>
        <div className="form-heading">
          <p className="eyebrow">Consumer try-on</p>
          <h2>Prepare your private try-on.</h2>
          <p>
            Choose an authorized synthetic person image. This demonstration
            checks the image and consent before any provider request.
          </p>
        </div>

        <label className="upload-field">
          <span>Person image</span>
          <input
            type="file"
            accept="image/jpeg,image/png,image/webp"
            onChange={handleImageChange}
            required
          />
          <small>JPEG, PNG, or WebP. At least 1024 × 1024.</small>
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
          <h3>Before you continue</h3>
          <ul>
            <li>The boutique does not receive your photograph.</li>
            <li>No image content is written to application logs.</li>
            <li>Virtual try-on does not guarantee fit or sizing.</li>
          </ul>
        </div>

        <label className="consent-field">
          <input name="consent" type="checkbox" required />
          <span>
            I understand the image-handling notice and consent to this
            preflight check.
          </span>
        </label>

        <button
          className="button button-primary form-action"
          type="submit"
          disabled={isBusy}
        >
          {isBusy ? "Checking…" : "Prepare try-on"}
        </button>

        {message ? (
          <div className="readiness-result readiness-error" role="alert">
            <h3>Try-on is not ready</h3>
            <p>{message}</p>
          </div>
        ) : null}

        {result ? (
          <div className="readiness-result readiness-ready" role="status">
            <p className="result-label">Preflight passed</p>
            <h3>{result.productName} is ready for secure submission.</h3>
            <p>{result.message}</p>
            <p>
              Session: <code>{result.sessionId}</code>
            </p>
          </div>
        ) : null}
      </form>
    </section>
  );
}
