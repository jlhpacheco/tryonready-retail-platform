# Fly.io Deployment and Recovery Runbook

TryOnReady deploys one integrated ASP.NET Core container that serves the built
Next.js UI/API. It uses an isolated single-node Fly Postgres app over private
networking. This is a lean judge-demo topology, not managed HA.

## Fixed isolation boundary

| Resource | Name | Region | Size |
| --- | --- | --- | --- |
| Public application | `tryonready-demo` | `sjc` | 1 shared CPU, 512 MB |
| App-private volume | `tryonready_demo_data` | `sjc` | 1 GB |
| PostgreSQL app | `tryonready-demo-db` | `sjc` | 1 shared CPU, 256 MB |
| PostgreSQL volume | Fly-generated `pg_data` volume | `sjc` | 1 GB |

Never attach, inspect, restart, scale, rename, or reuse any `duevara-*` or
`carpermitapp*` app, Machine, volume, secret, database, or data.

The database is unsupported self-managed Fly Postgres with one Machine and one
volume. It has no replica, automatic failover, or managed high availability.
The app and database must remain in `sjc`.

## Cost control

The topology uses pay-as-you-go shared Machines and volumes, not Fly Managed
Postgres. The August 1, 2026 Fly list prices used for the gate are:

- shared CPU 1x / 512 MB app: approximately $3.32 per continuously running
  30-day month before any regional markup;
- shared CPU 1x / 256 MB database: approximately $2.02 per continuously running
  30-day month before any regional markup;
- two 1 GB volumes: $0.30/month total;
- daily incremental snapshots: first 10 GB of stored snapshots free per month;
- shared IPv4 and the first ten single-host certificates per organization are
  free; North American public egress is usage-based.

Allowing for regional variation, root filesystems, and small judge traffic, the
incremental projection is approximately **$6-$12/month** and must remain below
the authorized ~$25/month cap. Do not add replicas, Managed Postgres, dedicated
IPv4, static egress IPs, or support plans without new approval.

## Runtime truth and secrets

Committed non-secret configuration is in `fly.toml`. Runtime secrets are:

```text
ConnectionStrings__TryOnReady
DemoAccess__Retailer__Password
DemoAccess__Administrator__Password
```

Never print or commit their values. Live YouCam is disabled. Production uses
the provenance-locked stored result documented in
`CONTROLLED-REPLAY-PROVENANCE.md`:

```text
YouCam__Enabled=false
YouCam__SimulationEnabled=true
```

The UI labels it a **previously completed controlled demonstration** and states
that playback makes zero new provider requests.

The hosted site is synthetic-only. `fly.toml` enables SHA-256 allowlisting for
the repository's Moonlight Blazer and Marisol fixtures. Real customer media is
rejected before private storage. The application image runs as the non-root
`.NET app` user; the mounted app volume must remain owned by UID/GID 1654.

## Migration safety

Normal application startup never runs EF Core migrations. `fly.toml` defines:

```toml
[deploy]
  release_command = "migrate"
```

The release command runs the new image once and blocks the deployment if the
idempotent EF migration fails. The normal container command is `serve`.

## Provisioning sequence

Run from this repository only:

```powershell
fly postgres create --name tryonready-demo-db --org personal --region sjc --initial-cluster-size 1 --vm-size shared-cpu-1x --vm-memory 256 --volume-size 1
fly apps create tryonready-demo --org personal
fly volumes create tryonready_demo_data --app tryonready-demo --region sjc --size 1 --scheduled-snapshots --snapshot-retention 5 --yes
fly postgres attach tryonready-demo-db --app tryonready-demo --database-name tryonready --database-user tryonready_app --variable-name ConnectionStrings__TryOnReady --superuser=false --yes
fly secrets import --app tryonready-demo
fly deploy --app tryonready-demo --config fly.toml --strategy immediate --ha=false
```

Generate credentials locally and pass them through standard input to
`fly secrets import`; do not place values on the command line or in shell
history. `postgres attach` creates a dedicated database/user and injects only
the TryOnReady connection secret.

## Health, restart, and monitoring

- HTTPS is forced on the Fly-provided `tryonready-demo.fly.dev` hostname.
- Production sends HSTS, CSP, clickjacking, MIME-sniffing, referrer, and browser
  permissions headers. OpenAPI is not mapped in Production.
