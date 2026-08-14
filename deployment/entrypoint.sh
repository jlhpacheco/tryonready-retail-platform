#!/bin/sh
set -eu

case "${1:-serve}" in
  migrate)
    exec dotnet TryOnReady.Api.dll migrate
    ;;
  serve)
    exec dotnet TryOnReady.Api.dll
    ;;
  *)
    exec "$@"
    ;;
esac
