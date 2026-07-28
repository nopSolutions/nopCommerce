# Repository Guidelines

nopCommerce is an open-source ASP.NET Core e-commerce platform written in C# and targeting .NET 10. The main solution is `src/NopCommerce.sln`; the runnable web application is `src/Presentation/Nop.Web`.

Deeper project documentation lives under `context/`. Read `context/map/repo-map.md` before making changes that cross multiple modules.

## Build, Test, and Development Commands

- `dotnet restore src/NopCommerce.sln` — restore the full solution.
- `dotnet build src/NopCommerce.sln --no-restore` — build the full solution.
- `dotnet test src/Tests/Nop.Tests/Nop.Tests.csproj --no-build` — run the NUnit test suite.
- `dotnet run --project src/Presentation/Nop.Web/Nop.Web.csproj --urls http://localhost:5000` — run the web application; a configured database must already be available.

The current local baseline has one deterministic failing test: `ProductModelFactoryTests.CanPreparePriceModel`. Treat it as an observed baseline failure until its cause is investigated.

## Local Development Notes

- Local development requires a configured database; PostgreSQL 16 in Docker is a verified setup.
- For PostgreSQL, the target database must have the `citext` and `pgcrypto` extensions enabled before nopCommerce migrations run.
- If the database was created externally rather than by the nopCommerce installer, verify the required extensions manually.
- Local database settings are stored in `src/Presentation/Nop.Web/App_Data/dataSettings.json`; this file is ignored by Git and must not be committed.
- Plugin state is stored in `src/Presentation/Nop.Web/App_Data/plugins.json`; this file is also local and ignored by Git.
- Windows Smart App Control may block locally built plugin assemblies. Do not disable system-wide security controls as a default fix; disable only the affected local plugin when necessary.
- See `context/foundation/local-development.md` for the complete setup and troubleshooting guide.

## Project Structure

- `src/Libraries/Nop.Core` — shared domain entities, abstractions, and core infrastructure concepts.
- `src/Libraries/Nop.Data` — data access, data providers, and database migrations.
- `src/Libraries/Nop.Services` — application and business services.
- `src/Presentation/Nop.Web.Framework` — shared ASP.NET Core web infrastructure.
- `src/Presentation/Nop.Web` — storefront and administration web application.
- `src/Plugins` — optional features and external integrations.
- `src/Tests/Nop.Tests` — automated tests.

The main intended dependency direction is: `Nop.Web` → `Nop.Services` → `Nop.Data` → `Nop.Core`.

Verify actual project references, plugin loading, and runtime dependency injection before making architectural changes. Do not introduce reverse project references without explicit justification.

## Coding Conventions

- Follow `.editorconfig`.
- Preserve the conventions used by neighboring code.
- Async methods use the `Async` suffix.
- New plugins follow the `Nop.Plugin.<Group>.<Name>` naming convention.
- Do not move integration-specific logic into core libraries without an explicit architectural reason.

## Testing Guidelines

- Tests use NUnit and live under `src/Tests/Nop.Tests`.
- Run focused tests while developing and the relevant broader suite before finishing.
- Do not describe the complete suite as green while the documented baseline failure remains unresolved.
- Add characterization tests before changing behavior that is poorly covered or difficult to infer.

## Working with the Repository

1. Read `context/map/repo-map.md`.
2. Identify the affected module and likely blast radius.
3. Inspect existing tests and relevant Git history.
4. Record unknowns instead of treating assumptions as facts.
5. Propose a small, verifiable plan before modifying cross-module behavior.

## Commit and Pull Request Guidelines

Follow `CONTRIBUTING.md` and current repository history for branch, commit, issue, and pull-request conventions. Pull requests target `develop`.