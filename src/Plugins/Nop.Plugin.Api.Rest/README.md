Nop.Plugin.Api.Rest

A minimal nopCommerce plugin that exposes basic REST API endpoints for products, orders, customers and sales.

Installation
1. Add the project to the solution: dotnet sln src\NopCommerce.sln add Plugins\Nop.Plugin.Api.Rest\Nop.Plugin.Api.Rest.csproj
2. Build the solution.
3. In the nopCommerce admin, install and enable the plugin (Configuration -> Local plugins).

Plugin-contained Swagger
This plugin registers Swagger and serves the UI under a plugin-specific prefix so no host changes are required.
- Swagger UI: /swagger/api-rest/index.html
- Swagger JSON: /swagger/v1/swagger.json

API key and rate limiting
The plugin includes middleware that enforces an optional API key and per-client rate limiting.
Configuration (appsettings or environment variables):

- Plugins:ApiRest:ApiKey: when set, the plugin requires the header X-Api-Key with the matching value for API requests. Write operations always require it and are refused with 401 when it is not configured.
- Plugins:ApiRest:RateLimitPerMinute: integer, default 60.

Example appsettings.Development.json snippet:

{
  "Plugins": {
    "ApiRest": {
      "ApiKey": "your_test_api_key",
      "RateLimitPerMinute": "60"
    }
  }
}

Products
GET /api/rest/products returns a paged list of published products as ProductDto (Id, Name, Sku, Price).
GET /api/rest/products/{id} returns a single product as ProductDetailDto, which adds the editable
catalog properties to that list.

Write operations require the X-Api-Key header and accept only the core product properties. Categories,
manufacturers, tags, pictures, attributes and inventory per warehouse are managed from the admin area.

- POST /api/rest/products creates a product and returns 201 with the created ProductDetailDto. Name is required.
- PATCH /api/rest/products/{id} updates the supplied properties only and leaves the omitted ones untouched, so a
  caller clears a string field by sending an empty string rather than null.
- DELETE /api/rest/products/{id} soft deletes a product and returns 204. A second call returns 404.

Postman collection
A sample Postman collection is included at Plugins/Nop.Plugin.Api.Rest/postman/Nop.Plugin.Api.Rest.postman_collection.json. Set the collection variable `base_url` and `api_key` before running.

Notes
- The plugin applies API-key & rate-limit middleware for requests under /api/rest only. The plugin Swagger UI is excluded from these checks.
- For production, secure your API keys and consider more robust rate-limiting (distributed) and authentication (JWT/OAuth).
- Avoid returning domain entities directly; the plugin uses DTOs to define the public contract.
