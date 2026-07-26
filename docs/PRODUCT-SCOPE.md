# Product Scope

## Purpose

TryOnReady helps small independent clothing boutiques explore virtual try-on without an enterprise technology budget.

## Intended workflow

1. A boutique owner applies from a phone, tablet, or desktop.
2. The owner photographs or uploads a garment and enters basic product details.
3. TryOnReady provides a virtual-try-on readiness assessment.
4. A future server-side workflow submits an approved garment to YouCam Apparel Virtual Try-On.
5. An administrator reviews the result.
6. An approved product receives a customer-facing try-on page.
7. The boutique sees aggregated activity only.

## First scaffold experience

The public site explains the concept and links to four workflow pages. Product Readiness is functional: it reads an owner-selected garment image in the browser and sends only its technical metadata to the local readiness API. Boutique Application, Admin Review, and Consumer Try-On remain clearly labeled scaffold previews.

## Product claims boundary

TryOnReady may describe visualization and workflow assistance. It must not claim physical fit, sizing accuracy, medical analysis, diagnostic results, or a completed live provider integration.

## Privacy boundary

Retailers must never see customer source/generated photographs, private customer attributes, or customer image-analysis details.
