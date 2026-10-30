# Contributing

Use English for code, UI text, documentation and commit messages.

## Conventional Commits

Use `type(scope): short imperative description`.

Supported types: `feat`, `fix`, `docs`, `refactor`, `test`, `build`, `ci`, `chore`, `perf` and `revert`. Scopes are optional; prefer `app`, `scanner`, `import`, `security` or `docs` when useful.

Examples:

```text
feat(app): detect removable volumes
fix(import): retain batches when Defender fails
docs(security): explain execution blocking limitations
ci: build the WPF application on Windows
```

Keep each commit focused. Explain meaningful behavior changes and validation in the body. Mark breaking changes with `!` after the type or scope and include a `BREAKING CHANGE:` footer explaining migration.

## Validation

Run `dotnet build src/USBCondom/USBCondom.csproj -c Release` with the .NET 8 SDK on Windows. Follow `docs/operations.md` for device and Defender checks. Report checks that were not performed. Never describe the prototype as enforcing system-wide execution blocking.
