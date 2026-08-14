# TryOnReady Judge Evaluation Map

## Executive Summary

- **Baseline viability is supported by recorded live proof.** The rules require a working application integrating at least one YouCam Fashion API. TryOnReady completed one controlled YouCam AI Clothes v3 task on July 28, 2026; the recorded evidence shows `YouCamLive`, terminal state `Succeeded`, the generated result, and one API unit. The public judge path replays that provenance-locked result without making a new provider request.
- **The strongest scoring story is the operating layer.** Every judging criterion is equally weighted. The demo should connect the same five controls—readiness, approval, consent/privacy, duplicate/unit control, and aggregate reporting—to technological implementation, design, impact, and idea quality.
- **The first minute must establish audience and novelty.** Say “three-person boutique,” show Luna & Thread, and explain that the YouCam result is the engine while TryOnReady makes it safe and operable for a small retailer.
- **Do not spend video time on stack recitation.** Show working evidence. Put .NET, PostgreSQL, Next.js, private storage, and tests in on-screen proof or submission text.

## Official Judging Structure

The [official Devpost rules](https://youcam-api.devpost.com/rules) use:

1. **Stage One pass/fail:** the project reasonably fits the theme and applies the required API.
2. **Stage Two, equally weighted:**
   - Technological Implementation
   - Design
   - Potential Impact
   - Quality of the Idea

The submission also requires a functional repository, text description, screenshots, a public 1–3 minute end-to-end video that explains the YouCam API used, and free judge access through the judging period.

## Baseline Pass/Fail Map

| Requirement | Judge-visible proof | Repository proof | Current state | Submission action |
|---|---|---|---|---|
| Working application | Public URL or functioning demo | Full ASP.NET/Next.js workflow | Public Fly judge URL deployed and smoke-tested | Keep the public URL available through judging |
| At least one YouCam Fashion API | `Provider mode: YouCamLive`, generated result, unit count | `YouCamApparelVirtualTryOnGateway.cs` uses AI Clothes v3 endpoints; controlled result provenance is documented | Completed controlled YouCamLive task: `Succeeded`, 1 API unit | Preserve the verified result and exact replay provenance |
| Clear retail value | Luna & Thread workflow and five controls | Landing page, forms, dashboard, docs | Strong | Lead with “small boutique can actually run” |
| Repository and instructions | Public source-available repo, README, judge guide | Public `main`, narrow judging license, README, and judge guide | Complete and anonymously reachable | Keep public links and instructions synchronized |
| End-to-end video | Application → garment → approval → consent → YouCam result → dashboard | Playwright covers the same path; public video shows the working experience | Complete: public 2:38 YouTube video | Keep the approved public video URL available through judging |

The public replay is deliberately labeled as a previously completed controlled demonstration. It is not represented as a new provider request performed during judge playback.

## Criterion 1: Technological Implementation

**Judge question:** How thoroughly and skillfully does the project integrate a YouCam API, demonstrate value, and show genuine non-trivial effort?

### Claim

TryOnReady implements the full server-to-server YouCam AI Clothes v3 lifecycle inside a protected retail workflow.

### Demonstrate

1. Garment image passes a local readiness check.
2. Admin approves the garment.
3. Guest consents and selects Marisol’s authorized synthetic photo.
4. Screen shows `Provider mode: YouCamLive`.
5. Result reaches `Succeeded`.
6. Screen shows the unit count.
7. Same request is submitted again and is stopped as a duplicate.

### Repository evidence

- `src/TryOnReady.YouCam/YouCamApparelVirtualTryOnGateway.cs`
  - reserves two files;
  - uploads only to trusted HTTPS hosts;
  - creates `cloth-v3` task;
  - polls task state;
  - bounds result downloads;
  - sends Bearer authorization server-side.
- `src/TryOnReady.Infrastructure/Persistence/PersistentTryOnService.cs`
  - requires consent;
  - creates a request fingerprint;
  - prevents duplicates, including concurrent insert races;
  - requires approved boutique and garment;
  - records reserved units.
- `src/TryOnReady.Infrastructure/Processing/TryOnJobProcessor.cs`
  - submits and polls background work;
  - records consumed units;
  - stores result privately;
  - deletes the consumer source after terminal processing;
  - attempts provider-resource deletion when configured.
- `tests/TryOnReady.IntegrationTests/CompleteJourneyTests.cs`
- `apps/web/e2e/smoke.spec.ts`

### Claim-safe qualifier

Automated evidence uses the deterministic provider simulation. Call the provider integration “live-proven” only after the controlled YouCam task is recorded in `docs/IMPLEMENTATION-LOG.md`.

## Criterion 2: Design

**Judge question:** Is this a complete, coherent product experience rather than a technical proof of concept?

### Claim

One continuous workflow makes the value understandable to a first-time boutique owner, administrator, shopper, or judge.

### Demonstrate

- Landing page states the audience and one-sentence value.
- Every page is numbered as the next step.
- Luna & Thread values are prefilled.
- Each success state links to the next role.
- Guest flow requires no account.
- Privacy and fit limitations appear at the decision point.
- Dashboard is legible and contains no photos.

### Design evidence

- `apps/web/app/page.tsx`
- `apps/web/components/BoutiqueApplicationForm.tsx`
- `apps/web/components/ProductReadinessForm.tsx`
- `apps/web/components/AdminReviewWorkspace.tsx`
- `apps/web/components/ConsumerTryOnPreflight.tsx`
- `apps/web/e2e/mobile.spec.ts`

### Design risk

The current home headline, “Virtual try-on, without the enterprise budget,” is credible but generic. Prefer: “Virtual try-on that a small boutique can actually run,” followed by the four plain-English benefits.

## Criterion 3: Potential Impact

**Judge question:** Does the project make a specific, credible case for solving a real audience’s problem based on what is demonstrated?

### Claim

The target is a small clothing retailer that wants a modern guest experience but lacks the team to manage a secure, asynchronous, unit-metered VTO integration.

### Evidence

- The 2022 Economic Census counted 34,479 full-year U.S. employer firms in clothing and clothing-accessories retail; 32,124 had fewer than 20 employees.
- Q1 2026 e-commerce was 16.9% of U.S. retail sales and grew faster year over year than total retail.
- Google and Walmart demonstrate that personal-photo VTO is a scaled large-platform capability.
- The product demonstrates the specific missing operations: readiness, approval, consent, duplicate control, units, private storage, and aggregate reporting.

### Demonstrate

Show fictional owner Elena Rivera operating a three-person boutique. Tie each screen to time, risk, or cost that her team does not have to engineer.

### Do not claim

- established return reduction;
- established conversion lift;
- a proven subscription business;
- a precise independent-boutique TAM;
- production-grade identity or multi-tenancy.

### Pilot close

Invite 10–25 boutiques to test activation, operator time, result completion, units per success, duplicate units avoided, shopper funnel, explicit garment interest, and deletion compliance.

## Criterion 4: Quality of the Idea

**Judge question:** Is the use of YouCam creative and non-obvious, with real understanding of the problem space?

### Claim

TryOnReady moves the innovation boundary from image generation to small-retailer operations and trust.

### Five-part novelty proof

1. **Readiness before cost:** garment image quality is checked before it can become consumer-eligible.
2. **Approval before publish:** an administrator controls what enters the guest catalog.
3. **Consent before processing:** a guest photo crosses the server boundary only after explicit consent.
4. **Duplicate control before another task:** repeat submissions reuse prior work and expose avoided units.
5. **Signals without surveillance:** retailer metrics exclude shopper photos.

### Best line for the judge

> “A raw VTO demo proves an image can be generated. TryOnReady proves a three-person boutique can prepare, approve, fund, protect, and learn from that experience.”

## First-Minute Proof Plan

| Time | On screen | Spoken takeaway |
|---:|---|---|
| 0:00–0:08 | Hero and Luna & Thread | “Virtual try-on that a small boutique can actually run.” |
| 0:08–0:20 | Four-step workflow | “The retailer prepares once; guests try privately without an account.” |
| 0:20–0:34 | Moonlight Blazer readiness | “Bad inputs are stopped before a provider unit can be used.” |
| 0:34–0:46 | Admin approval | “Nothing is published to shoppers without a human checkpoint.” |
| 0:46–1:00 | Guest consent and `API key in browser: Never` | “The shopper controls the photo; the boutique never sees it.” |

The result, duplicate proof, dashboard, and pilot close follow in the remaining 60–90 seconds.

## On-Screen Evidence Checklist

- Luna & Thread and Elena Rivera.
- Moonlight Blazer `SYN-BLZ-001`.
- 1254 × 1254 readiness result.
- Approved boutique and product.
- Marisol Lopez authorized synthetic asset.
- Consent checkbox.
- `Provider mode: YouCamLive` in the final recording.
- `API key in browser: Never`.
- `Try-on status · Succeeded`.
- Generated YouCam result.
- Nonzero live unit evidence consistent with the configured task cost.
- `Duplicate request prevented`.
- Dashboard: completed, units, duplicates.
- No shopper photos in retailer/admin dashboard.

Do not expose the YouCam key, signed upload URL, provider result URL, private storage path, or user-secrets file.

## Submission Claim Ledger

| Claim | Status on July 26, 2026 | Evidence needed before use |
|---|---|---|
| Full small-retailer workflow implemented | Verified | Repository and automated test log |
| Live YouCam adapter implemented | Verified in code | Adapter code |
| One live YouCam result completed | **Verified July 28, 2026** | `YouCamLive`, `Succeeded`, generated result, and one recorded API unit; see replay provenance |
| API key remains server-side | Verified by repository status/test evidence | Browser status and secret scan |
| Duplicate submission does not create second task | Verified in simulation | Test and live controlled retry if safe |
| Retailer never sees shopper photos | Verified in workflow/tests | Dashboard and access checks |
| Build/test suite passes | Previously verified | Fresh run before submission |
| Public judge URL works | **Verified** | Fly URL plus smoke run |
| Returns decrease | Unproven hypothesis | Pilot plus transaction/return comparison |
| Conversion increases | Unproven hypothesis | Pilot plus purchase data and comparison |
| Retailers will pay | Unproven hypothesis | Interviews and paid/committed pilot |

## Highest-Risk Gaps, in Order

1. **Public availability through judging** — keep the Fly URL and replay path healthy.
2. **First-screen value clarity** — the operating-layer novelty must be explicit.
3. **Claim discipline** — distinguish the July 28 live task from replay playback and future pilot outcomes.
4. **Secret-safe evidence** — no key, signed URL, or private storage path may appear.
5. **Pilot validation** — willingness to pay and commercial outcomes remain unproven.

## Final Submission Checklist

- [x] One controlled live AI Clothes v3 task completed with authorized synthetic assets.
- [x] Live task and unit evidence recorded without secrets or provider URLs.
- [x] Fly.io URL deployed and the same smoke path passed.
- [x] Public source repository and judging license are anonymously accessible; organizer notified.
- [x] README links to market validation, novelty, judge map, video narrative, pilot metrics, and judge guide.
- [x] Screenshots use only fictional/synthetic assets.
- [x] 1–3 minute video is publicly visible and names the YouCam Apparel VTO API.
- [x] Video shows the app functioning on its intended device.
- [x] Submission text explains what was significantly updated during the submission period.
- [x] No third-party copyrighted music or unauthorized marks appear.
- [ ] Judge credentials and instructions remain valid through the judging period.
- [ ] Build, tests, lint, typecheck, Playwright, and secret scan rerun against the final commit.
- [ ] Working/pending claims in `docs/IMPLEMENTATION-LOG.md` are current.
