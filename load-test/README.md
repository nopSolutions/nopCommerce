# nopCommerce Load Test

K6 scripts for generating storefront traffic and placing real orders through the nopCommerce web endpoints.

This folder is intentionally self-contained. It does not start Docker, `docker compose`, or any supporting services for you.

## Prerequisites

- `k6` installed
- A running nopCommerce instance for this repo
- Sample products and checkout settings configured so guest checkout can place orders

## Default target

The scripts default to:

```bash
http://localhost:8080
```

Override with:

```bash
BASE_URL=http://localhost:5000 k6 run automated-order-placement.js
```

## Scripts

- `automated-order-placement.js`: full add-to-cart + one-page-checkout + order placement flow
- `simple-order-test.js`: smoke test that generates HTTP traffic only
- `verify-nopcommerce-config.sh`: basic readiness checks for automated ordering
- `run-load-test.sh`: convenience wrapper around `k6`

## Usage

```bash
cd load-test
./verify-nopcommerce-config.sh
./run-load-test.sh automated
```

Or run directly:

```bash
cd load-test
k6 run automated-order-placement.js
```

## Required nopCommerce settings

- Guest checkout enabled
- One-page checkout enabled
- At least one working payment method
- Sample products with stock available

If your store does not use the default sample data, update the product IDs and SEO URLs in `automated-order-placement.js`.
