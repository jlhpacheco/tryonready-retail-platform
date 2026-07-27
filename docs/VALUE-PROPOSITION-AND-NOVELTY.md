# TryOnReady Value Proposition and Novelty

## The One-Sentence Position

> **Virtual try-on that a small boutique can actually run.**

TryOnReady is a retailer-paid, guest-friendly operating and trust layer around YouCam Apparel Virtual Try-On. A boutique prepares and approves a garment once; shoppers privately try it without an account; the server controls valid requests and API units; and the retailer learns which garments attract aggregate interest without seeing shopper photos.

## Who Gets What

### Boutique owner

- A guided application and garment setup flow.
- A readiness check before a provider task can be created.
- A reviewed, publishable product checkpoint.
- Provider-unit and duplicate-request visibility.
- Aggregate results without customer photographs.
- A modern shopper experience without building an API integration.

### Guest shopper

- No account and no fee.
- A clear approved-garment catalog.
- Explicit consent before image processing.
- A private photo boundary.
- A generated visualization and next step.
- A plain warning that VTO does not guarantee physical fit or sizing.

### TryOnReady administrator

- A deliberate boutique and garment approval queue.
- Readiness and catalog context before publication.
- The ability to request changes or decline.
- No access to consumer images or secrets.

## Why This Is Not a Raw API Wrapper

| Raw Apparel VTO capability | Missing retailer operation | TryOnReady innovation |
|---|---|---|
| Accept garment and person images | Are the images suitable and authorized? | Server-side garment-readiness gate and rights confirmation |
| Generate a result | Is the garment safe to publish? | Human boutique and garment approval |
| Process a person image | Did the shopper knowingly consent, and who can view it? | Explicit consent, private storage, no retailer photo access |
| Create asynchronous provider tasks | What prevents repeat clicks from spending again? | Deterministic duplicate fingerprint and concurrency-safe reuse |
| Deduct provider units | Can the retailer see controlled usage? | Reserved/consumed unit accounting and aggregate dashboard |
| Return a time-limited result URL | How is the result retrieved and served safely? | Server retrieval, trusted-host checks, private local asset, no-store proxy |
| Expose API documentation | Can a three-person boutique operate it continuously? | One guided retailer → readiness → admin → guest → result workflow |

The creative idea is not a new image generator. It is translating enterprise-grade VTO into a small-retailer service with the controls that make each generation publishable, private, measurable, and cost-aware.

## The Immediate Product Promise

### Prepare and approve once

The boutique supplies catalog details and an authorized garment image. TryOnReady validates the file and a human administrator approves it before shoppers can select it.

### Let shoppers try privately

The guest selects an approved garment, reads the privacy boundary, consents, and supplies an authorized photo. No consumer account is required. The retailer never receives source or generated shopper photographs.

### Pay for controlled, valid requests

TryOnReady blocks unchanged duplicate submissions from creating a second provider task and tracks reserved and consumed units. Readiness checks occur before a consumer request can reach the provider.

### Learn without watching the shopper

The retailer sees aggregate try-on counts, outcomes, duplicates, and units. Lightweight explicit signals—save garment, visit product page, ask the boutique, request an in-store fitting—are the right next measurements. Customer accounts and persistent wishlists remain out of the lean MVP.

## What Is Working and What Is Pending

### Verified in the repository as of July 26, 2026

- ASP.NET Core server-side YouCam AI Clothes v3 adapter.
- File reservation, signed upload, task creation, polling, result retrieval, and trusted-host validation.
- Private image storage outside the public web root.
- Boutique application, garment readiness, admin approval, guest consent, result, and dashboard workflow.
- Duplicate-request protection and API-unit accounting.
- Automatic consumer-source deletion after terminal processing and configurable result retention.
- Retailer aggregate dashboard without customer photographs.
- Synthetic Luna & Thread catalog and fictional customer assets.
- Release build with zero warnings/errors, 10 .NET tests, and Playwright continuous-flow tests in simulation and isolated PostgreSQL modes, according to `docs/IMPLEMENTATION-LOG.md`.

### Pending and therefore not claimable as complete

- One controlled live YouCam Apparel VTO task reaching success.
- Reconciliation of that live task with the TryOnReady unit record.
- A public Fly.io judge URL passing the smoke flow.
- Real retailer or shopper pilot outcomes.
- Measured willingness to pay.
- Measured conversion, return, or purchase-interest impact.
- Confirmed provider-resource deletion unless the deletion endpoint template is configured and evidenced.

## Judge-Facing Novelty Statement

Use this wording:

> Most VTO demos begin with two images and end with a rendering. TryOnReady begins earlier and ends later. It checks whether a small retailer’s garment is ready, requires a publishable human approval, protects a guest’s photo and consent, prevents duplicate provider spend, records API units, and gives the boutique useful totals without customer imagery. The YouCam result is the engine; TryOnReady is the operating system that makes it usable for an independent boutique.

## Claim-Safe Product Copy

### Hero

