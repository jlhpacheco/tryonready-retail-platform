# TryOnReady Retail Platform

TryOnReady is a mobile-first retail technology scaffold for independent clothing boutiques. Its product direction is:

> Virtual try-on for boutiques without the enterprise budget.

This repository is an early hackathon foundation. It does not contain a live YouCam integration, production persistence, customer photographs, or production boutique data.

## First-phase capabilities

- Public, responsive Next.js landing page and workflow routes
- Working Product Readiness form backed by the local API; image bytes stay in the browser
- Working in-memory Boutique Application submission and Admin Review decisions
- Consumer Try-On consent, image preflight, and approved synthetic garment presentation
- ASP.NET Core 10 API and worker foundations
- Health check and OpenAPI document endpoints
- Neutral server-side virtual try-on adapter contracts
- Synthetic Luna & Thread demo fixtures
- Unit and API integration test foundations

## Repository layout

```text
apps/web/                         Next.js frontend
src/TryOnReady.Api/               HTTP API composition root
src/TryOnReady.Application/       Application workflows and ports
src/TryOnReady.Domain/            Lean domain concepts
src/TryOnReady.Infrastructure/    Infrastructure registration and fixtures
src/TryOnReady.YouCam/            External VTO adapter boundary
src/TryOnReady.Worker/            Background worker composition root
tests/TryOnReady.UnitTests/       xUnit unit tests
tests/TryOnReady.IntegrationTests/ ASP.NET Core integration tests
```

## Prerequisites

- .NET SDK 10
- Node.js 20 or newer
- npm

## Local verification

```powershell
dotnet restore .\TryOnReady.sln
dotnet build .\TryOnReady.sln --no-restore
dotnet test .\TryOnReady.sln --no-build

Set-Location .\apps\web
npm install
npm run lint
npm run typecheck
npm run build
npm run test:e2e
```

For the Visual Studio experience, open `TryOnReady.sln`, select the `TryOnReady.Api` HTTP profile, and press F5 or Ctrl+F5. The API build exports the Next.js frontend, the API serves it at `http://localhost:5090/`, and Visual Studio opens the site automatically.

For frontend hot reload outside Visual Studio, run `npm run dev` from `apps/web`; the standalone development server uses its own local URL.

## Local API endpoints

With `TryOnReady.Api` running:

- `http://localhost:5090/api` — API entry point and endpoint index
- `http://localhost:5090/health` — health check
- `http://localhost:5090/openapi/v1.json` — OpenAPI document
- `http://localhost:5090/api/scaffold` — scaffold/integration status
- `http://localhost:5090/api/demo/catalog` — synthetic Luna & Thread catalog
- `POST http://localhost:5090/api/readiness/assess` — metadata-only product readiness assessment

The live YouCam Apparel VTO request is intentionally disabled in this phase. Provider credentials must remain server-side and a reviewed follow-up task must implement upload, task creation, polling, and result retrieval.

The Playwright smoke suite starts an isolated Release-mode API at `http://127.0.0.1:5091` and runs the landing-page and API workflows in desktop and mobile-sized Chrome contexts. It does not use the live YouCam API or consume API units.

## Safety and scope

Read the following documents before contributing:

- [Hackathon scope](HACKATHON-SCOPE.md)
- [IP and license boundary](IP-AND-LICENSE-BOUNDARY.md)
- [Data and image safety](DATA-AND-IMAGE-SAFETY.md)
- [Security policy](SECURITY.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Product scope](docs/PRODUCT-SCOPE.md)
- [Demo script](docs/DEMO-SCRIPT.md)
- [Small-business user guide](docs/SMALL-BUSINESS-USER-GUIDE.md)
- [Scaffold implementation and verification record](docs/SCAFFOLD-IMPLEMENTATION.md)

Do not add credentials, real boutique/customer data, or customer photographs. The YouCam credential must remain server-side. This repository intentionally has no open-source license.
