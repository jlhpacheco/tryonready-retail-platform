# TryOnReady

**Virtual try-on for boutiques without the enterprise budget.**

TryOnReady is a mobile-first retail enablement platform for independent clothing boutiques, especially small owner-operated stores working with limited staff, technical capacity, and margins.

The hackathon prototype demonstrates how a boutique can:

1. Apply from a phone.
2. Add a garment and product details.
3. Receive practical image-readiness feedback.
4. Validate the item through the YouCam Apparel Virtual Try-On API.
5. Submit the item for approval.
6. Publish a customer-ready virtual try-on experience.
7. Review aggregated product activity without accessing customer photographs.

## Hackathon status

This repository is a private, standalone project for the YouCam API Skin AI & Apparel VTO Hackathon. It is not an extension of Duevara and contains no Duevara source code, architecture, data, credentials, branding, or business rules.

## Intended stack

- Next.js, React, and TypeScript mobile-first PWA
- ASP.NET Core 10 Web API
- PostgreSQL with Entity Framework Core
- .NET background worker
- YouCam Apparel VTO REST API
- Private object storage
- xUnit and Playwright tests

## Required boundaries

- Synthetic, self-owned, or explicitly licensed demo assets only
- No real boutique, customer, employer, or production information
- No customer face or body photographs committed to Git
- No API credentials committed to Git
- The YouCam secret remains server-side
- No open-source license unless the owner expressly approves one

Read these files before making code changes:

- `HACKATHON-SCOPE.md`
- `IP-AND-LICENSE-BOUNDARY.md`
- `DATA-AND-IMAGE-SAFETY.md`
- `SECURITY.md`
- `docs/ARCHITECTURE.md`
- `docs/PRODUCT-SCOPE.md`
- `CODEX-CARRYFORWARD.md`

## Ownership

All original TryOnReady materials in this repository are intended to remain proprietary to the repository owner unless a later written agreement states otherwise.
