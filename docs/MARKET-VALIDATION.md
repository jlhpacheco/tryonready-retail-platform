# TryOnReady Market Validation

## Executive Summary

- **The target is large enough for a focused retail service without pretending every clothing store is an independent boutique.** The 2022 U.S. Economic Census counted 34,479 employer firms in clothing and clothing-accessories retail that operated for the full year. Of those, 32,124 had fewer than 20 employees—93.2% of the full-year firm count. This is the best available public proxy for the small-retailer problem pool, not a count of boutiques that will buy TryOnReady.
- **Large platforms are normalizing virtual try-on while e-commerce keeps gaining ground.** U.S. e-commerce represented 16.9% of retail sales in Q1 2026 and grew 9.8% year over year, versus 3.9% for total retail. Google now offers personal-photo apparel try-on across product listings, and Walmart previously reported apparel VTO across more than 270,000 items. A small boutique increasingly competes against that experience, even when its differentiator is service and curation.
- **The consumer problem is visual uncertainty, not guaranteed physical fit.** In a 2023 Google/Ipsos U.S. survey of 1,614 adult online clothing shoppers, 42% said model images did not represent them and 59% reported dissatisfaction because an item looked different on them than expected. TryOnReady can test whether a private, no-account visualization improves confidence and engagement; it must not promise size accuracy, conversion lift, or fewer returns before a pilot.
- **The product’s strongest reason to exist is operational.** YouCam’s official workflow involves authenticated file reservation, signed upload, task creation, polling, unit consumption, rate limits, and time-limited result retrieval. TryOnReady packages those mechanics with garment-readiness checks, approval, consent, duplicate protection, unit accounting, private storage, and aggregate reporting.
- **The next commercial proof should be a 10–25-boutique, 8–12-week pilot.** Do not publish a revenue TAM yet. Validate willingness to pay, operator effort, shopper completion, provider reliability, cost per completed result, duplicate units avoided, privacy compliance, garment-interest signals, and any directional change in purchase-interest behavior.

## The Problem, Defined Narrowly

TryOnReady is for U.S. independent or owner-operated clothing retailers that:

1. sell new apparel from a physical store, online storefront, or both;
2. have a small team and no dedicated VTO engineering or catalog-operations group;
3. can supply authorized garment images and product metadata;
4. want to offer guest shoppers a private visualization experience; and
5. are willing to run a controlled pilot and measure results.

The problem is not “small retailers have no access to AI.” The problem is that access to an apparel VTO API does not by itself provide a trustworthy catalog workflow, safe customer-image handling, usage controls, or a result that a three-person boutique can operate repeatedly.

## How Many Retailers Plausibly Face This Problem?

### Evidence-backed employer-firm proxy

The most relevant current firm-level federal table is the U.S. Census Bureau’s 2022 Economic Census, table `EC2200SIZEEMPFIRM`, for 2022 NAICS `458110` (“Clothing and clothing accessories retailers”). It covers U.S. employer firms and was released April 24, 2025.

| Firm employment size | Full-year firms |
|---|---:|
| Fewer than 5 employees | 20,500 |
| 5–9 employees | 7,902 |
| 10–19 employees | 3,722 |
| **Fewer than 20 employees (derived)** | **32,124** |
| 20–49 employees | 1,625 |
| **All full-year employer firms** | **34,479** |

Derived shares:

- Fewer than 20 employees: `32,124 / 34,479 = 93.2%`
- Fewer than 50 employees: `33,749 / 34,479 = 97.9%`

**Interpretation:** 32,124 is a defensible upper-bound proxy for small U.S. employer firms in the product category. It is not a count of independent boutiques, digitally active merchants, or willing buyers.

**Limitations:** The table excludes nonemployers; includes both physical and nonstore firms classified in this industry; does not identify independent ownership, e-commerce maturity, available garment imagery, technology stack, or willingness to pay; and is a 2022 snapshot.

### Establishment cross-check

The 2023 County Business Patterns dataset counted 83,287 U.S. employer establishments in 2017 NAICS `4481` (“Clothing stores”). In the downloaded national file:

- 66,352 establishments had fewer than 20 employees at the location (79.7%).
- 78,153 had fewer than 50 employees at the location (93.8%).

