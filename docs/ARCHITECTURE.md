# Architecture

TryOnReady uses one ASP.NET Core 10 host for the exported TypeScript frontend,
HTTP API, background task processor, and private image delivery.

```text
Next.js / TypeScript browser
        |
ASP.NET Core API and cookie authorization
        |
Application workflows and ports
   /                    \
Domain              Infrastructure
                         |
            EF Core 10 + PostgreSQL
                         |
               Private file storage
                         |
            YouCam adapter or simulation
```

## Backend responsibilities

- `TryOnReady.Domain`: lean entities and workflow state.
- `TryOnReady.Application`: catalog, try-on, storage, and provider ports.
- `TryOnReady.Infrastructure`: EF Core repositories, private file storage,
  duplicate protection, background processing, cleanup, and dashboard queries.
- `TryOnReady.YouCam`: AI Clothes v3 file reservation, signed upload, task
  creation, polling, result retrieval, simulation, and disabled modes.
- `TryOnReady.Api`: authentication, authorization, multipart boundaries, image
  validation, health checks, OpenAPI, and static frontend hosting.
- `TryOnReady.Worker`: the same processing services available as a separate
  composition root if deployment later separates background work.

Dependencies point inward toward the domain. The API and worker compose
implementations at startup.

## Browser and authorization boundary

The browser contains no provider credential. Retailer and administrator actions
use an HTTP-only, SameSite cookie and distinct role policies. Guest catalog,
consent, submission, polling, and result routes are public for the lean MVP.
Unapproved garment images return not found to unauthenticated requests.

The Next.js application is statically exported during the API build. ASP.NET
serves the UI and API from one origin, which keeps cookies and deployment
configuration simple.

## Persistence and storage

EF Core 10 persists boutique applications, product catalog records, approval
state, try-on jobs, duplicate counts, task references, and API-unit accounting.
Local development uses only `tryonready-postgres` on host port `55432`.

Garment, consumer, and result images are stored outside `wwwroot`. The database
stores opaque asset identifiers and metadata, not image bytes. Consumer inputs
and generated results follow `DATA-RETENTION.md`.

## Provider workflow

`IApparelVirtualTryOnGateway` keeps provider details outside the application
workflow. The live YouCam adapter:

1. reserves person and garment file identifiers;
2. uploads both files to signed HTTPS targets;
3. creates an AI Clothes v3 task;
4. polls the provider task endpoint until success or failure; and
5. retrieves the generated result through the secure server.

A deterministic simulation implements the same interface for repeatable tests
without spending API units. Request fingerprints return an existing job for an
unchanged product/person pair, preventing duplicate provider tasks.

## Deployment

The `Dockerfile` builds the Next.js export and .NET host. `fly.toml` runs the
single container on Fly.io. Production PostgreSQL, the YouCam key, role
passwords, and connection strings are runtime secrets and never repository
configuration.
