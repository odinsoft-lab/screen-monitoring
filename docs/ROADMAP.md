# Roadmap

High-level plan for improving Screen.Monitoring. Items are grouped into milestones. This is a small demo project; scope should stay tight and practical.

## Milestone 0.1 — Baseline (current)

- [x] Primary screen capture on sender
- [x] JPEG encode and GZip compress
- [x] TCP transport with simple length-prefixed frames (4B length + 1B flag)
- [x] WinForms receiver displaying frames in a PictureBox

## Milestone 0.2 — Configurability and UX

- [ ] Make frame interval configurable at runtime (e.g., via args or settings)
- [ ] Expose JPEG quality and resolution scaling options
- [ ] Receiver: simple status UI (connected, FPS estimate, dropped frames)
- [ ] Sender: graceful exit (ESC) + cancellation token support

## Milestone 0.3 — Robustness and Performance

- [ ] Handle partial reads for the 5-byte header (loop until fully read)
- [ ] Handle partial reads for payload (loop until exact length)
- [ ] Backpressure: if receiver can’t keep up, adapt interval
- [ ] Avoid excessive allocations; reuse buffers where possible
- [ ] Basic exception classification and retries

## Milestone 0.4 — Security and Safety

- [ ] Optional TLS (self-signed for demo)
- [ ] Configurable bind IP and allow-list
- [ ] Basic authentication token (demo-grade)
- [ ] Make compression flag and framing more explicit (versioned header)

## Milestone 0.5 — Multi-monitor and Capture Options

- [ ] Select specific monitor or region to capture
- [ ] Optional cursor capture toggle
- [ ] Window capture mode (foreground or specific window)

## Milestone 0.6 — Alternative Transports

- [ ] UDP/RTP sample path using `Packet` with reordering/retransmission
- [ ] Measure and compare latency/bandwidth vs TCP

## Milestone 0.7 — Input and Control (optional)

- [ ] Optional remote input (mouse/keyboard) with strict security
- [ ] Hotkeys for quick start/stop and quality switches

## Milestone 0.8 — Packaging and Ops

- [ ] Publish single-file binaries for Sender/Receiver
- [ ] Add minimal logging and metrics hooks
- [ ] Create a basic installer or zip bundles

## Observability

- [ ] Add FPS/latency counters (sender and receiver)
- [ ] Optional lightweight logs (info/warn/error)

## Testing

- [ ] Add a test project for non-UI code in `src/Shared`
- [ ] Unit tests for compression/decompression helpers
- [ ] Small integration test for header framing

## Documentation

- [ ] Expand README with screenshots/GIFs
- [ ] Add usage tips for firewall and network settings

## Non-goals (for now)

- Production-ready remote desktop solution
- Cross-platform capture (Windows-only by design)
- Complex NAT traversal or STUN/TURN
