import { expect, test, type Page } from "@playwright/test";
import { resolve } from "node:path";

const retailer = {
  username: "retailer@tryonready.demo",
  passwordEnvironmentVariable: "TRYONREADY_RETAILER_PASSWORD",
};

const administrator = {
  username: "admin@tryonready.demo",
  passwordEnvironmentVariable: "TRYONREADY_ADMIN_PASSWORD",
};

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

const proofHoldMilliseconds = Number(
  process.env.TRYONREADY_PROOF_HOLD_MS ?? "1200",
);

function requiredEnvironmentVariable(name: string) {
  const value = process.env[name];

  if (!value) {
    throw new Error(
      `${name} is required at runtime and must never be committed to the repository.`,
    );
  }

  return value;
}

async function holdProof(page: Page, multiplier = 1) {
  await page.waitForTimeout(proofHoldMilliseconds * multiplier);
}

async function frameLocatorBelowHeader(page: Page, selector: string) {
  await page.locator(selector).evaluate((element) => {
    const top = element.getBoundingClientRect().top + window.scrollY - 130;
    window.scrollTo({ top, behavior: "auto" });
  });
}

async function signIn(
  page: Page,
  account: "Retailer" | "Administrator",
) {
  const credentials = account === "Retailer" ? retailer : administrator;
  const password = requiredEnvironmentVariable(
    credentials.passwordEnvironmentVariable,
  );

  await page.goto("/sign-in/");
  await page
    .getByRole("group", { name: "Demo role" })
    .getByRole("button", { name: new RegExp(`^${account}\\b`) })
    .click();
  await page.getByLabel("Username").fill(credentials.username);
  await page.getByLabel("Password").fill(password);
  await page
    .getByRole("button", { name: `Continue as ${account}` })
    .click();
  await page.waitForLoadState("networkidle");
  await page.waitForTimeout(500);
}

test("records the guarded TryOnReady judge journey", async ({ page }) => {
  const mode =
    process.env.TRYONREADY_DEMO_MODE ??
    (process.env.npm_lifecycle_event === "demo:master" ? "master" : "prerun");

  await page.goto("/");
  await expect(
    page.getByRole("heading", {
      name: "Virtual try-on, built for the shop floor.",
    }),
  ).toBeVisible();
  await expect(
    page.getByText("Garment checked before try-on", { exact: true }),
  ).toBeVisible();
  await holdProof(page, 2);

  await page
    .getByRole("link", { name: "Start the Luna & Thread demo" })
    .click();
  await expect(page).toHaveURL(/\/sign-in\/?$/);

  await signIn(page, "Retailer");
  await expect(page).toHaveURL(/\/boutique-application\/?$/);
  const imageRightsConfirmation = page.getByLabel(
    /I confirm that this boutique will use only photographs/i,
  );
  await imageRightsConfirmation.check();
  await expect(imageRightsConfirmation).toBeChecked();
  await page.getByRole("button", { name: "Submit application" }).click();
  await expect(page.getByText("Application submitted")).toBeVisible();
  await holdProof(page);

  await page
    .getByRole("link", { name: /Continue to Product Readiness/i })
    .click();
  await page.getByLabel("Garment photograph").setInputFiles(blazer);
  await expect(page.getByText("1254 × 1254")).toBeVisible();
  await page
    .getByRole("button", { name: "Save garment and check image" })
    .click();
  await expect(
    page.getByText(
      "Moonlight Blazer is waiting for administrator approval.",
    ),
  ).toBeVisible();
  await holdProof(page);

  await signIn(page, "Administrator");
  await expect(page).toHaveURL(/\/admin-review\/?$/);
  await expect(
    page.getByRole("region", { name: "Boutique applications" }).locator("article"),
  ).toHaveCount(1);
  await expect(
    page.getByRole("region", { name: "Product reviews" }).locator("article"),
  ).toHaveCount(1);

  await page.getByRole("button", { name: "Approve boutique" }).click();
  await expect(
    page.getByText("Boutique application saved: Approved."),
  ).toBeVisible();
  await page.getByRole("button", { name: "Approve product" }).click();
  await expect(page.getByText("Product review saved: Approved.")).toBeVisible();
  await holdProof(page);

  await page
    .getByRole("link", { name: /Continue to Consumer Try-On/i })
    .click();
  await expect(
    page.getByText(
      /YouCam connection: (Live and ready|Stored replay · zero new provider requests)/,
    ),
  ).toBeVisible();
  await expect(page.getByText("Secret keys: hidden from shoppers")).toBeVisible();
  await page.getByLabel("Person image").setInputFiles(marisol);
  await expect(page.getByText(/864 × 1821/)).toBeVisible();
  await frameLocatorBelowHeader(page, ".consumer-preview");
  await holdProof(page, 2);
  await page.getByLabel(/I consent to processing this image/i).check({
    force: true,
  });
  await frameLocatorBelowHeader(page, ".consumer-preview");
  await holdProof(page, 2);

  if (mode !== "master") {
    await expect(
      page.getByRole("button", { name: "Generate virtual try-on" }),
    ).toBeEnabled();
    return;
  }

  if (process.env.TRYONREADY_ALLOW_LIVE_PROVIDER_CALL !== "true") {
    throw new Error(
      "The live master is guarded. Set TRYONREADY_ALLOW_LIVE_PROVIDER_CALL=true only after approving one controlled YouCam task.",
    );
  }

  await page
    .getByRole("button", { name: "Generate virtual try-on" })
    .click();
  await expect(page.getByText("Try-on status · Succeeded")).toBeVisible({
    timeout: 180_000,
  });
  await expect(
    page.getByRole("img", {
      name: "Generated virtual try-on result for Moonlight Blazer",
    }),
  ).toBeVisible();
  await expect(
    page.getByText(
      /Generated with YouCam · Requests used: 1|previously completed controlled demonstration · playback makes zero new provider requests/,
    ),
  ).toBeVisible();
  await frameLocatorBelowHeader(page, ".tryon-result-panel");
  await holdProof(page, 3);

  await page
    .getByRole("button", { name: "Generate virtual try-on" })
    .click();
  await expect(page.getByText("Already generated.")).toBeVisible({
    timeout: 15_000,
  });
  await frameLocatorBelowHeader(page, ".tryon-result-panel");
  await holdProof(page);

  await page
    .getByRole("link", { name: /View the retailer results dashboard/i })
    .click();
  await expect(page).toHaveURL(/\/admin-review\/?$/);
  await expect(
    page.getByRole("heading", {
      name: "One completed journey. The controls did their job.",
    }),
  ).toBeVisible();
  await holdProof(page, 2);
});
