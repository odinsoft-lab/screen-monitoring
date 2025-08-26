# Contributing to screen-monitoring

Thanks for your interest in contributing! This repository is a small .NET 8 sample that streams the primary screen from a sender to a Windows Forms receiver over TCP.

This guide explains how to set up your environment, propose changes, and submit pull requests.

## Ways to contribute

- Report bugs and regressions
- Propose enhancements and new features
- Improve documentation
- Contribute code (small, focused pull requests are best)

## Development setup

### Requirements

- Windows 10 or later
- .NET SDK 8.0+
- PowerShell (pwsh) or Windows Terminal
- Optional: Visual Studio 2022 or VS Code

### Clone and build

```pwsh
# Clone your fork or the upstream repo
# git clone <repo-url>
# cd screen-monitoring

# Build the solution at the repo root
 dotnet build .\ScreenCapture.sln
```

### Run locally

Start the receiver (WinForms app):

```pwsh
 dotnet run --project .\src\Receiver\Receiver.csproj
```

Start the sender (console app):

```pwsh
# Usage: dotnet run --project <csproj> -- <IP> <PORT>
 dotnet run --project .\src\Sender\Sender.csproj -- 127.0.0.1 8088
```

Receiver default port is `8088`. To change it, edit `src/Receiver/App.config` (key: `ListenPort`).

## Branching and workflow

1. Create a feature branch from `main`:
   - `feature/<short-topic>` for features
   - `fix/<short-topic>` for bug fixes
2. Make small, atomic commits (see Conventional Commits below).
3. Open a Pull Request targeting `main`.
4. Keep PRs focused; ideally under ~300 lines changed.

## Commit messages (recommended)

Follow Conventional Commits:

- `feat: add adjustable frame interval`
- `fix(receiver): handle partial header reads`
- `docs: clarify quick start steps`

Scopes: `sender`, `receiver`, `shared`, `docs`.

## Coding standards

- C# 12, .NET 8
- Nullable reference types: enabled
- Prefer `var` for local variables when type is evident
- Use `async/await` and cancellation tokens where appropriate
- Always handle partial reads/writes when using streams and sockets
- Dispose `IDisposable` objects deterministically (`using` or `await using`)
- Keep methods small and focused; extract helpers when logic grows

### Style

- Naming: PascalCase for types/methods, camelCase for locals/parameters, _camelCase for private fields
- Braces: always use braces for single-line statements
- Logging: use `Debug.WriteLine` minimally; for larger changes consider structured logging

## Tests

There is no test project yet. If you add significant logic (especially in `src/Shared`), consider adding a new test project and basic unit tests as part of your PR.

## Pull Request checklist

- [ ] The change compiles (`dotnet build`)
- [ ] Local smoke test passes (sender/receiver basic handshake)
- [ ] Comments and docs updated where needed
- [ ] No secrets or sensitive info in code or logs

## Reporting issues

Please include:

- Environment: OS version, .NET SDK version
- Steps to reproduce
- Expected vs. actual behavior
- Logs or screenshots when helpful

## Code of Conduct

Be kind, respectful, and constructive. Harassment and discrimination are not tolerated.

## License

This project currently has no license file. If you intend to distribute changes, propose and add an appropriate license in a separate PR.