This is a location count, so it includes individual locations of chains and must not be presented as an independent-business count. It is useful only as a cross-check showing that small operating locations dominate the category.

### Pilot-market qualification range

Because public data do not identify the subset with suitable ownership, digital intent, catalog assets, and willingness to pilot, use an explicit qualification assumption:

| Scenario | Assumed qualified share of the 32,124-firm proxy | Discovery pool |
|---|---:|---:|
| Conservative | 5% | about 1,600 firms |
| Base | 10% | about 3,200 firms |
| Expanded | 15% | about 4,800 firms |

These percentages are assumptions, not sourced adoption rates. The range is a recruiting and discovery pool, not SAM revenue.

### Recommended pilot scope

- Recruit 10–25 boutiques.
- Prefer 2–10 employee businesses with at least 20 active apparel SKUs and an owner or manager who controls digital merchandising.
- Run for 8–12 weeks.
- Start with 5–20 approved garments per boutique.
- Do not set a subscription price until interviews establish the buyer, budget, expected usage, and acceptable cost per completed try-on.

A practical early beachhead is one region or one existing boutique association where onboarding and feedback can be hands-on. This is an execution choice, not a claim about geographic demand.

## Competitive Pressure From Large Omnichannel Retailers

Three facts establish the pressure without overstating causality:

1. **E-commerce is material and growing faster than retail overall.** The Census Bureau reported that seasonally adjusted U.S. e-commerce sales were 16.9% of total retail sales in Q1 2026. E-commerce grew 9.8% year over year while total retail grew 3.9%.
2. **Personal-photo VTO is becoming a scaled platform feature.** Google’s May 2025 shopping flow lets a shopper upload a full-length photo and try apparel from product listings; a December 2025 update described access from billions of Shopping Graph listings.
3. **Large retailers can deploy VTO across large catalogs.** Walmart reported in September 2022 that its “Be Your Own Model” experience covered more than 270,000 apparel items.

**Inference:** Shoppers can encounter interactive visualization before purchase on platforms with enormous engineering, catalog, and data resources. Independent boutiques compete for the same shopper attention but cannot replicate that operating infrastructure one store at a time.

**Limitation:** These examples demonstrate availability and competitive capability, not that every shopper expects VTO or that VTO causes conversion improvements.

## What Shopper Friction Does VTO Address?

The narrow, supportable job is:

> Help a shopper answer “How might this garment look on me?” before deciding whether to save it, visit the product page, ask the boutique, or pursue an in-store fitting.

Supporting evidence:

- A Google/Ipsos U.S. survey conducted April 28–30, 2023 among 1,614 adults who had shopped for clothing online found that 42% did not feel represented by model imagery and 59% had been dissatisfied with an online item because it looked different on them than expected.
- Perfect Corp’s July 3, 2026 apparel VTO explainer distinguishes visual context from measurement guidance and says the strongest experience uses both: size guidance for measurement confidence and VTO for visual confidence.
- The NRF/Happy Returns 2025 study estimated that 19.3% of online sales would be returned. That figure is retail-wide, not apparel-specific, and the merchant survey covered large U.S. merchants with more than $500 million in revenue. It establishes returns as an industry pressure, not TryOnReady’s expected impact.

TryOnReady should therefore say:

- “Preview how the garment may look on your photo.”
- “Virtual try-on does not guarantee physical fit or sizing.”
- “We will test whether try-on improves confidence and purchase-interest signals.”

It should not say:

- “Guaranteed fit.”
- “Reduces returns.”
- “Increases conversion.”
- “Pays for itself.”

Those are hypotheses to measure.

## Why Direct Enterprise VTO Is Operationally Hard for a Small Boutique

The YouCam API is capable, but the official provider contract still leaves a retailer-facing operating system to build:

