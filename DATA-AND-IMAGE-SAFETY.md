# Data and Image Safety

## Hackathon data rule

Use synthetic, self-owned, or explicitly licensed data and images only.

The hackathon prototype must not contain or process real customer, boutique, employer, or production information.

## Prohibited repository content

Do not commit:

- Customer face or body photographs
- Government-issued identification
- Biometric templates or embeddings
- Real boutique applications or catalogs
- Private retailer product feeds
- Payment or banking information
- Production database exports
- API responses containing personal information
- Local upload folders or generated try-on images

## Demo personas

All demo boutiques, owners, customers, products, prices, activity counts, and transactions must be fictional and clearly described as synthetic.

Recommended demo boutique:

- Boutique: Luna & Thread
- Owner: Elena Rivera
- Purpose: fictional demonstration only

## Consent design

The consumer workflow must show clear consent before photo processing. The notice should state:

- The photo is being sent to an external AI service for virtual try-on.
- The result is a visual simulation and not a physical-fit guarantee.
- The boutique cannot view the original or generated customer image.
- The user can request deletion from TryOnReady storage.

## Retention design

For the prototype:

- Keep source and output images private.
- Use short-lived storage wherever practical.
- Record expiration timestamps.
- Provide a visible delete action.
- Do not retain images for analytics.
- Store only aggregated, non-identifying activity for the boutique dashboard.

## Retailer visibility

A boutique may see:

- Product-level try-on counts
- Processing success or failure counts
- Outbound product-link clicks
- API units consumed
- Products requiring better source images

A boutique must not see:

- Customer photographs
- Generated customer images
- Personal image attributes
- Customer identity inferred from a photograph

## Development rule

Automated tests must use generated fixtures, placeholder files, or assets whose rights are documented. Tests must not download random images from the internet.
