# TryOnReady Pilot Metrics

## Executive Summary

The pilot must answer four questions:

1. Can a small boutique activate and operate the workflow with little support?
2. Can shoppers complete a private, no-account try-on reliably?
3. Can TryOnReady control provider usage and show useful garment-interest signals?
4. Can every image and consent boundary be enforced as documented?

Run an 8–12-week pilot with 10–25 boutiques and 5–20 approved garments per boutique. Use the first two weeks to establish baselines before setting commercial improvement targets. Privacy, secret handling, duplicate prevention, and deletion compliance are hard gates from day one.

## Measurement Principles

- Count unique entities and define every denominator.
- Separate invalid inputs, provider failures, shopper abandonment, and product-policy rejection.
- Segment by boutique, garment, device class, provider mode, image-readiness outcome, and week.
- Exclude internal QA and judge-demo traffic from retailer outcomes.
- Label simulation traffic and never mix it with live provider metrics.
- Store aggregate event metadata, not shopper images or biometric attributes.
- Do not treat product-page visits, saves, or inquiries as purchases.
- Do not claim causal conversion or return impact without a comparison design and transaction data.

## Core Metrics

| Metric | Definition and formula | Primary denominator | Why it matters | Guardrail or caveat |
|---|---|---|---|---|
| Retailer activation rate | Boutiques with at least one approved garment and one live shopper-eligible product / boutiques admitted to pilot | Admitted pilot boutiques | Tests whether the service reaches usable value | Also report time to activation and support touches |
| Time to first approved garment | Median and P75 hours from application submission to first garment approval | Activated boutiques | Measures operator effort and workflow latency | Split retailer delay from admin delay |
| Readiness pass rate | Garment submissions passing on first check / garment submissions checked | Garment checks | Shows catalog compatibility and onboarding friction | A very high rate may mean the gate is weak; report rejection reasons |
| Readiness recovery rate | Initially failed garments later passing / initially failed garments | Initially failed garments | Tests whether guidance enables correction | Report reshoot, resize, format, and rights issues separately |
| Admin approval time | Median and P75 hours from ready garment submission to first decision | Garments entering review | Tests service operating burden | Measure business hours and elapsed hours |
| Admin first-pass approval rate | Garments approved on first decision / garments decided | Decided garments | Measures catalog quality and review clarity | Do not optimize by weakening review |
| Completed try-ons | Unique live nonduplicate jobs reaching `Succeeded` | Count, not a rate | Core shopper value volume | Exclude simulation, internal QA, and duplicate reuse |
| Result completion rate | Live unique jobs reaching `Succeeded` / live unique jobs accepted for processing | Provider-accepted unique jobs | Measures end-to-end reliability | Also report terminal failures and timeout aging |
| Submit-to-result time | Median and P95 seconds from accepted submission to terminal success | Successful live unique jobs | Captures shopper wait and provider/worker performance | Report queueing and provider time separately when possible |
| Duplicate requests prevented | Submissions matched to an existing fingerprint / all submitted requests | Submitted requests | Shows repeated shopper behavior and control effectiveness | A spike may indicate unclear UI or slow status feedback |
| Duplicate units avoided | Sum of configured units-per-task for prevented duplicates | Prevented duplicate submissions | Converts reuse into controlled provider usage | Label as avoided configured units, not cash savings |
| Units per completed try-on | Live API units consumed / live successful unique jobs | Live successful unique jobs | Measures variable efficiency | Include units consumed by failed jobs in a separate all-in metric |
| All-in units per success | All live units consumed, including failed jobs / live successful unique jobs | Live successful unique jobs | Shows true provider efficiency | Can exceed configured units per task when failures consume units |
| Consumer consent-to-submit rate | Unique sessions submitting after consent / sessions reaching the consent panel | Sessions reaching consent | Measures whether privacy copy supports informed action | Consent cannot be prechecked or coerced |
| Upload validation pass rate | Sessions with a valid person image / sessions attempting upload | Upload-attempt sessions | Identifies photo friction before provider use | Never store image-analysis attributes for retailer reporting |
| Result-view rate | Sessions that display a successful result / sessions with successful jobs | Successful jobs | Verifies result delivery, not just backend completion | Account for closed browser and later polling |
| Guest funnel completion | Sessions displaying a result / sessions viewing an approved garment | Approved-garment sessions | End-to-end shopper completion | Report each funnel step; do not infer intent |
| Garment save rate | Explicit save actions after result / sessions displaying a result | Result-view sessions | Lightweight interest signal | MVP can use session-local or event-only save; no account required |
| Product-page click rate | Retailer product-page clicks after result / sessions displaying a result | Result-view sessions | Stronger behavioral interest signal | A click is not a purchase |
| Purchase-interest rate | Explicit “Ask boutique” or “I’m interested” actions / sessions displaying a result | Result-view sessions | Tests retailer usefulness | Collect contact details only under separate opt-in; aggregate by default |
| Fitting-request rate | Explicit in-store fitting actions / sessions displaying a result | Result-view sessions | Connects digital discovery to boutique service | Appointment completion requires separate retailer evidence |
| Retailer weekly active rate | Retailers viewing catalog/dashboard or processing work in a week / activated retailers | Activated retailers | Indicates repeat operating value | Separate owner login from automated activity |
| Customer-source deletion compliance | Consumer source assets deleted by deadline / consumer source assets eligible for deletion | Eligible source assets | Hard privacy control | Target 100%; every miss is an incident |
| Result deletion compliance | Generated results deleted by configured deadline / results eligible for deletion | Eligible result assets | Tests documented retention | Target 100%; record grace-period and retry state |
| Provider deletion attempt success | Successful configured provider resource deletions / eligible terminal provider tasks | Eligible provider tasks | Measures external cleanup | Only report when deletion endpoint is configured |
| Unauthorized retailer photo access | Successful or attempted retailer access to shopper source/results | Security/audit events | Validates privacy boundary | Target zero successful access; investigate all attempts |
| Browser secret exposure | Builds or responses containing provider credentials or signed provider upload URLs | Build/release checks | Hard security control | Target zero |

