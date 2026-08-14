# TryOnReady Devpost Submission Draft

Status: **Submitted to the YouCam API Skin AI & Apparel VTO Hackathon.**

Submission receipt: `1133079`  
Submitted: August 10, 2026 at 11:18:38 p.m. Eastern  
Live entry: <https://devpost.com/software/tryonready>

Devpost project: <https://devpost.com/software/tryonready>

Hackathon: <https://youcam-api.devpost.com/>

Official deadline: August 17, 2026 at 11:45 a.m. ET / 8:45 a.m. PT.
Internal target: August 16, 2026 at 5:00 p.m. PT.

Jose Luis approved the submission and the public video on August 10, 2026.
Do not place passwords, provider
credentials, signed URLs, or real customer data on Devpost.

## Project metadata

- **Name:** TryOnReady
- **Tagline:** Virtual try-on that a small boutique can actually run.
- **App status:** New
- **Project start date:** 07-25-26
- **Submitter:** Jose Luis Pacheco
- **Submitter type:** Individual
- **Country of residence:** United States (California)
- **Judge website:** <https://tryonready-demo.fly.dev/>
- **Repository:** <https://github.com/jlhpacheco/tryonready-retail-platform>
- **Repository access:** Public and source-available for hackathon judging under the repository `LICENSE`
- **YouTube publishing account:** <https://www.youtube.com/@jlhpacheco>
- **Video:** <https://youtu.be/hKEY5tmI7G0>
- **Optional social-media post:** Leave blank.

## Inspiration

Independent boutiques compete for the same shoppers as national retail
platforms, but usually without dedicated engineering, security, catalog, or
integration teams. TryOnReady started with a practical question: how can a
small shop offer apparel virtual try-on without turning the owner into an API
operator or exposing customer images?

Our synthetic boutique, Luna & Thread, represents that resource gap. Elena
Rivera prepares one garment once, an administrator verifies it, and a guest
shopper such as Marisol Lopez can use the approved experience without opening
an account.

## What it does

TryOnReady adds an operating and trust layer around **YouCam AI Clothes v3 /
Apparel Virtual Try-On**:

- A boutique submits a guided application and confirms its rights to garment
  imagery.
- The retailer adds a garment and passes image-readiness checks.
- An administrator approves the boutique and garment before customer use.
- A guest shopper selects an approved garment, reads the privacy and
  visualization limits, gives consent, and provides an authorized image
  without creating an account.
- The server controls provider credentials, asynchronous processing, duplicate
  prevention, API-unit accounting, private result delivery, and cleanup.
- The retailer sees aggregate operational results, not shopper source or
  generated images.

The deployed judge experience uses only clearly labeled synthetic adults and
merchandise. It includes a **previously completed controlled demonstration** of
YouCam AI Clothes v3. Playback makes zero new provider requests and does not
claim a new unit was consumed. Virtual try-on visualizes appearance; it does
not guarantee physical fit or sizing.

## How we built it

The responsive interface is built with Next.js, React, and TypeScript. A single
ASP.NET Core 10 host serves the exported web application and API from one
origin.

The application layer uses explicit ports for catalog, approval, private
storage, and virtual try-on. EF Core 10 and PostgreSQL persist boutique
applications, approved products, job state, duplicate fingerprints, provider
references, and aggregate usage data. Images remain outside the public web
root.

The YouCam adapter implements the full AI Clothes v3 server workflow: reserve
file identifiers, upload the person and garment images to signed HTTPS targets,
create a task, poll its status, securely retrieve the result, and clean up
provider resources. The browser never receives a provider credential.

The judge deployment runs on isolated Fly.io application and PostgreSQL
Machines in San Jose. It uses encrypted persistent volumes, health checks,
automatic restarts, release migrations, daily snapshots, a tested restore
rehearsal, and a stored-result fallback.

## Challenges we ran into

### Turning an API call into a safe retail workflow

The provider task is asynchronous, but a boutique needs clear states and
predictable recovery. We modeled submission, processing, polling, success,
failure, and cleanup explicitly. Request fingerprints return the existing
result for an unchanged product/person pair instead of creating a duplicate
provider task.

### Keeping private images and credentials out of the wrong places

We kept the YouCam key entirely server-side, stored images outside the public
web root, separated retailer and administrator authorization, prevented
retailer access to shopper imagery, constrained upload size and concurrency,
and documented deletion boundaries.

### Creating a stable judge experience without misleading anyone

A live provider dependency can fail or consume units whenever a judge repeats
a demonstration. We preserved a verified synthetic result from a controlled
successful run and display it with exact provenance: **previously completed
controlled demonstration; playback makes zero new provider requests**. The
live adapter remains implemented independently from replay mode.

### Deploying a complete product within a small operational footprint

We separated the application and PostgreSQL resources while keeping both in
one region. Release migrations, non-root execution, restrictive production
headers, private database networking, persistent volumes, snapshots, rollback,
and restore instructions had to fit a lean shared-Machine deployment.

## Accomplishments that we're proud of

