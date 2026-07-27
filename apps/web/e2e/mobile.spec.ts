import { expect, test } from "@playwright/test";

test("landing and sign-in remain usable on a phone viewport", async ({
  page,
}) => {
  await page.goto("/");
  await expect(
    page.getByRole("heading", {
      name: /Virtual try-on that a small boutique can actually run/i,
    }),
  ).toBeVisible();

  const hasHorizontalOverflow = await page.evaluate(
    () => document.documentElement.scrollWidth > window.innerWidth + 1,
  );
  expect(hasHorizontalOverflow).toBe(false);

  await page.goto("/sign-in/");
  await expect(
    page.getByRole("heading", {
      name: "Choose the role for the next step.",
    }),
  ).toBeVisible();
  await expect(page.getByLabel("Username")).toBeVisible();
  await expect(page.getByLabel("Password")).toBeVisible();
});
