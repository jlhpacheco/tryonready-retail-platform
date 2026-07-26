# Third-Party Notices

## Original synthetic garment test assets

The three PNG files in `samples/garments` were generated specifically for the
private TryOnReady hackathon project using OpenAI image generation on July 25,
2026. They depict fictional, unbranded garments and contain no real people,
customer data, or third-party logos.

## Synthetic judge demo asset pack

The three garment PNG files under `samples/synthetic/garments` and the two
fictional adult customer PNG files under `samples/synthetic/customers` were
generated specifically for TryOnReady using OpenAI image generation on July 25
and July 26, 2026. They contain no real customer, real retailer, third-party
brand, or third-party logo. Their hashes, dimensions, prompt summaries, and
authorized demo purpose are recorded in
`samples/synthetic/ASSET-METADATA.md`.

These files may be used for the private hackathon repository, local automated
tests, the judge demonstration, and controlled YouCam Apparel VTO validation.
They must not be represented as photographs of real people or real merchandise.

TryOnReady depends on third-party frameworks and packages distributed under their own licenses. Their inclusion does not grant an open-source license to TryOnReady.

Primary application dependencies include:

- .NET and ASP.NET Core — Microsoft
- Entity Framework Core — Microsoft
- xUnit — .NET Foundation and contributors
- Next.js — Vercel and contributors
- React — Meta Platforms, Inc. and contributors
- TypeScript — Microsoft
- ESLint and related plugins — their respective contributors

The external Apparel Virtual Try-On provider is Perfect Corp.'s YouCam API.
Product information is available at <https://yce.makeupar.com/ai-api>, and the
developer documentation is available at
<https://docs.perfectcorp.com/develop/introduction>. No provider credential or
proprietary sample is included in the repository.

Consult installed package metadata and upstream repositories for exact license texts and versions.
# Additional vertical-slice dependencies

- Npgsql Entity Framework Core provider 10.0.3 — PostgreSQL license
- Microsoft Entity Framework Core 10.0.4 — MIT License
- SixLabors.ImageSharp 3.1.12 — Six Labors Split License

ImageSharp 4.0.0 was evaluated and rejected because its build requires a
separately supplied Six Labors license key. TryOnReady does not contain or
request that key. Version 3.1.12 is pinned for server-side image header
validation and must be reviewed against the Six Labors license before any
commercial production use.
