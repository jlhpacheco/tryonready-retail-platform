# Security Policy

## Security objectives

TryOnReady must protect API credentials, retailer records, customer uploads, generated images, and administrative functions while remaining simple enough for a hackathon demonstration.

## Secret handling

- Never commit real credentials.
- Use environment variables or an approved secret manager.
- Keep the YouCam credential server-side.
- Do not expose secrets through browser bundles, logs, screenshots, sample files, tests, or error messages.
- Rotate a credential immediately if it is accidentally disclosed.

## Required authorization boundaries

Roles:

- `BoutiqueOwner`
- `Reviewer`
- `Administrator`

A boutique owner may manage only their own application and products. A reviewer may inspect applications and garment validation results but may not manage secrets. Administrative access must be limited and auditable.

## Upload controls

- Allow only approved image types.
- Enforce file-size and image-dimension limits.
- Generate server-side filenames rather than trusting user filenames.
- Store uploads outside the public web root.
- Reject active content and unsupported formats.
- Do not log raw image data.

## API controls

- Validate all input.
- Use server-side rate limits for expensive AI operations.
- Require idempotency for retryable job submissions.
- Apply timeouts and bounded retries.
- Return safe errors without provider credentials or internal traces.
- Record job status transitions in an audit log.

## Browser and PWA controls

- No YouCam credential in client code.
- No persistent customer images in browser storage.
- Avoid caching private image responses in the service worker.
- Use secure cookies or well-scoped tokens.
- Apply Content Security Policy and standard security headers.

## Logging

Permitted logging:

- Request correlation ID
- Job ID
- Status transition
- Processing duration
- Error category

Prohibited logging:

- Credentials
- Authorization headers
- Full customer photographs
- Image payloads
- Sensitive personal data

## Vulnerability reporting

Do not publish suspected vulnerabilities in a public issue. Report them privately to the repository owner with reproduction steps and the minimum necessary evidence.
