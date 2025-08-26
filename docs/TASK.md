# Tasks and Backlog

This document lists actionable tasks derived from the current codebase and the roadmap. Use it to track near-term work.

## Active tasks (next up)

1) Robust TCP reads in Receiver
- [ ] Header read loop: ensure exactly 5 bytes are read before parsing length/flag
- [ ] Payload read loop: continue reading until `size` bytes are received
- Acceptance: run sender/receiver for 2 minutes without header/payload read exceptions

2) Graceful shutdown and cancellation
- [ ] Add `CancellationToken` support in both Sender and Receiver loops
- [ ] Map ESC (Sender console) and a UI button (Receiver) to request cancellation
- Acceptance: pressing ESC (Sender) or Stop (Receiver) exits without unhandled exceptions

3) Configurable frame interval and JPEG quality
- [ ] Add CLI args to Sender: `--interval-ms`, `--jpeg-quality`
- [ ] Validate bounds (e.g., 1–100 for JPEG quality)
- Acceptance: changing parameters affects CPU usage and observed FPS

4) Basic status UI on Receiver
- [ ] Show connection state, last frame time, and rough FPS
- [ ] Handle reconnects cleanly if Sender restarts
- Acceptance: UI reflects connected/disconnected within 2s and shows FPS ~ close to expected

## Backlog

- [ ] Add resolution scaling option on Sender (e.g., 0.5x, 0.75x)
- [ ] Buffer pooling for compression/decompression
- [ ] Optional TLS encryption for TCP stream
- [ ] Select monitor or region to capture
- [ ] Cursor capture toggle
- [ ] Minimal logging abstraction (ILogger-like)
- [ ] Add unit tests for `Shared.Receiver.DecompressToImage` and `ByteArrayToImage`
- [ ] Add integration test for header framing (5-byte header + payload)

## How to work on a task

1. Create a branch from `main` (e.g., `feature/fps-status`)
2. Commit in small steps; keep PRs focused
3. Build and smoke-test locally
4. Open a PR and link this task list item

## Definition of Done

- Compiles on Windows with .NET 8 (`dotnet build`)
- Manual smoke test: Receiver shows frames from Sender for at least 60 seconds
- No new warnings or obvious leaks (dispose streams and bitmaps)
- README/docs updated if behavior or usage changes
