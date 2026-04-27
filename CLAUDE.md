# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Restore packages
dotnet restore src

# Build solution
dotnet build src/NopCommerce.sln --configuration Release

# Run the web app (development)
dotnet run --project src/Presentation/Nop.Web/Nop.Web.csproj

# Publish for deployment
dotnet publish src/Presentation/Nop.Web/Nop.Web.csproj -c Release -o /app/published
```

Docker alternatives (MSSQL, MySQL, PostgreSQL):
```bash
docker-compose up
docker-compose -f mysql-docker-compose.yml up
docker-compose -f postgresql-docker-compose.yml up
```

## Tests

```bash
# Run all tests
dotnet test src

# Run tests for a specific project
dotnet test src/Tests/Nop.Tests/Nop.Tests.csproj --configuration Release

# Run a single test by name filter
dotnet test src/Tests/Nop.Tests/Nop.Tests.csproj --filter "FullyQualifiedName~TestClassName.MethodName"
```

Test stack: NUnit 4.x, Moq, AwesomeAssertions, SQLite (in-memory for test DB).

## Technology Stack

- **.NET 10** (pinned via `global.json`, SDK `10.0.100`)
- **ASP.NET Core 9.0** for the web layer
- **Autofac** for dependency injection (registered via `IPlugin`, `IDependencyRegistrar` interfaces)
- **Entity Framework Core** for data access (MSSQL, MySQL, PostgreSQL supported)
- **AutoMapper 14** for DTO/model mapping
- **Redis** optional distributed cache

## Architecture

The solution (`src/NopCommerce.sln`) is layered:

```
Libraries/
  Nop.Core       — Entities, domain interfaces, caching abstractions, events, helpers
  Nop.Data       — EF Core mappings, migrations, repository pattern
  Nop.Services   — Business logic (one service class per domain area)

Presentation/
  Nop.Web            — Main ASP.NET Core app: public storefront + /Admin area
  Nop.Web.Framework  — Base controllers, model binders, filters, routing helpers

Plugins/           — 30+ pluggable features (payment, shipping, tax, widgets, auth…)

Tests/
  Nop.Tests      — Unit/integration tests (mirrors the Libraries structure)
```

### Key conventions

- **Plugin system**: every plugin implements `IPlugin` and has its own `Plugin/` directory under `src/Plugins/`. Plugins are compiled separately and dropped into `Nop.Web/Plugins/` at runtime.
- **Services**: business logic lives exclusively in `Nop.Services`. Controllers call services; services call repositories or other services.
- **Factories**: in `Nop.Web`, `*ModelFactory` classes map domain objects → view models. Avoid doing mapping directly in controllers.
- **Events**: `IEventPublisher` / `IConsumer<TEvent>` are used for loosely coupled cross-cutting concerns (cache invalidation, audit logs, etc.).
- **Settings**: strongly-typed settings classes extend `ISettings` and are persisted in the database via `ISettingService`.
- **Admin area**: lives under `src/Presentation/Nop.Web/Areas/Admin/`.

### Database / Migrations

- Migrations are managed by **FluentMigrator** (not EF migrations). Migration classes live in `Nop.Data/Migrations/`.
- Each supported database (MSSQL, MySQL, PostgreSQL) may have engine-specific migration sub-classes.

## CI

GitHub Actions (`.github/workflows/dotnet.yml`) runs on `develop` push/PR:  
`Setup .NET 10 → dotnet restore → dotnet build (Release) → dotnet test`
