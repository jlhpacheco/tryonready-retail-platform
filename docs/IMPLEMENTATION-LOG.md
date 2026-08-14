# Implementation and Verification Log

This is the append-only human-readable record for the real vertical-slice phase.
It must distinguish implemented code, automated simulation, and controlled live
provider evidence.

## July 26, 2026 — governing brief confirmed

Confirmed priorities:

- real server-side YouCam AI Clothes v3 flow before SaaS or decorative work
- isolated PostgreSQL with EF Core 10 and .NET 10
- private file storage and deletion rules
- continuous boutique → garment → admin → consumer → result journey
- duplicate-request protection and API-unit tracking
- Fly.io deployment and judge handoff

Security decisions:

- the API key previously posted in conversation is treated as exposed and must
  be rotated
- secrets and redemption codes are excluded from source and documentation
- the existing unrelated PostgreSQL container is a protected boundary and will
  not be modified
- YouCam browser setup was not changed because the local Codex browser-control
  connection failed before reaching the account

Repository state before this phase:

- branch `main` matched `origin/main`
- GitHub CLI was installed and authenticated as `jlhpacheco`
- at that checkpoint, the Git remote pointed to the private TryOnReady repository; the repository became public and source-available for judging on August 14, 2026
- the application used in-memory persistence and a disabled provider adapter

Official provider contract confirmed:

- file reservation: `POST /s2s/v2.0/file/cloth-v3`
- signed upload: provider-returned HTTPS `PUT`
- task creation: `POST /s2s/v2.0/task/cloth-v3`
- status/result polling: `GET /s2s/v2.0/task/cloth-v3/{task_id}`
- server-side Bearer authentication
- documented image size limit below 10 MB

Implementation and verification results will be added below as they are
completed. No live provider call has been made at this point.

## July 26, 2026 — first compile checkpoint

The first incremental build correctly stopped on two dependency issues:

- EF Core 10.0.10 conflicted with the stable Npgsql 10.0.3 provider's EF Core
  10.0.4 assembly alignment. The EF packages were pinned to 10.0.4 while
  retaining the .NET/EF 10 requirement.
- ImageSharp 4.0.0 required a separate Six Labors build license. It was removed
  before use and replaced with 3.1.12 for the narrow image-identification
  boundary. This dependency remains subject to its split license and is not a
  blanket approval for commercial production.

The errors were discovered before a database was started or a YouCam request
was made.

## July 26, 2026 — server-side vertical slice implemented

Implemented:

- EF Core 10 persistence for boutique applications, products, and try-on jobs
- initial migration `20260726163357_InitialVerticalSlice`
- isolated PostgreSQL Compose service `tryonready-postgres` on
  `127.0.0.1:55432`
- private file-system storage outside the public web root
- server-side image identification and readiness validation
- YouCam AI Clothes v3 reservation, signed upload, task, polling, and result
  adapter
- deterministic provider simulation for automated testing
- duplicate fingerprinting and API-unit accounting
- retailer, administrator, and guest role journey
- retailer aggregate dashboard without consumer photographs
- automatic cleanup rules for consumer input and result assets

The protected unrelated PostgreSQL container was not stopped, restarted,
queried, migrated, or modified.

## July 26, 2026 — account and secret verification

- The hackathon YouCam account screenshot showed 1,040 bonus units.
- A replacement API key was generated after the previously disclosed key was
  treated as compromised.
- The replacement key is stored only in Visual Studio User Secrets.
- A local status check confirmed provider mode `YouCamLive`,
  `LiveYouCamIntegration=true`, and `ApiKeyExposedToBrowser=false`.
- The key value was not printed or copied into the repository.
- No controlled live provider task had been submitted at this checkpoint.

## July 26, 2026 — verified browser journey

Release and static verification:

- integrated Release build: 0 warnings, 0 errors
- .NET unit tests: 2 passed
- .NET integration tests: 8 passed
- ESLint: passed
- TypeScript: passed

Playwright verified the same continuous path:

1. retailer signs in;
2. Luna & Thread is submitted;
3. the Moonlight Blazer is uploaded and passes readiness;
4. the administrator approves the boutique and product;
5. a guest consents and uploads Marisol Lopez's authorized synthetic image;
6. the provider simulation completes and returns a result;
7. an unchanged resubmission is stopped as a duplicate; and
8. the dashboard reports completed work without customer photographs.

The suite passed twice:

- 3 of 3 tests against isolated in-memory persistence
- 3 of 3 tests against `tryonready-postgres`

The PostgreSQL-backed evidence after iterative test runs was:

- 3 boutique application rows
- 3 product rows
- 2 completed simulated try-on job rows
- 2 prevented duplicate submissions
- 0 provider units consumed

The temporary PostgreSQL-backed API test process was stopped after the run.
The `tryonready-postgres` container remained healthy.

## July 26, 2026 — synthetic judge pack and guides

Added:

- Moonlight Blazer (`SYN-BLZ-001`)
- Harbor Sage Blouse (`SYN-TOP-002`)
- Midnight Wrap Dress (`SYN-DRS-003`)
- Marisol Lopez fictional adult source image
- Danielle Smith fictional adult source image

Every asset has exact dimensions, byte length, SHA-256, prompt summary,
fictional-person notice, and permitted demo purpose in
`samples/synthetic/ASSET-METADATA.md`.

The simple-English judge journey was published as Markdown, Word, and PDF. The
final ten-page PDF was rendered and every page was visually inspected for
clipping, overflow, numbering, image placement, and readability.

## July 26, 2026 — market and judge narrative strengthened

Added:

- source-backed U.S. small clothing-retailer proxy and pilot-market range
- value proposition and operating-layer novelty statement
- criterion-by-criterion Devpost judge evidence map
- claim-safe 1–3 minute live-demo script and shot list
- pilot definitions for activation, readiness, approval, try-on completion,
  duplicate units avoided, consent/drop-off, garment interest, and deletion
- landing-page copy led by “Virtual try-on that a small boutique can actually
  run”

Fresh verification after the copy and documentation changes:

- Next.js production build: passed
- .NET Release build: 0 warnings, 0 errors
- .NET unit tests: 2 passed
- .NET integration tests: 8 passed
- ESLint: passed
- TypeScript: passed
- Playwright in-memory journey: 3 passed

One parallel verification attempt created local simulation-resource
interference and was not counted; the .NET and Playwright suites both passed
when rerun independently.

No YouCam task was submitted, no provider unit was consumed, and no database
or Docker container was accessed during this documentation phase. A controlled
live YouCam completion and the public Fly.io judge URL remain pending.
