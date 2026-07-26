# Fly.io Deployment Runbook

TryOnReady uses one ASP.NET Core container. The container build compiles and
exports the Next.js frontend, copies it into the API's `wwwroot`, and serves the
site and API from one origin.

This document describes the intended deployment. Commands are marked verified
only after they are executed successfully and the public smoke test passes.

## Required runtime configuration

```text
ASPNETCORE_URLS=http://0.0.0.0:8080
Persistence__Provider=Postgres
ConnectionStrings__TryOnReady=<Fly PostgreSQL connection>
PrivateStorage__RootPath=/data/private
YouCam__Enabled=true
YouCam__ApiKey=<rotated server-only key>
```

The provider key and database connection string must be Fly secrets, never
`fly.toml` values.

## Deployment sequence

1. Authenticate the CLI with `fly auth login`.
2. Create or select the dedicated TryOnReady Fly application.
3. Provision a dedicated PostgreSQL service/database for TryOnReady.
4. Create a Fly volume mounted at `/data` for private, short-lived image assets.
5. Set `ConnectionStrings__TryOnReady`, `YouCam__Enabled`, and
   `YouCam__ApiKey` with `fly secrets set`.
6. Deploy the saved source state with `fly deploy`.
7. Confirm `/health`, `/api`, the four workflow pages, and one simulated smoke
   path.
8. Run one controlled live YouCam task and compare unit accounting.
9. Put the final HTTPS URL in the judge instructions and demo script.

## Rollback and safety

- A failed deployment must not fall back to another application's database.
- Migrations run only against `ConnectionStrings__TryOnReady`.
- The application fails closed when the production connection string or YouCam
  key is missing.
- Rolling back the app image does not remove the database or `/data` volume.

