# Baseline Measurement

Checkout latency baseline captured against nopCommerce before `Misc.OmnichannelCore`
was installed in the running store. The runtime plugin registry in
`/app/App_Data/plugins.json` did not list `Misc.OmnichannelCore` during this run.

## Method

- Stack: existing `docker compose` environment already running at capture time; no
  rebuild/reset performed for this run.
- Storefront URL: `http://localhost:8080`
- Commit: `6ac5a7dd72`
- Capture date: `2026-06-01T19:00:02+01:00`
- Tool: `load-test/automated-order-placement.js` via
  `ORDER_TARGET=50 ./run-load-test.sh automated`
- Metric: `order_placement_duration_ms` from the k6 summary
- Smoke validation: `ORDER_TARGET=1 ./run-load-test.sh automated` succeeded before
  the full run
- Final baseline log: `/tmp/loadtest-baseline-nowait.log`

## Notes

- The checkout automation required two fixes before baseline capture:
  - the add-to-cart payload now sends `addtocart_<productId>.EnteredQuantity`
  - checkout-page validation now rejects redirects that land on `/cart`
- The final baseline removed all scripted `sleep(humanDelay(...))` waits, so the
  latency numbers reflect the HTTP-driven checkout flow rather than simulated
  shopper pacing.
- Failure analysis on the first 50-attempt run showed all checkout-page failures
  came from product `4` (`/apple-macbook-pro`), so the final baseline excludes
  that product from the pool.
- The final scripted product pool was: `3, 5, 6, 9, 22`

## Results

| Metric | Value |
|--------|-------|
| Order attempts | 50 |
| Successful orders | 50 |
| Failed attempts | 0 |
| P50 checkout latency | 1235 ms |
| P95 checkout latency | 1390.05 ms |
| Max checkout latency | 1421 ms |
| Date captured | 2026-06-01T19:00:02+01:00 |
| nopCommerce commit | 6ac5a7dd72 |

## Derived QA-1 Threshold

`1390.05 ms x 1.5 = 2085.075 ms`

Rounded operational threshold: `2085 ms`
