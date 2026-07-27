# Repository Guidelines

nopCommerce is an open-source ASP.NET Core e-commerce platform written in C# targeting .NET 10. The solution is `src/NopCommerce.sln`; the runnable web app is `Nop.Web`. Deeper, project-specific documentation lives under `@src/context/`.

## Build, Test, and Development Commands
- `./run.ps1` — primary local entrypoint. Starts the PostgreSQL 16 Docker container and runs `Nop.Web` at `http://localhost:5000`. Flags: `-Port <n>`, `-SkipDb`.
- `dotnet build src/NopCommerce.sln` — build the full solution. Requires the .NET 10 SDK pinned in `@global.json`.
- `dotnet test src/Tests/Nop.Tests` — run the test suite (NUnit).
- `dotnet run --project src/Presentation/Nop.Web` — run the app directly, without the `run.ps1` wrapper (bring your own database).

On first run the app serves a web installer; point it at the running Postgres instance to seed the schema and sample data.

## Project Structure & Module Organization
- `src/Libraries/` — `Nop.Core` (domain, infrastructure), `Nop.Data` (data access, FluentMigrator migrations), `Nop.Services` (business logic).
- `src/Presentation/` — `Nop.Web` (MVC store + admin) and `Nop.Web.Framework` (shared web infrastructure).
- `src/Plugins/` — 32 `Nop.Plugin.<Group>.<Name>` projects; keep feature-specific integrations here, never in the core libraries.
- `src/Tests/Nop.Tests` — the single test project.

Project references flow one way: `Nop.Web` → `Nop.Services` → `Nop.Data` → `Nop.Core`. Do not add a reference in the reverse direction.

## Coding Style & Naming Conventions
Style is enforced by `@src/.editorconfig`; do not override it per file. Async methods end in `Async`. New plugins use the `Nop.Plugin.<Group>.<Name>` folder and assembly name.

## Testing Guidelines
Tests use NUnit in `src/Tests/Nop.Tests`, organized to mirror the namespace of the code under test. Run one class with `dotnet test src/Tests/Nop.Tests --filter <ClassName>`.

## Commit & Pull Request Guidelines
Prefix each commit subject with its GitHub issue number, e.g. `#8247 <summary>` (see `git log`). Create one branch per issue as `issue-<n>-<slug>` and open pull requests against `develop`. Reference the issue in the PR description; see `@CONTRIBUTING.md`.
