# Data and Image Safety

TryOnReady's first scaffold uses synthetic metadata only. It stores no uploaded images.

## Prohibited repository content

- customer face or body photographs
- real boutique, owner, employee, or customer information
- customer image-analysis details or private attributes
- API credentials, authorization headers, or secrets
- image payloads, encoded image content, or production storage references

## Demonstration data

The approved fictional demo boutique is **Luna & Thread**, operated by the fictional owner **Elena Rivera**. Product data and identifiers must also be synthetic.

## Future handling boundary

Any later image workflow must:

- collect the minimum necessary data with clear consent
- keep source and generated customer photographs inaccessible to retailers
- use short, documented retention and deletion behavior
- prevent photographs and sensitive metadata from entering logs
- validate file type, size, and content at ingestion
- isolate uploaded content from public application assets

Perfect Corp's current documentation states that provider-uploaded files, file identifiers, task identifiers, and generated media may remain available for up to 30 days, while a result download URL is valid for two hours. A future live integration must treat this provider retention as a maximum—not as TryOnReady's desired product retention—and document deletion, download, storage, and consent behavior before handling customer images.

Retailers may eventually receive only aggregated operational metrics such as try-on counts, processing outcomes, product-page visits, outbound clicks, and API-unit consumption.
