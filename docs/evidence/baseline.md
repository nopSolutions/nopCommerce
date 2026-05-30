# Baseline Measurement

Checkout latency of **vanilla nopCommerce** (before the OmnichannelCore plugin is
installed). The QA-1 resilience gate is stated relative to this number
("checkout P95 ≤ 1.5× baseline during a 30 s WMS 503"), so it must be captured
first.

> **Status: template (plan.md Phase 1, owner Diogu).** Fill the numbers below
> from an actual run; do not leave placeholders in the final evidence pack.

## Method

- Stack: `docker compose up` with the **plugin uninstalled** (or a vanilla
  nopCommerce image).
- Storefront URL used: `http://localhost:8080` _(record exact build/commit)_.
- Load: place **50** orders through the storefront checkout (record the tool /
  script used).
- Metric: server-side checkout request latency (record how it was measured —
  e.g. app logs, reverse-proxy timing, or k6/JMeter).

## Results

| Metric | Value |
|--------|-------|
| Orders placed | _TODO_ |
| P50 checkout latency | _TODO ms_ |
| P95 checkout latency | _TODO ms_ |
| Max | _TODO ms_ |
| Date captured | _TODO_ |
| nopCommerce commit | _TODO_ |

## Derived QA-1 threshold

`P95 × 1.5 = _TODO ms_` — checkout P95 during the 30 s WMS-unavailable window
must stay at or under this value.