| Provider or integration obligation | Small-retailer consequence | TryOnReady control |
|---|---|---|
| Bearer-authenticated server calls | A key cannot safely live in browser code or a public repository | ASP.NET server-side adapter and secret boundary |
| File reservation followed by signed HTTPS upload | Upload state and errors must be managed | Private upload boundary and trusted-host checks |
| Asynchronous task creation and polling | The shopper needs progress, terminal states, and retry behavior | Background processing and status endpoint |
| Units are the API’s currency and features deduct different amounts | Repeated or invalid requests can create avoidable cost | Readiness gate, duplicate fingerprint, reserved/consumed units |
| 250 requests per 300 seconds per IP and access token | Production code needs pacing, backoff, and error handling | Centralized provider adapter and worker |
| Uploaded/generated assets are retained by the provider for defined periods; result links expire | Results must be retrieved, stored, proxied, and deleted under policy | Private result retrieval, short local retention, optional provider deletion |
| The API returns a rendering, not a publishable catalog process | Bad or unauthorized garment inputs can reach shoppers | Readiness checks and human approval |
| The API processes a shopper photo | Consent, access control, retention, and retailer visibility must be designed | Explicit consent, no-account guest flow, private storage, aggregate-only retailer view |

This table is the core market insight: TryOnReady sells repeatable operation and trust around the API, not the API response itself.

## Why a Retailer Might Pay

These are hypotheses, ordered by what the current MVP can test:

1. **Digital-experience parity:** the boutique can offer a modern interaction without building or staffing a VTO integration.
2. **Controlled variable cost:** the boutique sees completed work and units, while readiness and duplicate controls prevent some avoidable spend.
3. **Low shopper friction:** a guest does not create an account or pay a fee.
4. **Catalog trust:** a garment is checked once, reviewed, and published deliberately.
5. **Privacy separation:** the boutique gets aggregate activity but not shopper photographs.
6. **Merchandising signal:** aggregate try-on and explicit post-result interest events can show which garments attract attention.

Willingness to pay is unproven. Interview owners using three price frames—per completed try-on, monthly platform fee with included units, and retailer-funded campaign pilot—without implementing billing in the judging MVP.

## Pilot Outcomes That Would Validate the Case

Use the definitions in `PILOT-METRICS.md`. The minimum decision set is:

- retailer activation and time to first approved garment;
- garment-readiness pass rate and rejection reasons;
- admin approval time and rework rate;
- unique completed try-ons and result completion rate;
- provider units per completed try-on;
- duplicate requests and units avoided;
- consent/upload/submit/result funnel drop-off;
- explicit garment saves, product-page visits, purchase-interest, or fitting-request signals;
- retailer weekly active use and qualitative willingness to pay;
- customer-source and result deletion compliance;
- zero key exposure, retailer photo access, and unauthorized image retention.

Returns and purchase conversion require retailer transaction data and a credible comparison design. They should be optional later-stage outcomes, not hackathon claims.

## Facts, Inferences, and Assumptions

| Type | Statement |
|---|---|
| Sourced fact | 34,479 U.S. employer firms in NAICS 458110 operated for all of 2022. |
| Derived fact | 32,124 of those firms had fewer than 20 employees, or 93.2%. |
| Sourced fact | U.S. e-commerce represented 16.9% of total retail sales in Q1 2026. |
| Sourced fact | YouCam uses units, authenticated multi-step tasks, polling, rate limits, and timed asset access. |
| Repository fact | TryOnReady’s complete workflow has passed automated simulation tests; no controlled live YouCam completion is recorded as of July 26, 2026. |
| Inference | A managed operating layer is more usable for a small boutique than a raw API integration. |
| Assumption | 5%–15% of the under-20-employee firm proxy could meet early qualification criteria. |
| Assumption | A 10–25-boutique pilot is sufficient to test workflow usability and directional engagement, not causal commercial impact. |
| Unproven hypothesis | Retailers will pay for controlled VTO access and aggregate engagement signals. |
| Unproven hypothesis | VTO will increase purchase interest or reduce avoidable returns. |

## Recommended Next Steps

1. Complete one controlled live YouCam task with authorized synthetic assets and record only non-secret evidence.
2. Put the live provider state, result, and unit count in the final video.
3. Conduct 8–12 owner interviews before naming a price.
4. Recruit a 10–25-boutique pilot and baseline each metric before setting improvement targets.
5. Add only lightweight post-result signals—save garment, visit product page, ask boutique, request fitting—without adding accounts or checkout.
6. Treat public deployment, live-task proof, and judge access as submission blockers.

## Further Questions

