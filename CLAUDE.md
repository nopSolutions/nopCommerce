# CLAUDE.md - nopCommerce Architecture Guide

This document provides a high-level overview of the nopCommerce architecture for AI assistants and developers.

## What is nopCommerce?

nopCommerce is a free, open-source e-commerce platform built on ASP.NET Core. It supports multi-store, multi-language, and multi-vendor scenarios with a highly extensible plugin architecture.

## Solution Structure

```
src/
├── Libraries/
│   ├── Nop.Core          # Domain entities, interfaces, infrastructure
│   ├── Nop.Data          # Data access layer (LinqToDB ORM)
│   └── Nop.Services      # Business logic services
├── Presentation/
│   ├── Nop.Web           # Main web application (public store + admin)
│   └── Nop.Web.Framework # Common web infrastructure
└── Plugins/              # 28+ extensibility plugins
```

## Layered Architecture

```
┌─────────────────────────────────────────────────────┐
│  Presentation (Nop.Web, Nop.Web.Framework)          │
├─────────────────────────────────────────────────────┤
│  Services (Nop.Services)                            │
├─────────────────────────────────────────────────────┤
│  Data Access (Nop.Data)                             │
├─────────────────────────────────────────────────────┤
│  Core/Domain (Nop.Core)                             │
└─────────────────────────────────────────────────────┘
```

## Key Components

### Core Layer (Nop.Core)
- **BaseEntity** (`BaseEntity.cs`) - Base class for all entities
- **Domain Entities** (`Domain/`) - 100+ entities organized by feature (Catalog, Customers, Orders, etc.)
- **NopEngine** (`Infrastructure/NopEngine.cs`) - Central service resolution and configuration
- **INopStartup** (`Infrastructure/INopStartup.cs`) - Startup configuration contract
- **IWorkContext** - Current user, language, currency context
- **IEventPublisher** (`Events/`) - Publish/subscribe event system

### Data Layer (Nop.Data)
- **IRepository<T>** - Generic repository with caching and event publishing
- **EntityRepository<T>** - Implementation using LinqToDB
- **Entity Builders** (`Mapping/Builders/`) - FluentMigrator schema definitions

### Services Layer (Nop.Services)
- Interface-based services mirroring domain structure
- Examples: `IProductService`, `ICustomerService`, `IOrderService`
- Plugin management: `IPlugin`, `BasePlugin`, `PluginDescriptor`

### Presentation Layer
- **Program.cs** - Application entry point and bootstrap
- **Areas/Admin** - Administration interface
- **Factories** - ViewModel construction (Factory pattern)
- **Validators** - FluentValidation integration

## Architectural Patterns

| Pattern | Implementation |
|---------|----------------|
| Repository | `IRepository<T>` with caching & events |
| Dependency Injection | Autofac or built-in, via `INopStartup` |
| Event-Driven | `IEventPublisher` with auto-discovered consumers |
| Factory | Model factories for ViewModel construction |
| Plugin | `IPlugin` interface with dynamic loading |

## Plugin System

Plugins extend functionality without modifying core code:
- **Payments**: `IPaymentMethod` (PayPal, Manual, etc.)
- **Shipping**: `IShippingRateComputationMethod` (UPS, Fixed rates)
- **Tax**: `ITaxProvider` (Avalara, Fixed rates)
- **Widgets**: `IWidgetPlugin` (Google Analytics, Facebook Pixel)
- **Search**: `ISearchProvider` (Lucene)

Plugin structure: `plugin.json` metadata + main class inheriting `BasePlugin`

## Bootstrap Sequence

1. Load configuration (`appsettings.json`)
2. Configure DI container (Autofac or built-in)
3. Discover and execute `INopStartup.ConfigureServices()` by order
4. Build application
5. Execute `INopStartup.Configure()` for middleware
6. Publish `AppStartedEvent`

## Cross-Cutting Concerns

- **Caching**: `IStaticCacheManager`, `IShortTermCacheManager`
- **Localization**: `ILocalizationService`, `ILocalizedEntityService`
- **Security**: `IAclService`, `IPermissionService`
- **Logging**: `ILogger`, `ICustomerActivityService`

## Common Development Tasks

### Adding a new entity
1. Create entity class in `Nop.Core/Domain/{Feature}/`
2. Create entity builder in `Nop.Data/Mapping/Builders/{Feature}/`
3. Create service interface and implementation in `Nop.Services/{Feature}/`
4. Register service in an `INopStartup` implementation

### Creating a plugin
1. Create project in `src/Plugins/Nop.Plugin.{Group}.{Name}/`
2. Add `plugin.json` with metadata
3. Create main class inheriting `BasePlugin` and implementing feature interface
4. Implement `InstallAsync()` and `UninstallAsync()`

## Key File Locations

| Purpose | Location |
|---------|----------|
| Entry point | `src/Presentation/Nop.Web/Program.cs` |
| Service registration | `src/Presentation/Nop.Web.Framework/Infrastructure/NopStartup.cs` |
| Domain entities | `src/Libraries/Nop.Core/Domain/` |
| Repository interface | `src/Libraries/Nop.Data/IRepository.cs` |
| Business services | `src/Libraries/Nop.Services/` |
| Admin controllers | `src/Presentation/Nop.Web/Areas/Admin/Controllers/` |
| Plugin examples | `src/Plugins/` |
