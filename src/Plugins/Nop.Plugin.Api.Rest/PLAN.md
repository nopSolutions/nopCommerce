# Nop.Plugin.Api.Rest — Implementation Plan

This document records the plan to add a REST API plugin for nopCommerce. Proceed when ready; waiting ~10 minutes as requested before making further changes.

## Overview
- Plugin name: Nop.Plugin.Api.Rest
- Purpose: expose REST endpoints for Products, Orders, Customers, Sales (read and write), plus Swagger documentation and optional auth (JWT/API key).
- Target: .NET 10 nopCommerce solution (Razor Pages site)

## Goals
1. Provide well-scoped, secure, versioned API endpoints: /api/rest/*
2. Offer Swagger UI at /swagger for discoverability
3. Keep plugin self-contained and align with nopCommerce plugin model
4. Provide DTOs and mapping to avoid returning domain entities directly
5. Add basic tests and sample Postman collection

## Deliverables
- Plugin project: Plugins/Nop.Plugin.Api.Rest
- Controllers: ProductsController, OrdersController, CustomersController, SalesController
- DTO models under Models/
- Service registration helper (AddNopApiRest)
- README.md and PLAN.md (this file)
- Optional: PowerShell scaffold script and Swagger registration snippet for Nop.Web

## Implementation Steps
1. Scaffold plugin project and add to solution
2. Implement basic read-only endpoints (GET) for products, orders, customers, sales
3. Add DTOs and mapping; avoid leaking domain types
4. Integrate Swagger: prefer registering in host Nop.Web Program.cs
5. Add authentication: JWT or API key (configurable via plugin settings)
6. Extend endpoints for create/update/delete with validation and business rules
   - Done for products (POST/PATCH/DELETE on /api/rest/products), write operations require the X-Api-Key header.
7. Add integration tests and a Postman collection
8. Performance checks: ensure large queries use repository-level filters, not in-memory LINQ

## Security & Hardening
- Require Authorization for mutation endpoints
- Rate-limit or API-key for public endpoints if needed
- Validate all input and avoid over-posting

## Testing & Validation
- Unit tests for mapping and input validation
- Integration tests exercising endpoints against an in-memory or test DB
- Postman collection for manual validation

## Deployment & Admin
- Build and install via nopCommerce admin (Configuration → Local plugins)
- Document plugin settings and required AppSettings (JWT secrets, API key)

## Notes / Constraints
- Plugin references existing nopCommerce projects; ensure ProjectReference paths match repo layout
- For large datasets, use repository queries or paging to avoid loading everything into memory

## Next actions (when ready)
- Wait ~10 minutes (per request) then proceed to scaffold remaining controllers and add Swagger integration in Nop.Web, or proceed on user confirmation.

---
Timestamp: {TIMESTAMP}

