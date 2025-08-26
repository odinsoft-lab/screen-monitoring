# Screen.Capture

A minimal Windows screen capture and real-time sample. It captures the primary screen on the sender machine and streams frames over TCP to a receiver that displays them in a Windows Forms UI. Built with .NET 8.

## Overview

- Transport: TCP (length-prefixed frames). A UDP helper is included in `Shared` but not used in the main demo flow.
- Encoding: Full-screen capture → JPEG encode → GZip compress.
- UI: WinForms app (`Receiver`) renders frames into a PictureBox.

## Project structure

- `src/Shared`
  - `Sender`: screen capture, JPEG encoding, GZip compression; also contains a UDP send utility (sample)
  - `Receiver`: TCP receive helpers, GZip decompression, byte[] → Image conversion
  - `Packet`: structure for UDP framing (not used by the default TCP path)
- `src/Sender`: console app that sends frames to a target IP:Port at a short interval
- `src/Receiver`: WinForms app that displays incoming frames
- Solution: `ScreenCapture.sln`

## Protocol (wire format)

For each frame sent over TCP:

1) 5-byte header
   - 4 bytes: payload length (little-endian Int32)
   - 1 byte: compression flag (`0x01` = compressed, `0x00` = uncompressed)
2) Payload: the frame bytes (JPEG if uncompressed; GZip(JPEG) if compressed)

Default inter-frame delay is about 10 ms, but actual FPS depends on your machine and network.

## Requirements

- Windows 10 or later
- .NET SDK 8.0+

## Quick start

1) Build the solution

```pwsh
# Run at the repository root (where README.md resides)
dotnet build .\ScreenCapture.sln
```

2) Start the receiver

```pwsh
dotnet run --project .\src\Receiver\Receiver.csproj
```

- Default listen port: `8088`
- To change the port: edit `src/Receiver/App.config` or `src/Receiver/Properties/Settings.settings` (key: `ListenPort`)

3) Start the sender

```pwsh
# Usage: dotnet run --project <csproj> -- <IP> <PORT>
dotnet run --project .\src\Sender\Sender.csproj -- 127.0.0.1 8088
```

- For same-machine tests use `127.0.0.1` or `localhost`.
- For cross-machine tests, use the receiver machine’s IP and allow the port through Windows Firewall.

## Features

- Primary screen capture (`Screen.PrimaryScreen`)
- JPEG + GZip to reduce bandwidth
- Simple TCP framing (length + compression flag)
- WinForms UI for real-time display

## Limitations

- Windows-only (relies on WinForms and `Screen.PrimaryScreen`)
- No encryption/auth (intended as a learning/demo project)
- Latency and dropped frames may occur depending on network conditions
- CPU usage can be high (tight loop capture + encode + compress)
- UDP send helper exists but the default demo path is TCP-based

## Troubleshooting

- No image on the receiver
  - Ensure the receiver is running before the sender connects
  - Verify IP/Port match between sender and receiver
  - Allow the receiver’s listen port through Windows Firewall
- Slow transfer or high CPU usage
  - Increase the capture delay (`DelayBetweenSendsMs` in `src/Sender/Program.cs`)
  - Consider tuning JPEG quality/encoding strategy (currently default Image.Save JPEG)
- Multi-monitor
  - Only `Screen.PrimaryScreen` is captured; extend to target a specific monitor/region if needed

## Roadmap ideas

- Make JPEG quality, resolution, and frame rate configurable and adaptive
- Multi-monitor and window/region capture support
- TLS or pre-encryption (e.g., AES) and authentication
- More efficient binary protocol and pipeline optimization
- UDP/RTP sample path with reordering/retransmission handling
- Optional input control (remote mouse/keyboard) with strong security

## Directory layout

```
src/
  Receiver/
  Sender/
  Shared/
ScreenCapture.sln
```

## License

No license file is included. Add a license that fits your usage before distributing.

## Notes

- Built with .NET 8 (WinForms)
- Length prefix uses little-endian Int32
- Receiver default port: `8088` (`App.config > userSettings > ListenPort`)