- A coherent retailer-to-shopper workflow rather than a thin API wrapper.
- A complete server-side YouCam AI Clothes v3 adapter with no browser
  credential exposure.
- Human approval and garment readiness before a shopper can begin.
- Guest consent, private results, duplicate prevention, and aggregate retailer
  reporting.
- A judge-accessible deployment with a synthetic, provenance-verified
  controlled result.
- Clear product limits: no real-customer claim, no fit guarantee, and no
  unsupported conversion or return-reduction claim.
- Automated .NET, TypeScript, and browser test foundations plus production
  security and recovery controls.

## What we learned

The largest lesson was that the AI generation call is only one part of a usable
retail product. The difficult and valuable work is everything around it: input
readiness, human approval, consent, asynchronous state, duplicate control, unit
accounting, privacy, cleanup, and a recovery path.

We learned that asynchronous provider APIs need explicit application states
and idempotency. Treating a retry as a brand-new request would waste units and
confuse shoppers; fingerprinting the unchanged request lets us reuse a valid
result.

We also learned that privacy claims have to be enforced architecturally.
Keeping credentials on the server, storing images outside the public site,
separating roles, and limiting retailer visibility are stronger than relying
on interface copy alone.

Finally, we learned that a credible demonstration needs provenance and precise
language. A stored result is useful evidence only when we identify when and how
it was produced, label synthetic people and merchandise, disclose that playback
makes no new provider request, and avoid claiming fit, sizing, conversion, or
return outcomes that have not been measured.

## What's next for TryOnReady

The next step is a measured boutique pilot with human approval. We would
evaluate reliability, staff effort, shopper completion, controlled API cost,
duplicate prevention, deletion compliance, and privacy, rather than assume
conversion lift or return reduction in advance.

We also plan to refine catalog onboarding, improve accessibility and
operational monitoring, and validate whether the workflow remains simple for
a real small-business team while preserving the same server-side credential
and private-image boundaries.

## Built with

- YouCam AI Clothes v3 / Apparel Virtual Try-On
- ASP.NET Core 10, C#, and .NET 10
- Next.js, React, and TypeScript
- PostgreSQL and Entity Framework Core 10
- Fly.io Machines, private networking, and encrypted volumes

## Required custom answers

### Was there a moment during the hackathon where the API surprised you—in a good or frustrating way?

The useful surprise was how naturally YouCam's asynchronous task model could
fit behind a server-side application boundary. The frustrating surprise was
that a successful generation alone does not produce a safe retail experience.
File reservation and upload, task creation, polling, result retrieval, retry
behavior, cleanup, and customer-facing state all have to agree. That pushed us
to build a durable workflow rather than expose a single API call from the
browser.

### Are there industries or use cases you think Perfect Corp.'s API could serve that nobody is talking about yet?

The overlooked opportunity is the operational layer for resource-constrained
retailers. Independent boutiques, uniform and workwear programs, resale and
consignment catalogs, wardrobe-rental businesses, and staff-assisted retail
could use apparel visualization when it is paired with catalog readiness,
human approval, consent, duplicate control, cost accounting, and private
results. The API can support business operations, not only a consumer styling
widget.

### Where did you hit a wall technically? How did you work around it?

We hit the wall at the boundary between asynchronous provider work and a
repeatable judge and shopper experience. A retry could create another paid
task, a restart could lose in-memory state, and private images or credentials
could cross the wrong boundary. We worked around that by keeping the adapter
server-side, persisting job states in PostgreSQL, fingerprinting unchanged
requests, bounding polling and result sizes, keeping images outside the public
web root, and providing a provenance-verified stored result as an explicitly
labeled fallback. The fallback makes zero new provider requests and never
pretends to be a new live run.

## Private judge testing instructions checklist

The private Devpost testing instructions must include:

1. Judge URL: <https://tryonready-demo.fly.dev/>
2. The retailer and administrator usernames/passwords from the ignored local
   handoff file. Never copy them into this document or Git.
3. A note that the consumer path requires no account.
4. A note to use only the supplied Marisol synthetic adult fixture.
5. The exact disclosure: **previously completed controlled demonstration**;
   playback makes zero new provider requests.
6. The API identification: **YouCam AI Clothes v3 / Apparel Virtual Try-On**.
7. The limitation: visualization is not a fit or sizing guarantee.
8. Public repository URL and source-available hackathon judging license.

## Submission gate

- [x] Jose Luis approves the complete project-page wording.
- [x] Submitter name, type, country, and project start date are confirmed.
- [ ] Final screenshots and thumbnail are selected and approved.
- [x] Carlos's final video passes the strictly-under-3:00 gate (2:38).
- [x] The approved final video is publicly accessible on YouTube.
- [x] Private judge credentials were delivered only in the private organizer guide.
- [x] The public repository is anonymously accessible and the organizer was emailed the updated URL.
- [x] The public website and YouTube video are accessible.
- [x] Project was submitted before the official deadline.
- [x] Submission receipt/state was verified after submission.
