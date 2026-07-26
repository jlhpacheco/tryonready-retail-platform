# Synthetic Demo Asset Metadata

All names, people, garments, and product records in this folder are fictional.
The images were created specifically for the TryOnReady hackathon demonstration
with OpenAI image generation. They do not depict real customers or real
inventory.

## Permitted use

These files are authorized for:

- local TryOnReady development and automated testing;
- the private hackathon repository;
- judge demonstrations and the submission video; and
- controlled YouCam Apparel VTO requests using the hackathon API units.

They are not authorized for resale, identity claims, or representation as real
retail merchandise. Do not replace them with a real customer photograph.

## Luna & Thread mini catalog

### Moonlight Blazer

- Product number: `SYN-BLZ-001`
- Category: `top`
- Brand: Luna & Thread
- Color: Terracotta
- Material: Cotton-blend suiting
- Sizes: XS, S, M, L, XL
- Demo price: USD 89.00
- File: `garments/moonlight-blazer.png`
- MIME type: `image/png`
- Dimensions: 1254 x 1254 pixels
- File size: 1,788,350 bytes
- SHA-256: `8f02878034877b38cc1e9f8372bfd145819e6ffff74f5e295709e3af8f5fd907`
- Prompt summary: isolated terracotta women's blazer, front-facing product
  photograph, complete garment visible, neutral studio background, no model,
  hanger, label, logo, text, or prop.
- Generated: 2026-07-25

### Harbor Sage Blouse

- Product number: `SYN-TOP-002`
- Category: `top`
- Brand: Luna & Thread
- Color: Sage
- Material: Cotton-linen blend
- Sizes: XS, S, M, L, XL, XXL
- Demo price: USD 64.00
- File: `garments/harbor-sage-blouse.png`
- MIME type: `image/png`
- Dimensions: 1254 x 1254 pixels
- File size: 1,608,884 bytes
- SHA-256: `8fd8adb4386ec064f9f8f621469aa685410abe7c7b7da62bb9e9fa25030a5bdc`
- Prompt summary: isolated sage women's blouse, front-facing product
  photograph, complete garment visible, neutral studio background, no model,
  hanger, label, logo, text, or prop.
- Generated: 2026-07-26

### Midnight Wrap Dress

- Product number: `SYN-DRS-003`
- Category: `full_body`
- Brand: Luna & Thread
- Color: Deep navy
- Material: Woven rayon blend
- Sizes: XS, S, M, L, XL
- Demo price: USD 118.00
- File: `garments/midnight-wrap-dress.png`
- MIME type: `image/png`
- Dimensions: 1024 x 1536 pixels
- File size: 2,131,211 bytes
- SHA-256: `4173e3974e3423db5c508c864cb0ac854a0d6d49d007af481070c58f9b6cf16e`
- Prompt summary: isolated deep-navy wrap dress, front-facing product
  photograph, complete garment visible, neutral studio background, no model,
  hanger, label, logo, text, or prop.
- Generated: 2026-07-26

## Fictional adult customer sources

### Marisol Lopez

- Persona: fictional adult boutique customer
- File: `customers/marisol-lopez-source.png`
- MIME type: `image/png`
- Dimensions: 864 x 1821 pixels
- File size: 1,758,378 bytes
- SHA-256: `2e139397587fe1e624cfb6dd51cb89a55d601cce72705428987d872842946182`
- Prompt summary: consented fictional adult Latina customer, full-body,
  front-facing pose, fitted neutral base clothing, arms relaxed, even studio
  lighting, uncluttered background.
- Generated: 2026-07-26

### Danielle Smith

- Persona: fictional adult boutique customer
- File: `customers/danielle-smith-source.png`
- MIME type: `image/png`
- Dimensions: 864 x 1821 pixels
- File size: 1,707,326 bytes
- SHA-256: `37aa40ff25192a6aa49d47dc98369e25855935ea360caabe7229e8c2ec3c84e9`
- Prompt summary: consented fictional adult Black customer, full-body,
  front-facing pose, fitted neutral base clothing, arms relaxed, even studio
  lighting, uncluttered background.
- Generated: 2026-07-26

## Demo pairing suggestions

- Marisol Lopez + Moonlight Blazer (`top`)
- Danielle Smith + Harbor Sage Blouse (`top`)
- Marisol Lopez + Midnight Wrap Dress (`full_body`)

The source images are input fixtures. A generated YouCam result is a separate
temporary artifact and must not be committed to the repository. TryOnReady
stores private source and result images outside the public web root and deletes
them according to `docs/DATA-RETENTION.md`.
