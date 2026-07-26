# Product Scope

## Purpose

TryOnReady helps small independent clothing boutiques explore virtual try-on without an enterprise technology budget.

## Intended workflow

1. A boutique owner applies from a phone, tablet, or desktop.
2. The owner photographs or uploads a garment and enters basic product details.
3. TryOnReady provides a virtual-try-on readiness assessment.
4. An administrator reviews and approves the boutique and garment.
5. A guest customer selects the approved garment, provides consent, and
   selects an authorized person image.
6. The secure server submits the assets to YouCam Apparel Virtual Try-On,
   tracks the task, and retrieves the result.
7. The guest sees the generated result.
8. The boutique sees aggregate activity and provider-unit totals only.

## Lean hackathon MVP

The retailer and administrator use simple demonstration sign-in roles. Guests
do not need an account. PostgreSQL persistence, private upload storage, human
approval, provider simulation, the live server adapter, status polling,
result retrieval, duplicate prevention, retention cleanup, and a small results
dashboard are in scope.

Retailer subscription billing, checkout, customer accounts, wishlists, purchase
interest, marketing automation, and multi-tenant organization management are
not part of the hackathon MVP.

## Product claims boundary

TryOnReady may describe visualization and workflow assistance. It must not
claim physical fit, sizing accuracy, medical analysis, or diagnostic results.
Simulation evidence must be labeled as simulation. A live provider result may
be claimed only after the controlled task is actually completed and recorded.

## Privacy boundary

Retailers must never see customer source/generated photographs, private customer attributes, or customer image-analysis details.

The provider API key remains server-side. Consumer source and result assets
follow the automatic deletion rules in `DATA-RETENTION.md`.
