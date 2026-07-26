# Image Storage and Deletion Policy

## Storage boundary

Garment, consumer, and generated images are stored outside `wwwroot` in a
private application-data directory. The database stores opaque asset
identifiers and metadata, not image bytes or public URLs.

Browser uploads terminate at the ASP.NET API. The browser never receives the
YouCam API key or YouCam signed upload URL.

## Retention schedule

| Asset | Purpose | Default retention |
|---|---|---|
| Garment source | Approved catalog reference | Until the product is deleted or replaced |
| Consumer source | Provider input | Delete immediately after terminal success/failure; cleanup safety limit 24 hours |
| Generated result | Consumer display and demo evidence | 24 hours by default |
| Provider task resources | YouCam processing | Request deletion after a terminal result when the documented provider operation is configured |
| Metadata/audit record | Operations and unit accounting | Retain without image content |

The defaults are configurable server-side. A cleanup service removes expired
local assets even if a previous processing attempt terminated unexpectedly.

## Logging rules

Application logs may contain:

- internal job identifiers
- product identifiers
- provider task status
- HTTP status and documented provider error code
- attempt count and elapsed time
- API units reserved/recorded

Application logs must not contain:

- API keys or authorization headers
- image bytes or Base64 data
- signed upload or result URLs
- consumer file names
- email addresses or other direct personal identifiers

## Demo limitations

The hackathon build is not a production identity or authorization system.
Garment and result routes use opaque identifiers and no-store response headers,
but production deployment would additionally require authenticated,
purpose-limited access control.

