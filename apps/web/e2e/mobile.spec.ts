import { expect, test } from "@playwright/test";

test("landing and sign-in remain usable on a phone viewport", async ({
  page,
}) => {
  await page.goto("/");
  await expect(
    page.getByRole("heading", {
      name: "Virtual try-on, built for the shop floor.",
    }),
  ).toBeVisible();

  const hasHorizontalOverflow = await page.evaluate(
    () => document.documentElement.scrollWidth > window.innerWidth + 1,
  );
  expect(hasHorizontalOverflow).toBe(false);

  await page.goto("/future-pilot/");
  await expect(
    page.getByRole("heading", { name: "The pilot is next." }),
  ).toBeVisible();
  await expect(page.getByText("10–25", { exact: true })).toBeVisible();
  const pilotHasHorizontalOverflow = await page.evaluate(
    () => document.documentElement.scrollWidth > window.innerWidth + 1,
  );
  expect(pilotHasHorizontalOverflow).toBe(false);

  await page.goto("/sign-in/");
  await expect(
    page.getByRole("heading", {
      name: "Choose the role for the next step.",
    }),
  ).toBeVisible();
  await expect(page.getByLabel("Username")).toBeVisible();
  await expect(page.getByLabel("Password")).toBeVisible();
});
