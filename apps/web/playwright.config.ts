import { defineConfig, devices } from "@playwright/test";

const baseURL =
  process.env.PLAYWRIGHT_BASE_URL ?? "http://127.0.0.1:5091";

export default defineConfig({
  testDir: "./e2e",
  fullyParallel: false,
  forbidOnly: true,
  retries: 0,
  timeout: 120_000,
  workers: 1,
  reporter: [["list"], ["html", { open: "never" }]],
  use: {
    baseURL,
    trace: "retain-on-failure",
  },
  webServer: {
    command:
      "dotnet run --project ../../src/TryOnReady.Api/TryOnReady.Api.csproj --configuration Release --no-build --no-launch-profile --urls http://127.0.0.1:5091",
    url: `${baseURL}/health`,
    reuseExistingServer:
      process.env.PLAYWRIGHT_REUSE_EXISTING_SERVER === "true",
    timeout: 300_000,
    env: {
      ...process.env,
      ASPNETCORE_ENVIRONMENT: "Testing",
      Persistence__Provider: "InMemory",
      Persistence__InMemoryDatabaseName: "tryonready-playwright",
      PrivateStorage__RootPath: ".tryonready-data/playwright",
      YouCam__Enabled: "false",
      YouCam__SimulationEnabled: "true",
      TryOnProcessing__ApiUnitsPerTask: "1",
    },
  },
  projects: [
    {
      name: "desktop-chrome",
      testMatch: "smoke.spec.ts",
      use: {
        ...devices["Desktop Chrome"],
        channel: "chrome",
      },
    },
    {
      name: "mobile-chrome",
      testMatch: "mobile.spec.ts",
      use: {
        ...devices["Pixel 5"],
        channel: "chrome",
      },
    },
  ],
});
