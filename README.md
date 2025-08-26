# Screen Capture

Minimal Windows screen capture and real-time streaming sample. The sender captures the primary screen, JPEG-encodes the frame, GZip-compresses it, and streams it over TCP to a WinForms receiver that displays the image. Built with .NET 8.

## Overview

- Transport: TCP with a simple length-prefixed framing. A UDP helper exists in `Shared` (for experiments) but is not used by the main path.
- Encoding: Full-screen capture → JPEG → GZip.
- UI: WinForms app (`Receiver`) renders frames in a PictureBox.

## Project structure

- `src/Shared`
  - `Sender`: screen capture, JPEG encoding, GZip compression; includes a UDP send utility (sample)
  - `Receiver`: TCP receive helpers, GZip decompression, byte[] → Image
  - `Packet`: DTO for UDP framing (not used by the default TCP path)
- `src/Sender`: console app that sends frames to a target IP:Port in a loop
- `src/Receiver`: WinForms app that displays incoming frames
- Solution: `screen.capture.sln`

## Protocol (wire format)

For each frame sent over TCP:

1) 5-byte header
   - 4 bytes: payload length (little-endian Int32)
   - 1 byte: compression flag (`0x01` = compressed, `0x00` = uncompressed)
2) Payload: frame bytes (JPEG if uncompressed; GZip(JPEG) if compressed)

The sender currently always compresses (`0x01`). The receiver handles both modes. Default inter-frame delay is 10 ms, but actual FPS depends on hardware and network.

## Requirements

- Windows 10 or later (WinForms + primary screen capture)
- .NET SDK 8.0+

## Quick start

1) Build the solution

```pwsh
# Run at the repository root (where README.md resides)
dotnet build .\screen.capture.sln
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
- Press `Esc` in the sender console to stop the capture loop.

## Features

- Primary screen capture (`Screen.PrimaryScreen`)
- JPEG + GZip to reduce bandwidth
- Simple TCP framing (length + compression flag)
- WinForms UI for real-time display

## Limitations

- Windows-only (relies on WinForms and `Screen.PrimaryScreen`)
- No encryption/authentication (intended as a learning/demo project)
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
  - Consider adjusting JPEG quality via a custom encoder (current code uses default Image.Save JPEG)
- Multi-monitor
  - Only `Screen.PrimaryScreen` is captured; extend to target a specific monitor/region if needed

## Docs

- Roadmap: `docs/ROADMAP.md`
- Tasks: `docs/TASK.md`
- Contributing: `docs/CONTRIBUTNG.md`

## Directory layout

```
src/
  Receiver/
  Sender/
  Shared/
screen.capture.sln
```

## License

Licensed under the MIT License. See `LICENSE.md` for details.

## Notes

- Built with .NET 8 (WinForms)
- Length prefix uses little-endian Int32
- Receiver default port: `8088` (`App.config > userSettings > ListenPort`)
