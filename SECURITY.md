# Security Policy

## Current security posture

The Fly deployment is a synthetic-only hackathon judge demo. It is not a
production multi-tenant service and must not accept real customer media or
provider credentials.

The hosted demo forces HTTPS, emits HSTS/CSP/framing/content-type/referrer/
permissions headers, rate-limits login and upload requests, allows one upload
to be processed at a time, hides OpenAPI in Production, and uses a
`__Host-`-prefixed Secure/HttpOnly/SameSite=Strict authentication cookie.

## Configuration and secrets

- Never commit API keys, connection strings, access tokens, or credentials.
- Keep any future YouCam credential on the server.
- Browser code must never receive provider credentials.
- Use environment variables or an approved secret store for local and hosted secrets.
- Keep `.env.example` limited to safe names and non-secret placeholders.

## Application boundaries

- Validate input at API and upload boundaries.
- Require the same-origin application header on multipart mutations.
- In the hosted judge environment, accept only the SHA-256 allowlisted
  Moonlight Blazer and Marisol synthetic fixtures.
- Keep anonymous job/result identifiers limited to the synthetic-only demo;
  add a separate hashed high-entropy retrieval token before accepting real
  person images in any future environment.
- Keep business logic outside controllers and endpoints.
- Keep provider-specific behavior inside `TryOnReady.YouCam`.
- Use cancellation tokens for asynchronous operations.
- Log structured operational events without images, authorization headers, secrets, or personal data.
- Do not expose detailed exception messages in production.
- Run the application container as a non-root user. Keep PostgreSQL private to
  Fly's internal network.

## Reporting

Do not open a public issue containing a suspected vulnerability or sensitive data. Report security concerns privately to the repository owner.
