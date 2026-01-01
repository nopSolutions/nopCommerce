# Order Creation Workflow

This document describes the full order creation workflow implemented in the AliExpress SDK Console Harness.

## Overview

The `create-order` command executes a complete end-to-end workflow for creating an order on AliExpress:

1. **Product Search** - Search for products using a keyword
2. **Product Details** - Get detailed information about the selected product
3. **Freight Estimation** - Calculate shipping costs and options
4. **Order Creation** - Create the actual order

## Usage

```bash
cd dotnet/AliExpressSdk.ConsoleHarness
DOTNET_ENVIRONMENT=Development dotnet run -- create-order "Canvas Kung Fu Shoes"
```

## Configuration

The workflow is configured for South Africa with the following defaults:

- **Country Code**: ZA
- **Currency**: ZAR
- **Shipping Address**: 22 Guildford Road, Parklands, Cape Town, Western Cape 7441

These values are hardcoded in the `CreateOrderWorkflowCommand.cs` class.

## API Calls Made

### 1. Product Text Search (aliexpress.ds.text.search)

Searches for products matching the keyword with South Africa-specific parameters.

**Request Parameters:**
- `keyWord`: Search term
- `local`: "en_US"
- `countryCode`: "ZA"
- `currency`: "ZAR"
- `pageSize`: "10"
- `pageIndex`: "1"

### 2. Product Details (aliexpress.ds.product.get)

Retrieves detailed information about the product, including SKU options.

**Request Parameters:**
- `product_id`: Extracted from search results
- `ship_to_country`: "ZA"
- `target_currency`: "ZAR"
- `target_language`: "en"

### 3. Freight Estimation (aliexpress.ds.freight.query)

Gets shipping options and costs for the selected product.

**Request Parameters:**
```json
{
  "quantity": 1,
  "shipToCountry": "ZA",
  "productId": "<product_id>",
  "provinceCode": "Western Cape",
  "cityCode": "Cape Town",
  "selectedSkuId": "<sku_id>",
  "language": "en_US",
  "currency": "ZAR",
  "locale": "en_US"
}
```

### 4. Order Creation (aliexpress.ds.order.create)

Creates the order with the selected product and shipping details.

**Request Parameters:**
```json
{
  "ds_extend_request": {
    "payment": {
      "pay_currency": "ZAR"
    }
  },
  "param_place_order_request4_open_api_d_t_o": {
    "out_order_id": "<unique_guid>",
    "logistics_address": {
      "address": "22 Guildford Road",
      "city": "Cape Town",
      "contact_person": "AliExpress Test",
      "country": "ZA",
      "full_name": "Test Customer",
      "mobile_no": "0211234567",
      "phone_country": "+27",
      "phone_number": "0211234567",
      "province": "Western Cape",
      "zip": "7441"
    },
    "product_items": [
      {
        "product_id": <product_id>,
        "product_count": 1,
        "sku_attr": "<sku_attributes>",
        "logistics_service_name": "<logistics_code>"
      }
    ]
  }
}
```

## Response Persistence

All API requests and responses are saved in two locations:

1. **SampleResponses/** - Human-readable format for documentation
   - `product-search/request.json` and `response.json`
   - `product-details/request.json` and `response.json`
   - `freight-estimation/request.json` and `response.json`
   - `order-create/request.json` and `response.json`

2. **api-calls/** - Timestamped logs for debugging
   - `aliexpress-ds-text-search/request.json` and `response.json`
   - `aliexpress-ds-product-get/request.json` and `response.json`
   - `aliexpress-ds-freight-query/request.json` and `response.json`
   - `aliexpress-ds-order-create/request.json` and `response.json`

## Example Output

```
================================================================================
AliExpress Order Creation Workflow
================================================================================
Search keyword: Canvas Kung Fu Shoes
Ship to: ZA (ZAR)
Address: 22 Guildford Road, Parklands, Cape Town, Western Cape 7441
================================================================================

Step 1: Searching for products...
  Search request succeeded.
✓ Found product ID: 1005008839327869

Step 2: Getting product details...
  Product details request succeeded.
✓ Found SKU ID: 12000046899686894
  SKU Attributes: 14:350850#white501;200000124:200000333

Step 3: Getting freight estimation...
  Freight estimation request succeeded.
✓ Logistics service: CAINIAO_FULFILLMENT_STD

Step 4: Creating order...
  Order creation request succeeded.
  Response:
  {
  "aliexpress_ds_order_create_response": {
    "result": {
      "is_success": true,
      "order_list": {
        "number": [
          3066593483588246
        ]
      }
    },
    "request_id": "210142ba17671293421261046",
    "_trace_id_": "2101364017671293421261805e5393"
  }
}

================================================================================
✓ Order workflow completed successfully!
================================================================================
```

## API Models

The SDK now includes strongly-typed models for all API requests:

- **ProductSearchRequest** - For text search API
- **ProductDetailsRequest** - For product details API
- **FreightQueryRequest** - For freight estimation API
- **OrderCreateRequest** - For order creation API

These models are located in:
- `dotnet/AliExpressSdk/Models/ProductSearch/`
- `dotnet/AliExpressSdk/Models/ProductDetails/`
- `dotnet/AliExpressSdk/Models/Freight/`
- `dotnet/AliExpressSdk/Models/Order/`

Each model includes a `ToDictionary()` method for easy conversion to API parameters.

## Notes

- The order creation does **not** automatically process payment. The order is created but requires manual payment completion.
- This is safe for testing as orders can be cancelled before payment.
- API rate limits may apply - the command includes error handling for rate limit errors.
