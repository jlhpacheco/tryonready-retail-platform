# Codex Carry-Forward

## Repository

`jlhpacheco/tryonready-retail-platform`

## Project

**TryOnReady — Virtual try-on for boutiques without the enterprise budget.**

## Required reading

Before changing code, read:

1. `README.md`
2. `HACKATHON-SCOPE.md`
3. `IP-AND-LICENSE-BOUNDARY.md`
4. `DATA-AND-IMAGE-SAFETY.md`
5. `SECURITY.md`
6. `docs/ARCHITECTURE.md`
7. `docs/PRODUCT-SCOPE.md`
8. `docs/DEMO-SCRIPT.md`
9. `THIRD-PARTY-NOTICES.md`

## Non-negotiable constraints

- Keep TryOnReady independent of Duevara.
- Do not copy or reference Duevara source, data, architecture, credentials, branding, or business rules.
- Use synthetic, self-owned, or explicitly licensed demonstration assets only.
- Do not commit real customer or boutique data.
- Do not commit face or body photographs.
- Never commit API keys, database passwords, storage credentials, or signing secrets.
- Keep the YouCam credential server-side.
- Do not add an open-source license.
- Do not make physical-fit, sizing, medical, or diagnostic claims.
- Stop and report any requested change that conflicts with these rules.

## First implementation task

Create the smallest safe, buildable scaffold for:

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

The first task should include:

- Next.js TypeScript PWA shell
- ASP.NET Core 10 solution and projects
- Neutral YouCam adapter interfaces, without real credentials
- Synthetic boutique and product fixtures
- Health endpoint
- Basic OpenAPI support
- Unit-test baseline
- Local run instructions
- Safe environment-variable configuration

## Verification required on every task

1. Make the smallest safe change.
2. List every changed file.
3. Build the .NET solution.
4. Run relevant .NET tests.
5. Run frontend lint and tests when applicable.
6. Report failures honestly.
7. Do not claim success without command output.
8. Do not deploy or make the repository public.

## Preferred implementation order

1. Scaffold and build verification
2. Domain model and synthetic data
3. Boutique application vertical slice
4. Product and garment upload metadata
5. Readiness checklist
6. YouCam adapter and job lifecycle
7. Admin review
8. Consumer try-on page
9. Aggregated boutique dashboard
10. PWA and mobile validation
11. End-to-end demo hardening

## Definition of done for the hackathon

One polished vertical slice works end to end on desktop and a phone-sized browser, uses the real YouCam Apparel VTO API, protects credentials and images, has repeatable setup instructions, passes relevant tests, and can be demonstrated in under three minutes.
