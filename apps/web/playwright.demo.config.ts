import { defineConfig, devices } from "@playwright/test";
import { resolve } from "node:path";

const baseURL =
  process.env.PLAYWRIGHT_BASE_URL ?? "http://127.0.0.1:5090";

export default defineConfig({
  testDir: "./e2e",
  testMatch: "judge-demo.spec.ts",
  fullyParallel: false,
  forbidOnly: true,
  retries: 0,
  timeout: 300_000,
  workers: 1,
  outputDir: "../../artifacts/demo-runner",
  reporter: [["list"]],
  use: {
    baseURL,
    headless: process.env.TRYONREADY_DEMO_HEADLESS !== "false",
    launchOptions: {
      slowMo: 120,
    },
    screenshot: "only-on-failure",
    trace: "retain-on-failure",
    video: {
      mode: "on",
      size: {
        width: 1440,
        height: 900,
      },
    },
    viewport: {
      width: 1440,
      height: 900,
    },
  },
  webServer: {
    command:
      "dotnet run --project ../../src/TryOnReady.Api/TryOnReady.Api.csproj --configuration Release --no-build --no-launch-profile --urls http://127.0.0.1:5090",
    url: `${baseURL}/health`,
    reuseExistingServer:
      process.env.PLAYWRIGHT_REUSE_EXISTING_SERVER === "true",
    timeout: 300_000,
    env: {
      ...process.env,
      ASPNETCORE_ENVIRONMENT: "Testing",
      Persistence__Provider: "InMemory",
      Persistence__InMemoryDatabaseName: "tryonready-demo",
      PrivateStorage__RootPath: ".tryonready-data/demo",
      YouCam__Enabled: "false",
      YouCam__SimulationEnabled: "true",
      YouCam__SimulationResultPath: resolve(
        process.cwd(),
        "..",
        "..",
        "artifacts",
        "video-build",
        "master-full-result.jpg",
      ),
      TryOnProcessing__ApiUnitsPerTask: "1",
    },
  },
  projects: [
    {
      name: "judge-demo",
      use: {
        ...devices["Desktop Chrome"],
        channel: "chrome",
        viewport: {
          width: 1440,
          height: 900,
        },
      },
    },
  ],
});
