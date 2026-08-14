# Codex Carry Forward

## Project boundary

TryOnReady is an independent, public source-available hackathon project for small clothing boutiques. The primary direction is YouCam Apparel Virtual Try-On, and the repository contains the complete judge-ready MVP and deployment documentation.

## Permanent guardrails

1. Keep TryOnReady completely independent of Duevara.
2. Do not use employer source code, work data, customer information, internal systems, or credentials.
3. Use synthetic, self-owned, or explicitly licensed demo assets only.
4. Do not commit customer photographs, real boutique/customer information, API keys, or other secrets.
5. Keep provider credentials server-side.
6. Keep the narrow source-available hackathon judging license; do not broaden it into a general open-source license without explicit instruction.
7. Keep the public repository, deployment, and judge-facing links synchronized without exposing credentials or private shopper data.
8. Do not claim physical fit, sizing accuracy, medical findings, diagnostic conclusions, or completed integration.

## Approved technology

- Next.js, React, strict TypeScript, mobile-first responsive PWA baseline
- ASP.NET Core 10, C#, OpenAPI, hosted background worker
- PostgreSQL and Entity Framework Core as a later data direction
- xUnit, ASP.NET Core integration tests, frontend linting/type checking

## Current task

Create only the first buildable foundation: frontend shell and placeholder routes; API, application, domain, infrastructure, provider boundary, worker, unit tests, and integration tests; health and OpenAPI endpoints; safe options; synthetic Luna & Thread fixtures.

No live YouCam call, production database, native package, deployment, public visibility change, or full product implementation is authorized.

## Current implementation status

The first scaffold was completed and locally verified on July 25, 2026. The exact commands, results, dependency-security checks, file inventory, limitations, and recommended next task are recorded in `docs/SCAFFOLD-IMPLEMENTATION.md`.

Do not proceed into live YouCam integration until the scaffold has been reviewed. SaaS authentication, subscription plans, billing, and production PostgreSQL persistence remain deferred.
