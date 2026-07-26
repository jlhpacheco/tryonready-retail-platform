# Governing Product Brief

Last confirmed: July 26, 2026

This document is the authoritative implementation priority for TryOnReady.
When an older document describes a scaffold or a future phase, this brief takes
precedence.

## Demonstration outcome

TryOnReady is sold to independent clothing retailers. The retailer pays for
the service and offers virtual try-on to its shoppers at no charge as a
courtesy and convenience. The MVP tracks operational use and provider units
for the retailer; it does not implement subscriptions or payment processing.

After a successful try-on, the retail conversion path is: save the garment,
view the retailer's product page, or ask the boutique about purchase or an
in-store fitting. The judged MVP may demonstrate these calls to action and
aggregate interest counts. Persistent customer accounts, wishlists, inventory
integration, and checkout are a documented follow-up, not requirements for the
YouCam vertical slice. A retailer receives identifiable customer interest only
after separate, explicit customer opt-in and never receives the customer's
source or generated image.

The hackathon demonstration must be one continuous, understandable journey:

1. A small boutique submits an application.
2. The boutique adds a garment, supplies catalog information, and uploads an
   authorized garment image.
3. TryOnReady checks the image before a paid provider request.
4. An administrator reviews and approves the boutique and garment.
5. A consumer chooses an approved garment, reads the privacy notice, gives
   explicit consent, and selects an authorized person image.
6. The ASP.NET server uploads both images to YouCam, creates an AI Clothes v3
   task, polls its status, retrieves the completed image, and presents the
   result to the consumer.
7. The retailer sees a small operational dashboard with counts and API-unit
   use, but never sees consumer source or generated images.

The YouCam API key must never reach browser code, a generated frontend bundle,
Git history, Markdown documentation, screenshots, test output, or application
logs.

## Governing implementation order

1. Implement the real server-side YouCam Apparel VTO vertical slice:
   secure upload, task creation, status polling, result retrieval,
   duplicate-request protection, and API-unit tracking.
2. Persist the workflow in a dedicated TryOnReady PostgreSQL database using EF
   Core 10 and the .NET 10 runtime.
3. Store images outside the public web root, define deletion rules, and proxy
   only the approved garment/result assets that the current page needs.
4. Connect the existing boutique, readiness, admin, and consumer pages into the
   continuous journey.
5. Add missing retailer catalog fields and a small results dashboard.
6. Stabilize Playwright and prove one complete simulated path before spending an
   API unit on one controlled live test.
7. Deploy the combined ASP.NET/Next.js container to Fly.io, configure secrets,
   and give judges a working URL.
8. Prepare the 1–3 minute video, testing instructions, and private-repository
   access.

SaaS subscriptions, billing, multi-tenant production administration, and
decorative pages are explicitly below this work in priority.

## Provider contract

TryOnReady targets the official YouCam AI Clothes v3 server-to-server workflow:

- reserve upload targets with `POST /s2s/v2.0/file/cloth-v3`
- upload image bytes only to the returned signed URLs
- create the generation task with `POST /s2s/v2.0/task/cloth-v3`
- poll with `GET /s2s/v2.0/task/cloth-v3/{task_id}`
- retrieve the generated result only after `task_status` becomes `success`

The API key is sent as a Bearer token by the server-side YouCam adapter only.

Official references:

- [AI Clothes Virtual Try-On integration guide](https://docs.perfectcorp.com/reference/ai_clothes/section/overview/integration-guide)
- [YouCam API quick start](https://docs.perfectcorp.com/develop/quick_start_guide)
- [YouCam API FAQ](https://docs.perfectcorp.com/develop/faq)
- [YouCam API release notes](https://docs.perfectcorp.com/release/changelog)

## Definition of done

The vertical slice is complete only when all of these statements are true:

- data survives an API restart
- the database is uniquely named and isolated from every other local project
- a duplicate consumer submission cannot create a second paid YouCam task
- a consumer can poll the TryOnReady job without receiving a provider key or
  signed provider upload URL
- the completed image is served through TryOnReady, not exposed as a raw
  long-lived public object
- source images and results follow the documented deletion schedule
- API-unit totals are visible without exposing customer images
- automated tests pass in provider-simulation mode
- one deliberately authorized live test completes with its unit use recorded
- the Fly.io URL passes the same smoke path used locally
- documentation distinguishes simulated evidence from live-provider evidence
