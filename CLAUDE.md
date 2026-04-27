# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**nopCommerce 5.00** — open-source ASP.NET Core 10.0 e-commerce platform. All source code lives in `src/`, with the solution file at `src/NopCommerce.sln`.

## Commands

All commands run from the repository root (where this file lives):

```bash
# Restore dependencies
dotnet restore src

# Build solution
dotnet build --no-restore src

# Run all tests
dotnet test --no-build --verbosity normal src

# Run a single test project
dotnet test src/Tests/Nop.Services.Tests/Nop.Services.Tests.csproj

# Run a specific test class or method (NUnit filter syntax)
dotnet test src/Tests/Nop.Services.Tests/ --filter "FullyQualifiedName~ProductServiceTests"
```

Docker:
```bash
docker build -t nopcommerce .
docker-compose -f postgresql-docker-compose.yml up
docker-compose -f mysql-docker-compose.yml up
```

## Architecture

### Layer Structure

```
src/Libraries/
  Nop.Core/       — Domain entities (BaseEntity), caching abstractions, config, events, helpers
  Nop.Data/       — LINQ2DB ORM, FluentMigrator migrations, multi-DB (MSSQL/PostgreSQL/MySQL)
  Nop.Services/   — 40+ business service domains (Catalog, Orders, Customers, Logging, etc.)

src/Presentation/
  Nop.Web.Framework/  — Shared MVC infrastructure: INopStartup pipeline, routing, auth, validators
  Nop.Web/            — ASP.NET Core MVC entry point; controllers, Razor views, themes, Program.cs

src/Plugins/        — 30+ dynamically-loaded plugins (Payments, Shipping, Tax, Widgets, Auth, etc.)

src/Tests/
  Nop.Tests/          — Base test infrastructure (BaseNopTest — SQLite in-memory DB, Moq)
  Nop.Services.Tests/ — Primary service-layer test suite (27 domains)
```

### Startup Pipeline (`INopStartup`)

Services and middleware are registered via classes implementing `INopStartup`, auto-discovered at startup and ordered by `Order`. To add infrastructure concerns, implement `INopStartup` and register in `ConfigureServices`/`Configure`.

Key startup classes in `src/Presentation/Nop.Web.Framework/Infrastructure/`:
- `ErrorHandlerStartup` — Order 0 (first)
- `NopStartup` — Order 2000 (registers all core services)
- `NopCommonStartup` — session, cache, routing, themes (Order 100)
- `AuthenticationStartup` / `AuthorizationStartup` — auth pipeline
- `NopEndpoints` — route registration

### Data Access

All data access goes through `IRepository<TEntity>` (LINQ2DB-backed `EntityRepository<TEntity>`). Database schema changes use FluentMigrator migrations in `src/Libraries/Nop.Data/Migrations/`. The `INopDataProvider` abstraction supports MSSQL, PostgreSQL, and MySQL — avoid writing DB-specific SQL.

### Caching

Two caching interfaces in `src/Libraries/Nop.Core/Caching/`:
- `IStaticCacheManager` — long-lived distributed or in-memory cache (Redis, SQL Server via `DistributedCacheManager`, or `MemoryCacheManager`)
- `IShortTermCacheManager` — per-request cache (`PerRequestCacheManager`)

Cache keys use `CacheKey` objects with prefix patterns; invalidate by prefix using `RemoveByPrefixAsync`.

### Event System

`IEventPublisher.PublishAsync<TEvent>()` resolves all `IConsumer<TEvent>` implementations from DI and calls `HandleEventAsync`. Entity CRUD operations automatically publish `EntityInsertedEvent<T>`, `EntityUpdatedEvent<T>`, and `EntityDeletedEvent<T>`. Implement `IConsumer<T>` and register with DI to react to any event.

### Plugin System

Plugins in `src/Plugins/` follow the naming convention `Nop.Plugin.[Category].[Name]`. Each plugin is an independent `.csproj`, loaded dynamically by `ApplicationPartManagerExtensions`. Plugins contribute controllers, views, services, and DI registrations independently.

### Configuration

`appsettings.json` → `AppSettings` singleton. Typed config sections implement `IConfig` and are accessed via `appSettings.Get<TConfig>()`. Environment-specific overrides load from `appsettings.{Environment}.json`.

### Key Conventions

- **All service methods are async** — follow `async/await` throughout
- **Entities inherit `BaseEntity`** (provides `int Id`)
- **Partial classes** are used extensively — many core classes are `partial` to allow extension without modifying originals
- **DI container**: Autofac by default (toggled via `CommonConfig.UseAutofac`); registrations go through `INopStartup` or `IInfrastructureManager`

## Assignment Context

This repository is used for an architectural evolution assignment focused on observability (see `docs/`). Key integration points for observability work: `Program.cs` (startup pipeline), `INopStartup` implementations, `Nop.Services/Logging/`, and the `Nop.Services` layer where business operations happen.