## Funnel Definition

Use one anonymous session identifier with short retention and no customer account:

1. `approved_garment_viewed`
2. `person_upload_started`
3. `person_upload_valid`
4. `consent_accepted`
5. `try_on_submitted`
6. `provider_task_accepted`
7. `try_on_succeeded` or `try_on_failed`
8. `result_viewed`
9. optional explicit action:
   - `garment_saved`
   - `product_page_clicked`
   - `purchase_interest_clicked`
   - `fitting_request_clicked`

The event payload should contain only what the metric needs: timestamp, anonymous session ID, boutique ID, product ID, job ID, provider mode, device class, outcome, error category, configured/consumed units, and consent version where applicable. Do not include image bytes, filenames, signed URLs, emails, or body attributes.

## Activation and Operator Effort

Activation is more than account creation. A boutique is activated only when:

1. its application is approved;
2. at least one garment passes readiness;
3. the garment is approved and guest-visible; and
4. a test or shopper live try-on succeeds.

Track:

- elapsed time to each step;
- number of retailer and admin actions;
- support minutes;
- garments attempted per activated garment;
- top readiness rejection reasons;
- changes-requested rate;
- weekly catalog additions.

An operationally successful pilot should show that owners can repeat garment setup after the first assisted garment with fewer support touches. This learning curve is more decision-useful than activation rate alone.

## Readiness Metrics

Report readiness as a quality system, not a vanity pass rate.

Recommended reason taxonomy:

- unsupported media type;
- file over size limit;
- insufficient resolution;
- unreadable/corrupt image;
- category mismatch;
- multiple garments or ambiguous product;
- background/occlusion issue;
- rights confirmation missing;
- other documented validation failure.

Review false acceptance and false rejection manually on a sample. The gate creates value only if it stops materially unsuitable inputs without forcing unnecessary reshoots.

## Admin Approval Metrics

Measure:

- queue wait;
- active review time where instrumentable;
- first decision;
- re-review count;
- reason for changes/decline;
- approval consistency across reviewers.

Do not merge readiness pass with approval. Readiness is a technical gate; approval is a publishability and trust decision.

## Provider Reliability and Unit Control

Report live provider outcomes separately from simulation:

| Outcome | Count | Units consumed | Median latency | P95 latency |
|---|---:|---:|---:|---:|
| Succeeded |  |  |  |  |
| Provider rejected |  |  |  |  |
| Provider error |  |  |  |  |
| Timed out |  |  |  |  |
| Local validation stopped |  | 0 |  |  |
| Duplicate reused |  | 0 incremental |  |  |

