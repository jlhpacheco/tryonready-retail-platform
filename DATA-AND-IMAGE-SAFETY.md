# Data and Image Safety

The public Fly site is a **synthetic-only judge demo**, not a production
multi-tenant service. Its workflow accepts and privately stores only the
repository-approved synthetic Moonlight Blazer and Marisol adult fixtures,
selected by the judge. SHA-256 allowlisting rejects all other image bytes
before storage. Live YouCam is disabled; stored replay makes zero new provider
requests.

Accepted synthetic source and replay files are stored outside `wwwroot` on an
encrypted application volume and follow the configured 24-hour cleanup
schedule. The retailer/admin dashboard never returns source or generated
shopper images. Do not submit real customer, employee, or personal media.

## Prohibited repository content

- customer face or body photographs
- real boutique, owner, employee, or customer information
- customer image-analysis details or private attributes
- API credentials, authorization headers, or secrets
- image payloads, encoded image content, or production storage references

## Demonstration data

The approved fictional demo boutique is **Luna & Thread**, operated by the fictional owner **Elena Rivera**. Product data and identifiers must also be synthetic.

## Production handling boundary

Before any environment may accept real images, it must:

- collect the minimum necessary data with clear consent
- keep source and generated customer photographs inaccessible to retailers
- use short, documented retention and deletion behavior
- prevent photographs and sensitive metadata from entering logs
- validate file type, size, and content at ingestion
- isolate uploaded content from public application assets
- decode and re-encode image content to strip EXIF/GPS and other metadata
- replace GUID-only anonymous result access with a separate high-entropy
  retrieval secret stored only as a hash
- complete a production identity, tenancy, abuse-prevention, and privacy review

Perfect Corp's current documentation states that provider-uploaded files, file identifiers, task identifiers, and generated media may remain available for up to 30 days, while a result download URL is valid for two hours. A future live integration must treat this provider retention as a maximum—not as TryOnReady's desired product retention—and document deletion, download, storage, and consent behavior before handling customer images.

Retailers may receive only aggregated operational metrics such as try-on
counts, processing outcomes, product-page visits, outbound clicks, and
API-unit consumption.
