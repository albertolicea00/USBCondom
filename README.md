# USB Guard

A .NET 8 WPF prototype for copy-shop workstations, with a Windows 11 inspired interface in English.

**Prototype: system-wide execution blocking is not implemented. Do not deploy as a security boundary.**

## Run

Install the .NET 8 SDK on Windows 11, then run:

```powershell
dotnet run --project src/USBGuard/USBGuard.csproj
```

## Features

- Poll removable volumes every three seconds.
- Inspect file names and flag executable/script extensions and MZ signatures.
- Copy an import to local Incoming storage, inspect the copied bytes, and run Microsoft Defender.
- Release an entire import to Safe only when no file is flagged and Defender reports success.
- Never launch source files from the interface.

Imports live under `%LOCALAPPDATA%\USBGuard\Incoming` and `Safe`. Errors retain the import for manual review. These folders are ordinary folders, not an isolated sandbox. No automatic deletion is performed.

## Documentation

- [Architecture](docs/architecture.md)
- [Security and limitations](docs/security.md)
- [Operations and validation](docs/operations.md)
- [Roadmap](docs/roadmap.md)

## Build

```powershell
dotnet build src/USBGuard/USBGuard.csproj -c Release
dotnet publish src/USBGuard/USBGuard.csproj -c Release -r win-x64 --self-contained true -o artifacts/app
```

Compilation and hardware tests have not been performed in the authoring environment because it has runtimes but no .NET SDK.
