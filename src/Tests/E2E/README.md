# End-to-end tests

Plain-English end-to-end tests for the storefront and admin area, executed by
[testRigor](https://testrigor.com) against a real nopCommerce instance in a real
browser.

**This directory is the source of truth for the suite.** The CI workflow runs the
testRigor CLI with `--explicit-mutations` on every push to `develop`, which makes
the remote test suite match these files exactly. Edit a case by editing a file here; never edit
it in the testRigor UI, because the next run will overwrite it.

## Layout

| Path | Purpose |
|---|---|
| `test-cases/*.yaml` | One file per test case. **The filename is the test name.** |
| `rules/*.txt` | Reusable step sequences. **The filename is the rule name and its label.** |
| `variables.json` | Stored values shared by all cases (store URL, search URL, admin credentials). |
| `scripts/install-nopcommerce.sh` | Drives the install wizard over HTTP so an instance can be provisioned unattended. |

A test case uses a rule by listing the rule's filename in its `labels:` and then
naming it as a bare step. For example a case with `- admin-login` in `labels:`
can use `admin-login` as a single step. Rules may call other rules; list those
nested rules in `labels:` too (`product-add-to-cart` calls `product-open`), so a
label run selects every case that touches them.

## Coverage

| Case | Area | Data |
|---|---|---|
| TC-01 | Home page regions, category menu, footer, newsletter signup | seeded, read-only |
| TC-02 | Search hit and miss, category browse, product detail | seeded, read-only |
| TC-03 | Register, edit info, change password, re-login, address book | creates its own |
| TC-04 | Product review submission and visibility to a guest, wishlist | own customer, seeded product |
| TC-05 | Cart quantity update, removal, persistence across logout/login | creates its own |
| TC-06 | Guest checkout: billing address, shipping method, auto-selected payment | creates its own |
| TC-07 | Admin product create, price edit, out of stock, delete | creates its own |
| TC-08 | Admin coupon discount created and redeemed by a shopper | creates its own |
| TC-09 | Registered checkout, order history details, admin cancellation seen by the customer | creates its own |
| TC-10 | Admin login rejects anonymous visitors and invalid credentials | seeded, read-only |
| TC-11 | Product attribute selection, price adjustments, options reach the cart | seeded, read-only |
| TC-12 | Non-shippable product skips the shipping steps at checkout | creates its own |
| TC-13 | Admin warehouse creation refuses an empty Country and Zip / postal code | creates its own |
| TC-14 | Account review list links back to the reviewed product page | own customer, seeded product |

## Naming

- Test cases: `TC-NN <sentence describing the journey>.yaml`. The number fixes a
  reading order; it does **not** imply an execution order. Keep the
  numbers contiguous: renumber when a case is removed.
- Rules: `<area>-<action>.txt`, for example `admin-login`, `product-add-to-cart`.

## Test data

Read-only journeys assert against the catalog seeded by `InstallSampleData`
(`Apple MacBook Pro`, `Build your own computer`). Write journeys **always create their own data** — a generated
customer via `customer-register-new`, a generated product via
`admin-product-create-new` — so no case depends on another, the sample catalog
is never mutated, and two runs can execute concurrently without colliding.

## Running locally

Requires Docker and Node.js, plus a testRigor account with a
`WebDesktopChrome` test suite.

```bash
# 1. Boot an instance from the current checkout (builds the solution; slow)
docker compose up -d --build

# 2. Install it unattended
BASE_URL=http://localhost \
NOP_RESTART_CMD="docker restart nopcommerce" \
  bash src/Tests/E2E/scripts/install-nopcommerce.sh

# 3. Run the suite against it through the localhost tunnel
npm install --global testrigor-cli
testrigor test-suite run "$TESTRIGOR_SUITE_ID" \
  --auth-token "$TESTRIGOR_AUTH_TOKEN" \
  --localhost --url "http://localhost" \
  --test-cases-path "src/Tests/E2E/test-cases/*.yaml" \
  --rules-path "src/Tests/E2E/rules/*.txt" \
  --variables-path "src/Tests/E2E/variables.json" \
  --labels nop-e2e
```

Every case in a run executes concurrently against the one instance, and no case
depends on another, so the whole suite runs in a single step.

**Iterate on a label subset, not the whole suite.** A full run is around eight
minutes; a subset is two to five. Rule filenames double as labels, and `--labels`
is repeatable, so `--labels checkout-place-order --labels admin-product-delete`
selects the five cases that touch checkout and admin product management. Every
case is still uploaded — only the matching ones run.

Add `--test-case-uuid <uuid>` to run a single case.

The install script is idempotent: it exits successfully if the store is already
installed.

### Resetting the instance

`docker compose down && docker compose up -d` returns a genuinely fresh,
uninstalled instance. Neither container mounts a volume for its data, so this
discards both the database and the generated `App_Data/appsettings.json`.

## CI configuration

`.github/workflows/e2e-tests.yml` needs one repository variable and one secret:

| Name | Kind | Value |
|---|---|---|
| `TESTRIGOR_SUITE_ID` | variable | id of a test suite dedicated to this repository |
| `TESTRIGOR_CI_TOKEN` | secret | that suite's token, from its CI/CD Integration page |

While either is unset the workflow skips the suite. GitHub does not expose
secrets to workflows triggered by pull requests from forks, so the workflow only
runs `pull_request` for branches pushed to this repository; fork contributions
are covered once their commits reach `develop`.

Only a push to `develop` passes `--explicit-mutations`, so only merged code can
rewrite the stored suite. Pull requests and manual runs execute the checked-out
files and leave it untouched.

## Notes

- **The storefront must be in English.** The install script sends
  `Country=US-en-US`, which keeps the culture at `en-US` and stops nopCommerce
  downloading a locale pack. Installing through the wizard by hand and picking
  another country produces a translated storefront that fails every assertion.
- **`/install/restartapplication` stops the container.** It calls
  `RestartAppDomain()`, which ends the process, and `docker-compose.yml` sets no
  restart policy. Pass `NOP_RESTART_CMD` so the container is cycled instead.
- **Find products in the admin list by SKU, never by name.** The list page's
  `Product name` filter is not addressable: the ajax DataTable adds a `<th>`
  with the same text, and it wins over the filter input. `Go directly to
  product SKU` is unambiguous and lands straight on the edit page.
- **Never wait on a one-page-checkout step heading.** All six headings
  (`Shipping method`, `Payment method`, ...) are in the markup from first paint,
  so such a wait returns immediately while the section is still empty. Wait on
  real step content instead.
- **The one-page-checkout Confirm button needs a scroll *and* a loose
  relation** - `scroll down until page contains "Total:"`, then
  `click button "Confirm" roughly below "Total:"`. Without the scroll the click
  lands on the accordion header and silently places no order; with strict
  `below` it resolves to nothing.
- **Never guard an admin navigation with the page's own menu word.** The left
  menu carries `Products`, `Orders` and `Discounts` on every admin page. Wait
  for something unique to the destination, such as `Add new` on a list page or
  `Order statuses` on the order list.
- **Destructive admin buttons raise a confirmation modal** and do nothing until
  it is answered. Delete confirmations repeat the verb (`Delete` / `No, cancel`)
  rather than offering `Yes`.
- **Only the header search box has an autocomplete**, and racing that dropdown
  was the largest source of flakiness in this suite. Cases that only need to
  reach a product use the search page's own field, which has none. Both boxes
  render on `/search` and both submit buttons read `Search`, so press Enter
  there rather than clicking.
- **There is no payment-method step to drive.** Seeded
  `BypassPaymentMethodSelectionIfOnlyOne` is `true` and `Check / Money Order` is
  the only active method, so checkout jumps straight to payment information.
  The published Docker images also activate `Payments.Manual` and PayPal
  Commerce - the latter unconfigured, its SDK buttons stalling the step - so
  `install-nopcommerce.sh` deactivates both. A source build ships no
  `Payments.Manual`, and the script treats that as nothing to do. The method name appears only in
  the confirm step's order review.
- **`Apple MacBook Pro` has a minimum order quantity of 2**, so a step that
  sets the quantity to `1` fails. Quantity `0` is always allowed and removes the
  line, which is how TC-05 empties the cart.
- **Nothing in the sample catalog is non-shippable**, and the seeded
  `Gift wrapping` attribute is shippable-only, so it does not appear for a
  non-shippable cart. TC-12 creates its own product for that branch.
- **Admin cards and filter panels persist their collapsed state per user**, so a
  test that clicks one open toggles it shut on the next run. The install script
  seeds the filter panels this suite depends on; never click them from a case.
