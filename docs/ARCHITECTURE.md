# Architecture

TryOnReady uses a small layered .NET backend and a separate Next.js frontend.

```text
Next.js web application
        |
ASP.NET Core API
        |
Application workflows and ports
   /                    \
Domain              Infrastructure
                         |
                 YouCam adapter boundary
                         |
                  Background worker
```

## Backend responsibilities

- `TryOnReady.Domain`: lean entities, enums, and value-oriented result records with no infrastructure dependencies.
- `TryOnReady.Application`: use-case services and provider/repository ports. Business logic lives here rather than in API endpoints.
- `TryOnReady.Infrastructure`: dependency registration and synthetic scaffold repositories. PostgreSQL/EF Core is deferred.
- `TryOnReady.YouCam`: provider-facing contracts, safe options binding, and a disabled scaffold adapter. No live HTTP calls are present.
- `TryOnReady.Api`: HTTP composition root, health checks, OpenAPI, and boundary validation.
- `TryOnReady.Worker`: background processing composition root. It currently proves hosting and dependency wiring only.

Dependencies point inward toward the domain. The API and worker compose implementations at startup.

## Frontend responsibilities

`apps/web` contains a strict TypeScript, App Router-based Next.js application with semantic responsive navigation, a landing page, placeholder workflow routes, and a PWA manifest baseline. It contains no provider credentials or real photographs.

For a reliable one-click Visual Studio workflow, the Next.js application uses static export. Building `TryOnReady.Api` runs the frontend production build and copies the generated, ignored output into the API's `wwwroot`. In local Visual Studio debugging, the API serves both the exported web shell and `/api`, `/health`, and `/openapi` endpoints from `http://localhost:5090`. Frontend hot reload remains available separately through `npm run dev`.

## Data

The scaffold uses immutable synthetic fixtures in memory. PostgreSQL and Entity Framework Core are the approved future persistence direction but are intentionally not connected in this phase.

## External provider boundary

The application depends on a neutral `IApparelVirtualTryOnGateway`. Provider-specific configuration and implementation remain in `TryOnReady.YouCam`. The scaffold implementation reports that the capability is unavailable; it cannot make network requests.

The reviewed live implementation will follow the provider's asynchronous workflow: obtain upload targets/file identifiers, upload the source and reference assets, create an AI Clothes task, then poll or receive a webhook until success or failure. The current AI Clothes v3 documentation identifies `/s2s/v2.0/file/cloth-v3` and `/s2s/v2.0/task/cloth-v3` as the relevant provider endpoints. Those endpoint details must remain isolated inside `TryOnReady.YouCam`.

The provider currently documents per-IP and per-token rate limits and recommends graceful backoff for HTTP 429 responses. Retry, timeout, idempotency, polling cadence, and unit-consumption controls are design requirements for the later adapter, not features of this scaffold.

## Future workflow

Boutique application → garment details and upload → readiness assessment → provider validation → admin review → consumer try-on page → aggregated activity.
