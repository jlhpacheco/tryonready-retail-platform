# TryOnReady Judge and Small-Business Guide

## Judge narrative pack

- [Market validation](MARKET-VALIDATION.md)
- [Value proposition and novelty](VALUE-PROPOSITION-AND-NOVELTY.md)
- [Judge evaluation map](JUDGE-EVALUATION-MAP.md)
- [Final video narrative](VIDEO-NARRATIVE.md)
- [Pilot metrics](PILOT-METRICS.md)

## The one-sentence story

Elena Rivera submits her fictional boutique, Luna & Thread; adds an authorized
synthetic garment; an administrator approves both; a guest customer selects
the garment, consents, uploads an authorized synthetic photo; the secure server
creates and tracks the virtual try-on; and the customer sees the result without
an API key ever reaching the browser.

## Demo roles and credentials

| Role | Username | Password | What the role does |
|---|---|---|---|
| Retailer | `retailer@tryonready.demo` | `RetailerDemo!2026` | Submits Luna & Thread, adds products, and views aggregate results |
| Administrator | `admin@tryonready.demo` | `AdminDemo!2026` | Reviews the boutique and garment before consumer access |
| Guest customer | No sign-in | No password | Chooses an approved garment, consents, selects a photo, and views the result |

These are demonstration credentials, not production accounts. The YouCam API
key is a separate server secret and is never included in this guide, the
browser, or GitHub.

## Start the local demo

1. Start the `TryOnReady.Api` project with the **https** Visual Studio profile.
2. Open <https://localhost:7090/>.
3. If the local certificate warning appears, trust the ASP.NET development
   certificate on this development computer.
4. Select **Judge Sign In**.

The HTTP profile is also available at <http://localhost:5090/>.

## Complete use-case journey

### 1. Retailer submits the boutique

1. On **Judge Sign In**, choose **Retailer**.
2. Enter the retailer username and password exactly as shown above.
3. Select **Continue as Retailer**.
4. On **Boutique Application**, leave the prepared fictional values:
   - Boutique name: Luna & Thread
   - Owner: Elena Rivera
   - Business email: `elena@luna-thread.example.invalid`
   - Employees: 3
   - Primary sales channel: Physical store
5. Check the statement confirming image rights.
6. Select **Submit application**.
7. Confirm the message **Application submitted**.

Expected result: the application is stored in the isolated TryOnReady
PostgreSQL database with a submitted status.

### 2. Retailer adds the Moonlight Blazer

1. Select **Continue to Product Readiness**.
2. Keep the prepared product values:
   - Garment name: Moonlight Blazer
   - Product number: `SYN-BLZ-001`
   - Category: Top
   - Brand: Luna & Thread
   - Color: Terracotta
   - Material: Cotton-blend suiting
   - Size range: XS-XL
   - Price: USD 89.00
3. Choose
   `samples/synthetic/garments/moonlight-blazer.png`.
4. Confirm the screen reads the file as 1254 x 1254 pixels.
5. Select **Save garment and check image**.
6. Confirm that the image is ready and the product is waiting for
   administrator approval.

Expected result: TryOnReady validates the file before any provider unit can be
used and stores it outside the public web root.

### 3. Administrator approves the boutique and garment

1. Return to **Judge Sign In**.
2. Choose **Administrator**.
3. Enter the administrator username and password exactly as shown above.
4. Select **Continue as Administrator**.
5. In **Boutique applications**, select **Approve boutique** for Luna & Thread.
6. Confirm **Boutique application saved: Approved.**
7. In **Product reviews**, inspect the Moonlight Blazer image and metadata.
8. Select **Approve product**.
9. Confirm **Product review saved: Approved.**
10. Select **Continue to Consumer Try-On**.

Expected result: the product becomes visible in the guest catalog only after
both readiness and human approval.

### 4. Guest customer requests a virtual try-on

1. No customer sign-in is required.
2. Confirm **Moonlight Blazer** is the selected approved garment.
3. Confirm the page states **API key in browser: Never**.
4. Choose
   `samples/synthetic/customers/marisol-lopez-source.png`.
