# Hackathon Scope

## Project

**TryOnReady** is a standalone hackathon application that helps independent clothing boutiques prepare garments for customer-facing virtual try-on.

## In scope for the hackathon

- Mobile boutique application
- Manual garment and product entry
- Garment-image readiness guidance
- Real YouCam Apparel VTO integration
- Background job status and error handling
- Administrative review and approval
- Small consumer virtual try-on storefront
- Boutique activity dashboard using synthetic metrics
- QR or share-link demonstration
- iPhone, Android, tablet, and desktop browser support through a PWA

## Mandatory guardrails

1. TryOnReady remains independent of Duevara.
2. No Duevara source code, branding, architecture, credentials, data, or business rules.
3. No employer code, work data, confidential records, or employer-owned assets.
4. No production data or customer information.
5. No real customer face or body photographs in the repository.
6. Use only synthetic, self-owned, or explicitly licensed demonstration assets.
7. Never commit YouCam, database, storage, signing, or deployment secrets.
8. The browser and PWA must never receive the private YouCam credential.
9. The repository remains private and proprietary unless the owner expressly authorizes a different release.
10. Public videos and screenshots must not disclose secrets, private implementation details, or third-party content without permission.
11. Any scoring or readiness logic must be original, documented, and limited to digital-asset readiness. It must not claim physical fit accuracy.
12. The project must not provide medical, dermatological, or body-health advice.

## Explicitly out of scope

- Full e-commerce checkout
- Payment processing
- Retailer commissions
- Live inventory synchronization
- Native App Store releases
- Large multi-vendor marketplace functionality
- Physical sizing or fit guarantees
- Medical or skin diagnosis
- Real customer analytics
- Production deployment credentials
- Unlicensed brand, model, garment, music, font, or image assets

## Change control

A proposed feature must be rejected or paused when it:

- Weakens an IP, privacy, or security boundary
- Requires production or confidential data
- Exposes secrets to the browser
- Depends on an asset without verified usage rights
- Creates a physical-fit, health, or diagnostic claim
- Expands the project beyond a polished end-to-end hackathon demonstration