**Eyebrow:** Built for independent boutiques  
**Headline:** Virtual try-on that a small boutique can actually run.  
**Body:** Prepare and approve each garment once. Let shoppers try it privately without an account. Control valid provider requests and learn which garments attract interest—without seeing customer photos.

**Primary action:** Start the Luna & Thread demo  
**Secondary action:** See the five controls

### Five-control strip

1. **Ready before paid use** — Check the garment image before a shopper request can consume a provider unit.
2. **Approved before publish** — A person reviews the boutique and garment before guest access.
3. **Private by design** — Guests consent; retailers never see source or generated shopper photos.
4. **Spend under control** — Unchanged duplicate requests reuse the prior job and units are tracked.
5. **Signals, not surveillance** — The boutique sees aggregate engagement and outcomes, not private imagery.

### Retailer value block

**Headline:** Offer the experience. Keep the operation small.  
**Body:** TryOnReady handles the secure YouCam workflow, provider status, private results, duplicate protection, and unit tracking. Your team prepares the catalog and learns from aggregate interest.

### Shopper value block

**Headline:** Try the garment, not another account.  
**Body:** Choose an approved garment, consent to one private try-on, and view the result. The boutique pays for the service and never receives your photo.

### Hypothesis block

**Headline:** A pilot will test the business value.  
**Body:** TryOnReady will measure completed try-ons, result reliability, garment-interest actions, retailer effort, controlled units, and privacy compliance. Conversion and return improvements are hypotheses—not promises.

## Language Guardrails

| Avoid | Use instead |
|---|---|
| “Guaranteed fit” | “Preview how the garment may look on your photo” |
| “Accurate sizing” | “Visual context; use the retailer’s size guidance for measurements” |
| “Reduces returns” | “Test whether visualization reduces expectation mismatch or return intent” |
| “Increases conversion” | “Measure product-page visits and purchase-interest signals after try-on” |
| “Zero wasted units” | “Readiness and duplicate controls prevent some avoidable requests” |
| “Enterprise VTO for every boutique” | “A lean operating layer designed for small clothing retailers” |
| “Anonymous” | “No customer account required; image processing still occurs with consent” |
| “Images are immediately deleted everywhere” | “TryOnReady deletes the consumer source after terminal processing and follows documented retention; provider retention is governed separately” |
| “Live YouCam integration proven” | Use only after a controlled live task succeeds and is recorded |

## Why the Retailer Pays

The shopper receives TryOnReady as a courtesy. The retailer pays because the product could replace five separate burdens:

1. secure API integration;
2. garment-input operations;
3. customer consent and image handling;
4. usage and duplicate controls; and
5. aggregate engagement measurement.

This is a willingness-to-pay hypothesis. The pilot should determine whether the buyer prefers a platform fee, included-unit bundle, per-completed-result fee, or campaign pilot. Billing does not belong in the hackathon MVP.

## Competitive Framing

TryOnReady does not need to beat Google or Walmart as a shopping destination. It helps a boutique preserve its own identity and customer relationship while offering one capability those large platforms have normalized.

| Alternative | Advantage | Gap for the target boutique |
|---|---|---|
| Build directly on the YouCam API | Maximum control | Requires secure backend, async jobs, image policy, catalog governance, monitoring, and support |
| Send shoppers to a large marketplace | Immediate scaled experience | The boutique loses context, merchandising control, and direct learning |
| Use static product photography only | Low operational burden | Shopper must imagine the garment on themselves |
| TryOnReady | Guided small-retailer operation and private guest flow | Must still prove live reliability, willingness to pay, and pilot impact |

## Evidence a Judge Should See

1. Hero line and uninterrupted four-step workflow.
2. Moonlight Blazer image passing readiness before approval.
3. Admin publishing checkpoint.
4. Guest consent and “API key in browser: Never.”
5. A controlled live `YouCamLive` result—not simulation—in the final submission video.
6. An immediate unchanged retry showing “Duplicate request prevented.”
7. Dashboard counts and units without shopper photographs.
8. A concise pilot invitation with the exact outcomes to validate.

## Repository Evidence Paths

- Landing and workflow: `apps/web/app/page.tsx`
- Garment readiness: `src/TryOnReady.Application/Readiness/ProductReadinessService.cs`
- Approval workflow: `src/TryOnReady.Infrastructure/Persistence/PersistentAdminReviewService.cs`
- Consent and duplicate protection: `src/TryOnReady.Infrastructure/Persistence/PersistentTryOnService.cs`
- Provider integration: `src/TryOnReady.YouCam/YouCamApparelVirtualTryOnGateway.cs`
- Background processing and deletion: `src/TryOnReady.Infrastructure/Processing/TryOnJobProcessor.cs`
- Private storage: `src/TryOnReady.Infrastructure/Storage/FileSystemPrivateAssetStore.cs`
- API journey: `tests/TryOnReady.IntegrationTests/CompleteJourneyTests.cs`
- Browser journey: `apps/web/e2e/smoke.spec.ts`
- Working/pending ledger: `docs/IMPLEMENTATION-LOG.md`

