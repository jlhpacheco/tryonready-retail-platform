# TryOnReady Retail Platform

TryOnReady is a mobile-first retail technology service for independent clothing boutiques. Its product direction is:

> Virtual try-on that a small boutique can actually run.

A boutique prepares and approves each garment once; guest shoppers try it
privately without an account; valid provider requests and API units stay
controlled; and the retailer sees aggregate interest without customer photos.
Conversion and return improvements are pilot hypotheses, not established
outcomes.

This repository began as a hackathon scaffold and is now implementing the real
server-side YouCam vertical slice. Consult the implementation log for the exact
working-versus-pending boundary; do not infer completion from a page alone.

The boutique retailer pays for TryOnReady and offers virtual try-on to shoppers
at no charge as a courtesy and convenience. Subscription billing is outside the
hackathon MVP; the operational dashboard tracks provider usage instead.

## First-phase capabilities

- Public, responsive Next.js landing page and workflow routes
- Persistent boutique application, catalog, approval, and try-on records
- Server-validated private garment and consumer image uploads
- Consumer consent, duplicate-request protection, and background processing
- YouCam live adapter plus deterministic provider simulation for automated tests
- Retailer results and API-unit dashboard
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
samples/synthetic/                Authorized fictional demo images and metadata
docs/judge/                       Judge-ready Word and PDF guide
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
- `http://localhost:5090/api/status` — provider, persistence, and secret-boundary status
- `http://localhost:5090/api/demo/catalog` — synthetic Luna & Thread catalog
- `POST http://localhost:5090/api/products` — private multipart garment submission
- `POST http://localhost:5090/api/try-on-jobs` — private multipart consumer try-on submission
- `GET http://localhost:5090/api/dashboard` — aggregate results and API-unit totals

Repository defaults keep live YouCam mode disabled. A rotated credential was
verified in local Visual Studio User Secrets without printing its value. On
July 28, 2026, one controlled YouCam AI Clothes v3 task completed in
`YouCamLive` mode with terminal state `Succeeded` and one recorded API unit.
The public Fly judge path replays that provenance-locked result and makes no new
provider request. Simulation mode follows the same application workflow without
spending an API unit. No provider credential is accepted from browser code.

The Playwright smoke suite starts an isolated Release-mode API at
`http://127.0.0.1:5091` and runs the complete retailer, administrator, and guest
workflow in desktop Chrome plus a mobile Chrome smoke test. The same suite has
also passed against the isolated local `tryonready-postgres` database. It does
not use the live YouCam API or consume API units.

## Safety and scope

Read the following documents before contributing:

- [Hackathon scope](HACKATHON-SCOPE.md)
- [IP and license boundary](IP-AND-LICENSE-BOUNDARY.md)
- [Data and image safety](DATA-AND-IMAGE-SAFETY.md)
- [Security policy](SECURITY.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Governing product brief](docs/GOVERNING-PRODUCT-BRIEF.md)
- [YouCam account and secret setup](docs/YOUCAM-SETUP.md)
- [PostgreSQL isolation](docs/POSTGRES-ISOLATION.md)
- [Image storage and deletion policy](docs/DATA-RETENTION.md)
- [Fly.io deployment runbook](docs/FLY-DEPLOYMENT.md)
- [Implementation and verification log](docs/IMPLEMENTATION-LOG.md)
- [Market validation](docs/MARKET-VALIDATION.md)
- [Value proposition and novelty](docs/VALUE-PROPOSITION-AND-NOVELTY.md)
- [Judge evaluation map](docs/JUDGE-EVALUATION-MAP.md)
- [Video narrative](docs/VIDEO-NARRATIVE.md)
- [Pilot metrics](docs/PILOT-METRICS.md)
- [Hackathon lessons learned](docs/HACKATHON-LESSONS-LEARNED.md)
- [Judge and manual testing guide](docs/JUDGE-TESTING-GUIDE.md)
- [Judge guide in Word](docs/judge/TryOnReady-Judge-and-Use-Case-Guide.docx)
- [Judge guide in PDF](docs/judge/TryOnReady-Judge-and-Use-Case-Guide.pdf)
- [Synthetic asset metadata](samples/synthetic/ASSET-METADATA.md)
- [Product scope](docs/PRODUCT-SCOPE.md)
- [Demo script](docs/DEMO-SCRIPT.md)
- [Small-business user guide](docs/SMALL-BUSINESS-USER-GUIDE.md)
- [Scaffold implementation and verification record](docs/SCAFFOLD-IMPLEMENTATION.md)

Do not add credentials, real boutique/customer data, or customer photographs. The YouCam credential must remain server-side. This public source repository is available only for hackathon judging and testing under the [TryOnReady Source-Available Hackathon Judging License](LICENSE); it is not open-source software.
