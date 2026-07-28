# TryOnReady 1–3 Minute Video Narrative

## Target Cut

**Rendered judge cut:** 2 minutes 32 seconds
**Audience:** Hackathon judges seeing TryOnReady for the first time  
**Single idea:** YouCam generates the apparel result; TryOnReady makes the experience operable, private, and cost-controlled for a small boutique.

The rendered cut uses the controlled live YouCam result completed on July 28,
2026. It visibly shows `YouCamLive`, `Succeeded`, the generated Marisol result,
and `API units used: 1`. Do not describe a simulated output as provider evidence
in any alternate cut.

## Tight Script and Shot List

| Time | Screen and action | Voiceover | On-screen proof |
|---:|---|---|---|
| 0:00–0:10 | Open on the operations-proof hero: Luna & Thread, Moonlight Blazer, readiness, approval, guest boundary, and controlled unit count. | “Independent boutiques compete with enterprise digital shopping experiences, but a three-person shop does not have an enterprise integration team. TryOnReady is virtual try-on that a small boutique can actually run.” | “Boutique operations proof”; `YouCamLive`; readiness passed; approval; one completed try-on; one API unit; zero shopper photos in retailer view |
| 0:10–0:20 | Scroll or frame the workflow and privacy/value strip. | “The boutique prepares and approves a garment once. Guests try it privately without an account. Valid requests and provider units stay controlled, and the retailer gets useful totals—not customer photos.” | Four steps; privacy boundary; no-account value |
| 0:20–0:33 | Submit the prepared Luna & Thread application for Elena Rivera. | “Elena Rivera owns fictional boutique Luna & Thread. She submits one guided application with image-rights confirmation.” | Three employees; physical store; rights checkbox; “Application submitted” |
| 0:33–0:50 | Continue to Product Readiness, select Moonlight Blazer, and save. | “She adds the Moonlight Blazer. TryOnReady checks the file before it can become a paid try-on input, preventing unsuitable images from reaching the provider.” | `SYN-BLZ-001`; 1254 × 1254; readiness passed |
| 0:50–1:02 | Switch to administrator; approve boutique and garment. | “A human approval checkpoint decides what is safe and complete enough to publish to shoppers.” | Approved boutique; approved product; readiness status |
| 1:02–1:24 | Open Consumer Try-On; show approved garment, provider proof, privacy box, upload Marisol, and check consent. | “Guest shopper Marisol needs no account. She chooses the approved garment, sees the privacy and fit limits, and explicitly consents. Her photo stays outside the retailer workflow, and the YouCam key never reaches the browser.” | `Provider mode: YouCamLive`; `API key in browser: Never`; consent; no fit guarantee |
| 1:24–1:40 | Generate once; show status transitions and the real result. | “The ASP.NET server securely reserves and uploads both images, creates a YouCam AI Clothes v3 task, polls it, retrieves the result, and serves it back through TryOnReady.” | Processing → Succeeded; generated result; unit count |
| 1:40–1:51 | Submit the exact unchanged request once more. | “An unchanged retry reuses the prior request. No second provider task is created.” | `Duplicate request prevented` |
| 1:51–2:08 | Open retailer results dashboard. Frame counts and the absence of photos. | “The boutique sees completed work, stopped duplicates, and API units. It never sees Marisol’s source or generated photo.” | Completed; units used; duplicates stopped; no photos |
| 2:08–2:20 | Return to hero or a final title card. | “The YouCam result is the engine. TryOnReady is the readiness, approval, consent, cost, and privacy layer that makes it usable for an independent boutique. Our next step is a 10-to-25 boutique pilot measuring reliable try-ons, retailer effort, garment interest, controlled units, and deletion compliance.” | Five controls; pilot invitation; repository/URL/YouCam API label |

## Exact Opening and Closing

### Opening

> “Independent boutiques compete with enterprise digital shopping experiences, but a three-person shop does not have an enterprise integration team. TryOnReady is virtual try-on that a small boutique can actually run.”

**Opening visual distinction:** Others help one shopper choose a look.
TryOnReady helps a small boutique operate the service safely for every guest.

### Close

> “The YouCam result is the engine. TryOnReady is the readiness, approval, consent, cost, and privacy layer that makes it usable for an independent boutique. Our next step is a 10-to-25 boutique pilot measuring reliable try-ons, retailer effort, garment interest, controlled units, and deletion compliance.”

