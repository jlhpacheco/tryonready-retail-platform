# Controlled Demonstration Replay Provenance

## Deployment label

The deployed judge experience labels this result exactly as a **previously
completed controlled demonstration**. Playback makes zero new provider
requests. Live YouCam credentials are not configured in the Fly deployment.

The provider workflow demonstrated was **YouCam AI Clothes v3 / Apparel
Virtual Try-On**. The stored replay is evidence of an earlier controlled task;
it is not a claim that a new provider task runs during judge playback.

## Verified lineage

- Controlled task date: July 28, 2026.
- Contemporaneous application evidence: provider mode `YouCamLive`, terminal
  state `Succeeded`, and `API units used: 1` were visible together in the saved
  TryOnReady browser capture.
- The repository video narrative, written for that captured run, identifies it
  as the controlled live YouCam result completed on July 28, 2026.
- The stored result was copied without pixel edits from the local video-build
  master on August 1, 2026 and visually matched against that browser capture.
- No provider task identifier, signed URL, key, or credential is retained.

## Artifact checksums

| Artifact | SHA-256 |
| --- | --- |
| Marisol synthetic source | `2e139397587fe1e624cfb6dd51cb89a55d601cce72705428987d872842946182` |
| Moonlight Blazer synthetic source | `8f02878034877b38cc1e9f8372bfd145819e6ffff74f5e295709e3af8f5fd907` |
| Controlled result replay | `9dfd97d39ce7f7dc1033d452d11dcbfd3cc1bc62557d65e12093a8a7587ba6c2` |

The result file is
`samples/synthetic/replay/controlled-youcam-ai-clothes-v3-marisol-moonlight.jpg`.
Its verified size is 177,101 bytes. The result and both inputs depict only
fictional synthetic adults and merchandise created for this demonstration.

## Runtime safeguard

Production sets `YouCam__Enabled=false` and
`YouCam__SimulationEnabled=true`. The replay gateway reads the packaged result
and reports `ConsumesApiUnits: false`; it has no configured provider key and
makes no outbound provider request.
