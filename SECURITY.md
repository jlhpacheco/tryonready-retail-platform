# Security Policy

## Current security posture

This repository is an early private scaffold and is not production-ready.

## Configuration and secrets

- Never commit API keys, connection strings, access tokens, or credentials.
- Keep any future YouCam credential on the server.
- Browser code must never receive provider credentials.
- Use environment variables or an approved secret store for local and hosted secrets.
- Keep `.env.example` limited to safe names and non-secret placeholders.

## Application boundaries

- Validate input at API and upload boundaries.
- Keep business logic outside controllers and endpoints.
- Keep provider-specific behavior inside `TryOnReady.YouCam`.
- Use cancellation tokens for asynchronous operations.
- Log structured operational events without images, authorization headers, secrets, or personal data.
- Do not expose detailed exception messages in production.

## Reporting

Do not open a public issue containing a suspected vulnerability or sensitive data. Report security concerns privately to the repository owner.