Key cost-control calculations:

- `configured duplicate units avoided = prevented duplicates × configured units per task`
- `all-in units per success = all units consumed / successful unique live jobs`
- `avoidable failure units = units consumed on failures attributable to an input issue that readiness should have caught`

The last metric requires manual error review and should not be automated from provider error codes alone.

## Consent and Drop-Off

Consent must remain a requirement, not an optimization target. Track where users leave:

- before upload;
- after invalid upload;
- after reading the privacy panel;
- after consent but before submit;
- during processing;
- after success but before result view.

Use short optional exit feedback such as:

- “I did not want to upload a photo.”
- “I did not understand how my photo would be used.”
- “The upload requirements were difficult.”
- “The wait was too long.”
- “I changed my mind.”

Do not dark-pattern consent or remove privacy detail to increase completion.

## Garment Saves and Purchase-Interest Signals

These signals are recommended pilot additions but are not currently verified as implemented:

- **Save garment:** a session-local or anonymous event; no persistent account required.
- **View product:** outbound link to the boutique’s product page.
- **Ask the boutique:** opens a generic contact path; personal information requires a separate explicit opt-in.
- **Request fitting:** opens the retailer’s existing booking/contact flow.

The dashboard should show counts and rates by garment. It should not reveal the shopper’s image or identity.

If a pilot retailer provides transaction data, use aggregate garment/day or garment/week matching. Do not create a hidden identity join between try-on images and purchases.

## Privacy and Deletion Compliance

Hard gates:

- 100% of terminal consumer source assets deleted immediately after success/failure or by the documented safety deadline.
- 100% of generated results deleted by the configured result deadline.
- zero provider keys or authorization headers in browser bundles, API responses, logs, screenshots, or repository history.
- zero retailer/admin access to customer source or generated images.
- zero direct personal identifiers in operational logs.
- every deletion miss creates an incident record with cause, retry, and resolution time.

Provider retention is distinct from TryOnReady retention. Measure configured provider deletion attempts separately and never imply that local deletion proves provider deletion.

## Suggested Pilot Decision Gates

### Hard gates

- No secret exposure.
- No unauthorized retailer/admin shopper-photo access.
- Duplicate fingerprints never create a second provider task.
- 100% eligible local consumer-source and result assets meet the documented deletion deadline.
- Live provider mode is observable without exposing secrets.

### Operational gates

Set numeric thresholds after a two-week baseline. Suggested starting discussion points—not proven benchmarks—are:

- at least 90% result completion among provider-accepted valid live jobs;
- median admin decision within one business day;
- median submit-to-result time acceptable to observed shoppers;
- declining support touches for the second and later garment;
- all-in units per success stable enough for a predictable retailer offer.

### Value gates

Evidence of value requires both:

- repeated retailer use or explicit willingness to continue/pay; and
- nontrivial shopper engagement after result, such as product-page clicks, saves, inquiries, or fitting requests.

Do not require return-rate or purchase-conversion proof in the first pilot unless participating retailers can provide clean denominators and a comparison group.

## Analysis Plan

Weekly:

- operational reliability and incident review;
- activation and readiness funnel;
- provider units and duplicate control;
- privacy/deletion audit;
- top qualitative feedback.

At pilot midpoint:

- interview owners and staff;
- review readiness false accepts/rejects;
- test whether landing/privacy copy is understood;
- decide which post-result action is most useful.

At pilot end:

- cohort results by boutique and garment;
- confidence intervals or raw denominators for key rates;
- pre/post or comparison analysis only where design supports it;
- willingness-to-pay evidence;
- go, revise, or stop decision with named gaps.

## Minimum Dashboard Views

### Retailer

- approved garments;
- completed try-ons by garment;
- result completion rate;
- product-page/purchase-interest actions;
- duplicates stopped;
- provider units used;
- no customer images.

### TryOnReady operator

- activation funnel;
- readiness rejection reasons;
- admin queue time;
- provider outcome/latency;
- units per success;
- deletion compliance and incidents.

### Judge

- one controlled live completion;
- one prevented duplicate;
- aggregate counts;
- visible privacy boundary;
- no production or commercial claims beyond the evidence.