5. Check the consent statement.
6. Select **Generate virtual try-on** once.
7. Watch the status move through submission and processing.
8. Confirm the result reaches **Succeeded** and a generated image appears.

Expected result: the ASP.NET server—not the browser—uploads the authorized
images, creates the YouCam task, polls its status, and returns the generated
result. The boutique does not receive the source or generated customer image.

### 5. Verify duplicate protection and retailer results

1. Select **Generate virtual try-on** again without changing the garment or
   customer image.
2. Confirm **Duplicate request prevented** appears.
3. Select **View the retailer results dashboard**.
4. Confirm the dashboard reports the completed try-on and a stopped duplicate.

Expected result: the second click reuses the prior request rather than creating
another provider task or spending another API unit.

## Realistic Luna & Thread mini catalog

| Product | Product number | Category | Color | Material | Sizes | Price | Upload file |
|---|---|---|---|---|---|---:|---|
| Moonlight Blazer | `SYN-BLZ-001` | Top | Terracotta | Cotton-blend suiting | XS-XL | USD 89 | `samples/synthetic/garments/moonlight-blazer.png` |
| Harbor Sage Blouse | `SYN-TOP-002` | Top | Sage | Cotton-linen blend | XS-XXL | USD 64 | `samples/synthetic/garments/harbor-sage-blouse.png` |
| Midnight Wrap Dress | `SYN-DRS-003` | Full-body outfit | Deep navy | Woven rayon blend | XS-XL | USD 118 | `samples/synthetic/garments/midnight-wrap-dress.png` |

Exact hashes, dimensions, prompt summaries, and permitted uses are in
`samples/synthetic/ASSET-METADATA.md`.

## Fictional customer choices

- Marisol Lopez:
  `samples/synthetic/customers/marisol-lopez-source.png`
- Danielle Smith:
  `samples/synthetic/customers/danielle-smith-source.png`

Both are fictional adults generated specifically for this private demo. They
are not real people and must not be represented as real customers.

## What is verified

As of July 26, 2026:

- Release build: 0 warnings, 0 errors
- .NET unit tests: 2 passed
- .NET integration tests: 8 passed
- Frontend lint: passed
- TypeScript check: passed
- Playwright: 3 passed
- Desktop continuous journey: passed in provider simulation mode
- Mobile layout and sign-in: passed
- PostgreSQL-backed continuous journey: passed
- Duplicate-request protection: passed
- Browser API-key exposure check: false
- YouCam account balance screenshot: 1,040 bonus units
- Isolated local PostgreSQL: `tryonready-postgres` on port `55432`

The simulated provider verifies application behavior without spending an API
unit. The controlled live YouCam test must use the same authorized synthetic
assets, be submitted once, and then be recorded in
`docs/IMPLEMENTATION-LOG.md`.

## Security and privacy facts

- The YouCam API key is stored only in server-side secrets.
- No API key, redemption code, or signed upload URL is committed to GitHub.
- Consumer images are stored privately outside the public web root.
- Source and generated consumer images follow the deletion schedule in
  `docs/DATA-RETENTION.md`.
- Retailers see totals and processing outcomes, not customer photographs.
- The TryOnReady database is isolated from every other local database.
- The existing Duevara and CarPermit PostgreSQL container is out of scope and
  must never be changed by TryOnReady commands.

## Fast troubleshooting

- **The form says it cannot load applications:** sign in as Retailer again.
- **Admin queues do not load:** sign in as Administrator.
- **No garment appears for the guest:** approve both the boutique and product.
- **Image is rejected:** use one of the repository synthetic files without
  editing or resizing it.
- **Try-on stays in processing:** confirm the API is running and review its
  server log for a provider status or documented error code.
- **Live provider is disabled:** complete the simulation journey first, then
  follow `docs/YOUCAM-SETUP.md`.

## Scope after the MVP

The retailer pays for the service and offers it to guests as a courtesy and
convenience. A later phase may let a customer create an optional account, save
favorite garments, signal purchase interest, and return to prior choices. That
account and subscription expansion is intentionally outside this lean
hackathon MVP.