- Which role owns garment preparation in a 2–10 person boutique?
- How many active garments would a retailer actually enable for VTO?
- What percentage of existing catalog images pass readiness without reshooting?
- Would owners prefer predictable monthly spend or per-completed-result pricing?
- Which post-result action is most meaningful: product-page visit, fitting request, or direct inquiry?
- Can pilot retailers provide order/return data at garment level without exposing shopper identity?

## Source Register

| Source | Publication/release date | Metric or claim used | Geography and scope | Limitation |
|---|---|---|---|---|
| [U.S. Census Bureau, 2022 Economic Census, Employment Size of Firms](https://data.census.gov/table/ECNSIZE2022.EC2200SIZEEMPFIRM?q=EC2200SIZEEMPFIRM) | April 24, 2025 | Firms, establishments, and employment-size classes for NAICS 458110 | U.S. employer firms, reference year 2022 | Not an independent-boutique or willingness-to-pay count; nonemployers excluded |
| [U.S. Census Bureau, 2023 County Business Patterns](https://www.census.gov/data/datasets/2023/econ/cbp/2023-cbp.html) | June 26, 2025 | Establishments and establishment employment size for NAICS 4481 | U.S. employer establishments, reference year 2023 | Counts locations, including chain locations; uses 2017 NAICS |
| [U.S. Census Bureau, Q1 2026 Retail E-Commerce Sales](https://www.census.gov/retail/eCommerce.html) | May 18, 2026 | E-commerce share and year-over-year growth versus total retail | U.S. retail, seasonally adjusted, Q1 2026 | All retail categories; not apparel- or small-retailer-specific |
| [Google Shopping, personal-photo virtual try-on](https://blog.google/products-and-platforms/products/shopping/how-to-use-google-shopping-try-it-on/) | May 20, 2025; updated Nov. 20, 2025 | Personal-photo apparel VTO flow | Google Shopping users; feature availability may vary | Company-authored product announcement |
| [Google Shopping, apparel VTO and Google/Ipsos survey](https://blog.google/products-and-platforms/products/shopping/ai-virtual-try-on-google-shopping/) | June 14, 2023 | 42% representation and 59% expectation-mismatch findings | U.S. adults 18+ who shopped for clothing online, n=1,614, April 2023 | Google/Ipsos survey tied to a Google launch; not a TryOnReady outcome |
| [Walmart, “Be Your Own Model”](https://corporate.walmart.com/news/2022/09/15/walmart-levels-up-virtual-try-on-for-apparel-with-be-your-own-model-experience) | Sept. 15, 2022 | More than 270,000 VTO-enabled apparel items at launch | Walmart U.S. | Company-authored and older; shows scale, not consumer impact |
| [NRF/Happy Returns, 2025 Retail Returns Landscape](https://nrf.com/media-center/press-releases/consumers-expected-to-return-nearly-850-billion-in-merchandise-in-2025) | Oct. 15, 2025 | 15.8% estimated overall return rate; 19.3% online; survey methods | 2,006 U.S. consumers with a recent online return; 358 professionals at U.S. merchants over $500M revenue | Retail-wide, not apparel-specific; merchant sample is not small business |
| [Perfect Corp, AI Clothes Try-On explainer](https://www.perfectcorp.com/business/blog/ai-clothes/ai-clothes-changer) | July 3, 2026 | Visual-confidence job and distinction from size guidance | Provider-authored fashion e-commerce guidance | Marketing source; supports use case, not causal performance claims |
| [YouCam API FAQ](https://docs.perfectcorp.com/develop/faq) | Last updated approx. April 2026 | Units, polling, upload, authentication, server/backend security guidance | YouCam API | Documentation can change; verify before production |
| [YouCam API rate limits](https://docs.perfectcorp.com/develop/rate_limit) | Last updated July 2026 | 250 requests per 300 seconds per IP/token and backoff guidance | YouCam API | Operational limit, not a demand metric |
| [YouCam API file retention](https://docs.perfectcorp.com/develop/file_retention_period) | Last updated May 2026 | 30-day provider retention; 2-hour result download URL | YouCam API | TryOnReady’s own shorter local policy is separate |
| [Devpost official rules](https://youcam-api.devpost.com/rules) | Accessed July 26, 2026 | Working YouCam integration, retail value, video, repository, access, and equal-weight judging criteria | YouCam API Skin AI & Apparel VTO Hackathon | Rules may be amended; recheck before submission |