- Login requests are rate-limited. Multipart mutations require the app's
  same-origin header, are rate-limited, and are serialized through a
  one-request concurrency gate.
- The app stays running for judge reliability and has `on-failure` restart with
  ten retries.
- `/health` checks database connectivity, app-private disk headroom, and
  container memory headroom every 15 seconds.
- Monitor with `fly status --app tryonready-demo`,
  `fly checks list --app tryonready-demo`, and
  `fly logs --app tryonready-demo`.
- Inspect Machine memory and restart counts in Fly Metrics/Grafana. Inspect
  both volume sizes and regions with `fly volumes list` for each TryOnReady app.
- Review Fly Cost Explorer after deployment and at least weekly through
  August 31, 2026. Stop and investigate if projected incremental spend reaches
  $20; do not allow it to exceed $25 without new approval.

## Daily snapshots and safe recovery rehearsal

Both volumes have automatic daily snapshots with five-day retention. Verify:

```powershell
fly volumes list --app tryonready-demo
fly volumes list --app tryonready-demo-db
fly volumes snapshots list <postgres-volume-id> --app tryonready-demo-db
```

An appropriately safe recovery rehearsal creates a temporary same-region
volume from a database snapshot, confirms its size/region/state, and then
removes only that temporary rehearsal volume. It does not attach the restored
volume, start PostgreSQL against it, or modify the live database.

For a real restore:

1. Record the database Machine and volume IDs; stop writes by stopping only the
   `tryonready-demo` app Machine.
2. Create a new `sjc` volume from the chosen database snapshot. Never overwrite
   or delete the original volume.
3. Clone/update the PostgreSQL Machine configuration to mount the restored
   volume at the same database mount point.
4. Start the restored database Machine and verify PostgreSQL health privately.
5. Start the app, verify `/health`, and run read-only judge smoke checks.
6. Retain the original volume until Jose Luis explicitly approves cleanup.

## Application rollback

Database migrations must remain backward-compatible with the immediately prior
application image. To roll back source without touching data:

```powershell
fly releases --app tryonready-demo --image
fly deploy --app tryonready-demo --image <prior-immutable-image-ref> --strategy immediate --ha=false
fly status --app tryonready-demo
fly checks list --app tryonready-demo
```

The installed Fly CLI exposes immutable image references but has no
`releases rollback` subcommand, so rollback is a pinned-image redeploy. Never
roll back by deleting a database or volume.

## Judge smoke test

The official rules require free access through August 31, 2026 at 11:45 a.m.
Eastern. Put the Fly URL and role credentials only in private Devpost testing
instructions. Share the private repository with
`contact_event@PerfectCorp.com`.

In a clean browser:

1. Confirm `/`, `/health`, and `/api/status` over HTTPS.
2. Confirm `/api/status` reports `StoredReplay`, PostgreSQL, the full YouCam API
   name, and zero new provider requests.
3. Complete retailer application and garment readiness with repository
   synthetic assets.
4. Complete administrator approval.
5. Complete guest consent with the distinct Marisol synthetic source.
6. Confirm the stored result, exact replay label, and zero new YouCam requests.
7. Repeat unchanged input and confirm duplicate reuse.
8. Confirm the retailer dashboard contains aggregate counts and no shopper
   photos.

## Judging-window live provider schedule

Jose Luis approved this one-time live-provider window for the official judging
period. Windows Task Scheduler on the authorized deployment workstation runs
`tools/operations/Set-TryOnReadyProviderMode.ps1` at these Eastern times:

- enable live YouCam: August 18, 2026 at 11:59 a.m. Eastern
- disable live YouCam and restore stored replay: August 31, 2026 at 11:46 a.m.

The workstation is in Pacific time, so the registered local triggers are
August 18 at 8:59 a.m. and August 31 at 8:46 a.m. The tasks wake the workstation
when possible, run missed starts when the workstation becomes available, retry
up to three times, and verify `/api/status` after Fly restarts the application.

The YouCam key is stored only in Fly encrypted secrets and local .NET User
Secrets. It is never written to this script, repository, GitHub Actions, or
task arguments. The operations script targets only `tryonready-demo`.
