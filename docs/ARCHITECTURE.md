# Architecture

## Product shape

TryOnReady is a mobile-first Progressive Web App with three experiences:

1. Boutique Partner Portal
2. Admin Review Console
3. Consumer Try-On Storefront

## Target stack

- Frontend: Next.js, React, TypeScript
- Mobile delivery: responsive PWA for iPhone, Android, tablets, and desktop
- Backend: ASP.NET Core 10 Web API
- Application layer: .NET use cases and validation
- Persistence: PostgreSQL with Entity Framework Core
- Background processing: .NET Worker Service
- External AI: YouCam Apparel VTO REST API
- Object storage: private S3-compatible or cloud blob storage
- Testing: xUnit, integration tests, and Playwright

## System flow

```text
Boutique or consumer browser
          |
          v
Next.js mobile-first PWA
          |
          v
ASP.NET Core API
   |        |        |
   v        v        v
PostgreSQL  Private   Job queue
            storage      |
                         v
                   .NET worker
                         |
                         v
                 YouCam Apparel VTO
```

## Security boundary

The browser communicates only with the TryOnReady API. The browser must never receive the YouCam API secret.

## Primary modules

### Boutique onboarding

- Application creation
- Business profile
- Application status
- Administrative review

### Catalog readiness

- Manual product entry
- Garment image upload
- Required-field validation
- Actionable readiness guidance

The readiness result evaluates the digital asset and catalog completeness. It does not promise physical fit or sizing accuracy.

### YouCam adapter

- Submit authorized source assets
- Start virtual try-on work
- Poll or retrieve job status
- Map provider responses into neutral internal contracts
- Handle timeout, failure, and retry states

Provider-specific details should remain inside `TryOnReady.YouCam`.

### Publication

- Administrative approval
- Public product page
- Consumer photo consent
- Virtual try-on request
- Private result display
- Share or QR link

### Analytics

- Aggregated product activity only
- No boutique access to consumer photographs
- Synthetic metrics in the hackathon demonstration

## Suggested solution structure

```text
apps/web/
src/TryOnReady.Api/
src/TryOnReady.Application/
src/TryOnReady.Domain/
src/TryOnReady.Infrastructure/
src/TryOnReady.YouCam/
src/TryOnReady.Worker/
tests/TryOnReady.UnitTests/
tests/TryOnReady.IntegrationTests/
tests/TryOnReady.EndToEndTests/
```

## Initial implementation rule

Start with a vertical slice that demonstrates one boutique, one garment, one approval, and one consumer try-on. Do not build broad marketplace infrastructure before the real YouCam path works end to end.
