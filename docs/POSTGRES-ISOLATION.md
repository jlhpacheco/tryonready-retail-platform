# PostgreSQL Isolation

TryOnReady must use its own PostgreSQL database, Docker Compose project,
container name, network, volume, user, password, and host port.

## Protected boundary

Other local PostgreSQL containers and applications—including Duevara—are out of
scope. TryOnReady commands must never stop, restart, remove, inspect data from,
run migrations against, or reuse the volume or connection string of another
project.

The TryOnReady local identifiers are:

```text
Compose project: tryonready
Container:       tryonready-postgres
Database:        tryonready
User:            tryonready_app
Host port:       55432
Container port:  5432
Volume:          tryonready-postgres-data
```

The nonstandard host port deliberately avoids the common `5432` and the
already-observed `5433` mapping used by another local project.

## Allowed local commands

Run these commands only from the TryOnReady repository:

```powershell
docker compose -p tryonready up -d postgres
docker compose -p tryonready ps
docker compose -p tryonready logs postgres
```

Stopping TryOnReady later:

```powershell
docker compose -p tryonready stop postgres
```

Do not add `--volumes` to a stop/down command unless the explicit purpose is to
destroy the TryOnReady development database and that destructive action has
been confirmed.

## Application connection

The local development connection string uses only the isolated host port:

```text
Host=127.0.0.1;Port=55432;Database=tryonready;Username=tryonready_app;Password=local-development-only
```

Production uses Fly.io's injected `ConnectionStrings__TryOnReady` secret. No
database password is committed.

