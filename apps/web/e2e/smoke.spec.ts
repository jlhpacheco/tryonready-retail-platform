import { expect, test } from "@playwright/test";
import { resolve } from "node:path";

const retailer = {
  username: "retailer@tryonready.demo",
  password: "PlaywrightRetailer!2026",
};

const administrator = {
  username: "admin@tryonready.demo",
  password: "PlaywrightAdmin!2026",
};

const expectedPersistence =
  process.env.PLAYWRIGHT_EXPECTED_PERSISTENCE ?? "InMemory";

const blazer = resolve(
  process.cwd(),
  "..",
  "..",
  "samples",
  "synthetic",
  "garments",
  "moonlight-blazer.png",
);

const marisol = resolve(
  process.cwd(),
  "..",
  "..",
  "samples",
  "synthetic",
  "customers",
  "marisol-lopez-source.png",
);

async function signIn(
  page: import("@playwright/test").Page,
  account: "Retailer" | "Administrator",
) {
  const credentials = account === "Retailer" ? retailer : administrator;

  await page.goto("/sign-in/");
  await page
    .getByRole("group", { name: "Demo role" })
    .getByRole("button", { name: new RegExp(`^${account}\\b`) })
    .click();
  await page.getByLabel("Username").fill(credentials.username);
  await page.getByLabel("Password").fill(credentials.password);
  await page
    .getByRole("button", { name: `Continue as ${account}` })
    .click();
}

test.describe.serial("TryOnReady verified judge journey", () => {
  test("smoke checks public health and protected access", async ({ request }) => {
    const health = await request.get("/health");
    expect(health.status()).toBe(200);
    expect(await health.text()).toBe("Healthy");

    const status = await request.get("/api/status");
    expect(status.status()).toBe(200);
    expect(await status.json()).toMatchObject({
      providerMode: "Simulation",
      liveYouCamIntegration: false,
      apiKeyExposedToBrowser: false,
      persistence: expectedPersistence,
    });

    const identity = await request.get("/api/auth/me");
    expect(identity.status()).toBe(200);
    expect(await identity.json()).toMatchObject({
      isAuthenticated: false,
    });

    const protectedQueue = await request.get("/api/boutique-applications");
    expect(protectedQueue.status()).toBe(401);
  });

  test("completes retailer, administrator, and guest try-on flow", async ({
    page,
    request,
  }) => {
    const runId = Date.now().toString().slice(-8);
    const businessEmail = `elena+${runId}@luna-thread.example.invalid`;
    const sku = `SYN-BLZ-${runId}`;

    const baselineLogin = await request.post("/api/auth/login", {
      data: administrator,
    });
    expect(baselineLogin.status()).toBe(200);
    const baselineResponse = await request.get("/api/dashboard");
    expect(baselineResponse.status()).toBe(200);
    const baseline = (await baselineResponse.json()) as {
      totalJobs: number;
      succeededJobs: number;
      duplicateRequestsPrevented: number;
    };

    await signIn(page, "Retailer");
    await expect(page).toHaveURL(/\/boutique-application\/?$/);

    await page.getByLabel("Business email").fill(businessEmail);
    await page
      .getByLabel(/I confirm that this boutique will use only photographs/i)
      .check();
    await page.getByRole("button", { name: "Submit application" }).click();
    await expect(page.getByText("Application submitted")).toBeVisible();
    await page
      .getByRole("link", { name: /Continue to Product Readiness/i })
      .click();

    await expect(
      page.getByRole("heading", {
        name: "Save the product and check its image.",
      }),
    ).toBeVisible();
    await page.getByLabel("Product number").fill(sku);
    await page.getByLabel("Garment photograph").setInputFiles(blazer);
    await expect(page.getByText("1254 × 1254")).toBeVisible();
    await page
      .getByRole("button", { name: "Save garment and check image" })
      .click();
    await expect(
      page.getByText(
        "Moonlight Blazer is waiting for administrator approval.",
      ),
    ).toBeVisible({ timeout: 30_000 });

    await signIn(page, "Administrator");
    await expect(page).toHaveURL(/\/admin-review\/?$/);
    const applicationCard = page
      .getByRole("region", { name: "Boutique applications" })
      .locator("article")
      .filter({ hasText: businessEmail });
    await expect(applicationCard).toBeVisible();
    await applicationCard
      .getByRole("button", { name: "Approve boutique" })
      .click();
    await expect(
      page.getByText("Boutique application saved: Approved."),
    ).toBeVisible();

    const productCard = page
      .getByRole("region", { name: "Product reviews" })
      .locator("article")
      .filter({ hasText: sku });
    await expect(productCard).toBeVisible();
    await productCard.getByRole("button", { name: "Approve product" }).click();
    await expect(
      page.getByText("Product review saved: Approved."),
    ).toBeVisible();
    await productCard
      .getByRole("link", { name: /Continue to Consumer Try-On/i })
      .click();

    await expect(
      page.getByRole("heading", { name: "Moonlight Blazer" }),
    ).toBeVisible();
    await expect(page.getByText("YouCam connection: Demo mode")).toBeVisible();
    await expect(page.getByText("Secret keys: hidden from shoppers")).toBeVisible();
    await page.getByLabel("Person image").setInputFiles(marisol);
    await expect(page.getByText(/864 × 1821/)).toBeVisible();
    await page
      .getByLabel(/I consent to processing this image/i)
      .check();
    await page
      .getByRole("button", { name: "Generate virtual try-on" })
      .click();

    await expect(page.getByText("Try-on status · Succeeded")).toBeVisible({
      timeout: 30_000,
    });
    await expect(
      page.getByRole("img", {
        name: "Generated virtual try-on result for Moonlight Blazer",
      }),
    ).toBeVisible();
    await expect(page.getByText("Result ready · Live try-ons used: 0")).toBeVisible();

    await page
      .getByRole("button", { name: "Generate virtual try-on" })
      .click();
    await expect(page.getByText("Already generated.")).toBeVisible({
      timeout: 15_000,
    });

    await page
      .getByRole("link", { name: /View the retailer results dashboard/i })
      .click();
    await expect(page).toHaveURL(/\/admin-review\/?$/);
    await expect(page.getByText("Usage without customer photographs.")).toBeVisible();
    const metric = (label: string) => {
      const exactLabel = new RegExp(
        `^${label.replace(/[.*+?^${}()|[\]\\]/g, "\\$&")}$`,
      );
      return page.locator(".dashboard-grid > div").filter({
        has: page.locator("dt").filter({ hasText: exactLabel }),
      });
    };
    await expect(metric("Try-ons").locator("dd")).toHaveText(
      String(baseline.totalJobs + 1),
    );
    await expect(metric("Completed").locator("dd")).toHaveText(
      String(baseline.succeededJobs + 1),
    );
    await expect(metric("Live try-ons used").locator("dd")).toHaveText("0");
    await expect(metric("Duplicates stopped").locator("dd")).toHaveText(
      String(baseline.duplicateRequestsPrevented + 1),
    );
  });
});