## What the Video Must Prove

1. **One product, not disconnected screens:** Luna & Thread moves continuously from application through result.
2. **Real YouCam integration:** the final run visibly shows `YouCamLive`, a completed result, and coherent unit accounting.
3. **Non-trivial engineering:** readiness, background processing, duplicate reuse, and private result delivery are visible behaviors.
4. **Coherent design:** each role knows the next step; the guest does not sign in.
5. **Specific audience:** Elena’s three-person boutique is not an abstract “SMB.”
6. **Novelty:** five operating controls sit around the provider generation.
7. **Claim discipline:** VTO is a visualization, not a size or physical-fit guarantee.

## Live-Test Capture Plan

Before recording:

1. Pass the full simulation suite.
2. Confirm the selected synthetic garment and person assets.
3. Confirm `YouCamLive` through the safe status field; never open secrets.
4. Record the dashboard baseline.
5. Start screen capture with browser-only framing.
6. Submit once and wait for terminal success.
7. Record the visible result and unit count.
8. Submit the unchanged request once to show duplicate protection.
9. Record the post-run dashboard.
10. Update `docs/IMPLEMENTATION-LOG.md` with date, synthetic asset names, terminal result, and unit reconciliation—never the key or any signed URL.

If the live task fails, stop the recording, preserve the documented error code without secret URLs, fix the integration, and run a new deliberate test. Do not edit a simulation result into a “live” narrative.

## Browser Framing and Editing

- Record at a mobile-friendly desktop width that keeps headings and actions readable.
- Zoom only enough to make proof text legible.
- Cut waits, typing, and role-sign-in dead time; do not cut in a way that disguises provider mode or result state.
- Use brief callouts for `Readiness passed`, `Human approved`, `Consent`, `YouCamLive`, `Duplicate prevented`, and `No customer photos`.
- Keep the generated result on screen long enough to be recognized.
- Blur nothing that should have been excluded from capture in the first place.
- Do not open browser developer tools, logs, user secrets, provider signed URLs, database tools, or container dashboards.
- Use no copyrighted music or third-party trademarks without permission.
- Include captions; judges may watch without sound.

## Claim-Safe Caption Bank

Use:

- “Server-side YouCam AI Clothes v3”
- “Controlled live provider task”
- “Garment checked before consumer eligibility”
- “Guest consent required”
- “No customer account required”
- “Duplicate request reused”
- “Retailer sees aggregate totals, not customer photos”
- “Conversion and return impact are pilot hypotheses”

Avoid:

- “Perfect fit”
- “Accurate size”
- “Guaranteed privacy”
- “Zero wasted units”
- “Proven conversion lift”
- “Proven return reduction”
- “Production ready”

## Optional 90-Second Cut

If the full application typing is too slow, begin with Luna & Thread already submitted but show the application success state for two seconds. Keep the garment readiness, human approval, guest consent, live result, duplicate proof, and dashboard. Those are the non-negotiable story beats.

Suggested compression:

- 0:00–0:12 problem and position
- 0:12–0:27 application and garment
- 0:27–0:40 readiness and approval
- 0:40–1:02 consent and live result
- 1:02–1:14 duplicate proof
- 1:14–1:25 dashboard/privacy
- 1:25–1:30 pilot close

## Screenshot Set for Devpost

1. Landing page with the small-boutique position.
2. Moonlight Blazer readiness pass.
3. Admin review with approved product.
4. Guest privacy/consent and server-side key proof.
5. Controlled live result with non-secret provider/unit evidence.
6. Duplicate prevented.
7. Aggregate dashboard without customer photos.

## Pre-Upload Checklist

- [ ] Runtime is the final deployed build.
- [ ] Provider mode is live in the final recording.
- [ ] Live result is actually completed and recorded.
- [ ] No secret, authorization header, signed URL, or private path appears.
- [ ] Only fictional/synthetic people, boutique, and garment assets appear.
- [ ] YouCam Apparel VTO / AI Clothes v3 is named.
- [ ] Video runs between 1:00 and 3:00.
- [ ] End-to-end app footage is visible.
- [ ] Captions are accurate.
- [ ] Public YouTube or Vimeo link works without sign-in.
- [ ] Judge URL and repository instructions match the filmed version.
