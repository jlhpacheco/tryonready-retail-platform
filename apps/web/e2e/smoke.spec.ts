import { expect, test } from "@playwright/test";
import { join } from "node:path";

test.describe("TryOnReady scaffold smoke", () => {
  test("renders the landing page and navigates to product readiness", async ({
    page,
  }) => {
    await page.goto("/");

    await expect(
      page.getByRole("heading", {
        name: /Virtual try-on, without the enterprise budget/i,
      }),
    ).toBeVisible();
    await expect(
      page.getByText("Luna & Thread", { exact: true }),
    ).toBeVisible();

    await page
      .getByRole("navigation", { name: "Primary navigation" })
      .getByRole("link", { name: "Product Readiness" })
      .click();

    await expect(page).toHaveURL(/\/product-readiness\/?$/);
    await expect(
      page.getByRole("heading", {
        name: "Check a garment image before using a paid API unit.",
      }),
    ).toBeVisible();
  });

  test("checks a selected garment image and explains readiness problems", async ({
    page,
  }) => {
    await page.goto("/product-readiness/");

    await page.getByLabel("Garment name").fill("Synthetic Demo Blazer");
    await page.getByLabel("Product number").fill("DEMO-BLZ-001");
    await page.getByLabel("Garment category").selectOption("top");
    await page.getByLabel("Garment image").setInputFiles({
      name: "synthetic-demo.png",
      mimeType: "image/png",
      buffer: Buffer.from(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAusB9Y9Z2ZkAAAAASUVORK5CYII=",
        "base64",
      ),
    });

    await expect(
      page.getByRole("definition").filter({ hasText: "1 × 1" }),
    ).toBeVisible();
    await page
      .getByRole("button", { name: "Check image readiness" })
      .click();

    await expect(page.getByText("Needs a better image")).toBeVisible();
    await expect(
      page.getByText(
        "Use an image at least 1024 pixels wide and 1024 pixels tall.",
      ),
    ).toBeVisible();
  });

  test("serves API health, index, catalog, and readiness responses", async ({
    request,
  }) => {
    const healthResponse = await request.get("/health");
    expect(healthResponse.status()).toBe(200);
    expect(await healthResponse.text()).toBe("Healthy");

    const apiResponse = await request.get("/api");
    expect(apiResponse.status()).toBe(200);
    expect(await apiResponse.json()).toMatchObject({
      name: "TryOnReady API",
      phase: "scaffold",
      liveYouCamIntegration: false,
      openApi: "/openapi/v1.json",
    });

    const catalogResponse = await request.get("/api/demo/catalog");
    expect(catalogResponse.status()).toBe(200);
    expect(await catalogResponse.json()).toMatchObject({
      boutique: {
        displayName: "Luna & Thread",
        ownerDisplayName: "Elena Rivera",
      },
    });

    const readinessResponse = await request.post("/api/readiness/assess", {
      data: {
        productId: "bac012b4-fc08-4f74-a587-4b42fb791906",
        fileName: "synthetic-blazer.jpg",
        mediaType: "image/jpeg",
        byteLength: 2_000_000,
        pixelWidth: 1_600,
        pixelHeight: 1_600,
      },
    });
    expect(readinessResponse.status()).toBe(200);
    expect(await readinessResponse.json()).toMatchObject({
      isReady: true,
      issues: [],
    });
  });

  test("completes boutique submission, admin approval, and consumer preflight", async ({
    page,
  }) => {
    await page.goto("/boutique-application/");

    await expect(
      page.getByRole("heading", {
        name: "Tell us about your independent shop.",
      }),
    ).toBeVisible();
    await page
      .getByLabel(/I confirm that this boutique will use only photographs/i)
      .check();
    await page.getByRole("button", { name: "Submit application" }).click();
    await expect(page.getByText("Application submitted")).toBeVisible();

    await page.goto("/admin-review/");
    await expect(
      page.getByRole("heading", {
        name: "Review the boutique and product before consumer try-on.",
      }),
    ).toBeVisible();
    await page
      .getByRole("button", { name: "Approve boutique" })
      .first()
      .click();
    await expect(
      page.getByText("Boutique application saved: Approved."),
    ).toBeVisible();
    await page.getByRole("button", { name: "Approve product" }).click();
    await expect(
      page.getByText("Product review saved: Approved."),
    ).toBeVisible();

    await page.goto("/consumer-try-on/");
    await expect(
      page.getByRole("heading", { name: "Prepare your private try-on." }),
    ).toBeVisible();
    await page.getByLabel("Person image").setInputFiles(
      join(
        process.cwd(),
        "public",
        "demo",
        "synthetic-terracotta-blazer.png",
      ),
    );
    await page
      .getByLabel(/I understand the image-handling notice/i)
      .check();
    await page.getByRole("button", { name: "Prepare try-on" }).click();
    await expect(
      page.getByText("Preflight passed", { exact: true }),
    ).toBeVisible();
    await expect(
      page.getByText("Live YouCam generation is not enabled yet."),
    ).toBeVisible();
  });
});
