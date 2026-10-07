#!/usr/bin/env bash
# Scripted nopCommerce installation.
#
# nopCommerce refuses to serve the storefront until its interactive install
# wizard has run, so this drives that wizard over HTTP to provision an instance
# unattended. Country=US-en-US keeps the culture at en-US, so no locale pack is
# downloaded and the storefront renders in English; SubscribeNewsletters=false
# avoids an outbound call to nopcommerce.com.
set -euo pipefail

BASE_URL="${BASE_URL:-http://localhost}"
ADMIN_EMAIL="${NOP_ADMIN_EMAIL:-admin@yourStore.com}"
ADMIN_PASSWORD="${NOP_ADMIN_PASSWORD:-admin}"
DB_SERVER="${NOP_DB_SERVER:-nopcommerce_database}"
DB_NAME="${NOP_DB_NAME:-nopCommerce}"
DB_USER="${NOP_DB_USER:-sa}"
DB_PASSWORD="${NOP_DB_PASSWORD:-nopCommerce_db_password}"
INSTALL_SAMPLE_DATA="${NOP_INSTALL_SAMPLE_DATA:-true}"

COOKIES="$(mktemp)"
trap 'rm -f "$COOKIES"' EXIT

log() { printf '[install] %s\n' "$*"; }

# --- 1. Wait for the app, then decide install vs. already-installed ------
# Both checks live in one loop on purpose: once the store is installed /install
# 302s to the homepage, so a loop that waits for /install to return 200 would
# spin for the full timeout on an already-installed instance and make any
# "already installed" branch after it dead code.
log "waiting for app at $BASE_URL"
installed=""
for i in $(seq 1 90); do
  if [ "$(curl -s -o /dev/null -w '%{http_code}' -m 10 "$BASE_URL/" || true)" = "200" ]; then
    log "store already installed (after ${i} attempt(s))"
    installed="yes"
    break
  fi
  code="$(curl -s -o /dev/null -w '%{http_code}' -m 10 "$BASE_URL/install" || true)"
  if [ "$code" = "200" ]; then
    log "installer is up (after ${i} attempt(s))"
    break
  fi
  if [ "$i" = "90" ]; then
    log "FATAL: neither storefront nor installer became reachable (last HTTP $code)"
    exit 1
  fi
  sleep 5
done

# --- Seed admin UI preferences ------------------------------------------
# Every admin list page keeps its filter panel's collapsed state in a per-admin
# preference, and a collapsed panel hides the filter labels a step needs as
# anchors. The state is persisted through Preferences/SavePreference, so a test
# that clicked a panel open would toggle it shut on the next run; the panels are
# seeded open here instead. Over HTTP rather than SQL, so this works against any
# deployment.
seed_admin_preferences() {
  jar="$(mktemp)"

  tok="$(curl -s -c "$jar" -m 30 "$BASE_URL/login" \
    | sed 's/</\n</g' \
    | grep '__RequestVerificationToken' \
    | sed -E 's/.*value="([^"]+)".*/\1/' \
    | head -1)"

  curl -s -o /dev/null -b "$jar" -c "$jar" -m 30 -X POST "$BASE_URL/login" \
    --data-urlencode "__RequestVerificationToken=$tok" \
    --data-urlencode "Email=$ADMIN_EMAIL" \
    --data-urlencode "Password=$ADMIN_PASSWORD"

  tok="$(curl -s -b "$jar" -c "$jar" -m 30 "$BASE_URL/Admin/Setting/GeneralCommon" \
    | sed 's/</\n</g' \
    | grep '__RequestVerificationToken' \
    | sed -E 's/.*value="([^"]+)".*/\1/' \
    | head -1)"

  set_pref() {
    if curl -s -b "$jar" -m 30 -X POST "$BASE_URL/Admin/Preferences/SavePreference" \
        --data-urlencode "__RequestVerificationToken=$tok" \
        -d "name=$1" -d "value=$2" | grep -q '"Result":true'; then
      log "admin preference set: $1=$2"
      return 0
    fi
    log "WARNING: could not set admin preference $1=$2 -- $3"
    return 1
  }

  # Not hypothetical: TC-09 began failing on "Order statuses" after the panel
  # was left closed mid-session.
  set_pref "OrdersPage.HideSearchBlock" "false" \
    "TC-09 will fail on Order statuses"
  set_pref "ProductsPage.HideSearchBlock" "false" \
    "TC-07 and TC-12 will fail on Go directly to product SKU"

  # The published Docker images ship PayPal Commerce active but unconfigured -
  # paypalcommercesettings.clientid and .secretkey are both empty - and
  # DisplayButtonsOnPaymentMethod is on, so its SDK buttons render on the
  # payment-method step and checkout cannot advance past it. A source build
  # does not activate the plugin at all, and it cannot be exercised without
  # credentials, so it is deactivated here, and so is Manual below.
  # MethodUpdate rewrites the descriptor from what is posted, so the plugin's
  # own FriendlyName and DisplayOrder are sent back unchanged.
  #
  # A source build ships fewer payment plugins than the images, and
  # MethodUpdate answers 500 for one that is not there. Only then is the active
  # list consulted, to tell that harmless case from a real failure. The list
  # endpoint returns active methods only, so it cannot tell "absent" from
  # "inactive", and neither matters to checkout.
  deactivate_payment_method() {
    code="$(curl -s -o /dev/null -w '%{http_code}' -b "$jar" -m 30 \
      -X POST "$BASE_URL/Admin/Payment/MethodUpdate" \
      --data-urlencode "__RequestVerificationToken=$tok" \
      -d "SystemName=$1" -d "FriendlyName=$2" -d "DisplayOrder=$3" \
      -d "IsActive=false")"
    if [ "$code" = "200" ]; then
      log "payment method deactivated: $1"
      return
    fi
    active="$(curl -s -b "$jar" -m 30 -X POST "$BASE_URL/Admin/Payment/Methods" \
      --data-urlencode "__RequestVerificationToken=$tok" \
      -d "Start=0" -d "Length=100" -d "Draw=1" || true)"
    if printf '%s' "$active" | grep -q '"Data":' \
      && ! printf '%s' "$active" | grep -q "\"SystemName\":\"$1\""; then
      log "payment method not active in this build, nothing to do: $1"
    else
      log "WARNING: could not deactivate $1 (HTTP $code) -- $4"
    fi
  }

  deactivate_payment_method "Payments.PayPalCommerce" "PayPal Commerce" "-1" \
    "TC-06, TC-09 and TC-12 will stall on the payment-method step"

  # Manual (credit card) is the second method the images activate. It needs a
  # configured gateway to mean anything, and leaving it active keeps the
  # payment-method step on screen; with Check / Money Order alone,
  # BypassPaymentMethodSelectionIfOnlyOne skips that step, which is the shape
  # the checkout cases were written against.
  deactivate_payment_method "Payments.Manual" "Credit Card" "1" \
    "checkout stops on a payment-method step the cases must click through"

  rm -f "$jar"
}

if [ -n "$installed" ]; then
  log "nothing to install; refreshing admin preferences only"
  seed_admin_preferences
  exit 0
fi

# --- 2. Scrape the antiforgery token ------------------------------------
# InstallController is decorated with [AutoValidateAntiforgeryToken], so the
# POST needs both the hidden form field and its paired cookie.
log "fetching antiforgery token"
scrape_token() {
  # Split tags onto separate lines so the value= match cannot run past the
  # token's own <input>. (tr cannot: it maps one char to one char.)
  curl -s -c "$COOKIES" -m 30 "$BASE_URL/install" \
    | sed 's/</\n</g' \
    | grep '__RequestVerificationToken' \
    | sed -E 's/.*value="([^"]+)".*/\1/' \
    | head -1
}
token="$(scrape_token)"

# A real ASP.NET Core antiforgery token is >100 chars. Anything shorter means
# the scrape matched the wrong attribute and the POST would fail with a 400.
if [ "${#token}" -lt 50 ]; then
  log "FATAL: implausible __RequestVerificationToken (${#token} chars)"
  exit 1
fi
log "token acquired (${#token} chars)"

# --- 3. Submit the install form -----------------------------------------
# Retried because MSSQL commonly refuses connections for the first ~30-60s
# even after its container reports as started.
BODY_FILE="$(mktemp)"
trap 'rm -f "$COOKIES" "$BODY_FILE"' EXIT
installed=false

for attempt in 1 2 3 4 5; do
  log "submitting install form (attempt $attempt)"
  status="$(curl -s -o "$BODY_FILE" -w '%{http_code}' \
    -b "$COOKIES" -c "$COOKIES" -m 900 -X POST "$BASE_URL/install" \
    --data-urlencode "__RequestVerificationToken=$token" \
    --data-urlencode "AdminEmail=$ADMIN_EMAIL" \
    --data-urlencode "AdminPassword=$ADMIN_PASSWORD" \
    --data-urlencode "ConfirmPassword=$ADMIN_PASSWORD" \
    --data-urlencode "DataProvider=SqlServer" \
    --data-urlencode "ConnectionStringRaw=false" \
    --data-urlencode "ConnectionString=" \
    --data-urlencode "ServerName=$DB_SERVER" \
    --data-urlencode "DatabaseName=$DB_NAME" \
    --data-urlencode "IntegratedSecurity=false" \
    --data-urlencode "Username=$DB_USER" \
    --data-urlencode "Password=$DB_PASSWORD" \
    --data-urlencode "CreateDatabaseIfNotExists=true" \
    --data-urlencode "InstallSampleData=$INSTALL_SAMPLE_DATA" \
    --data-urlencode "SubscribeNewsletters=false" \
    --data-urlencode "Country=US-en-US" \
    --data-urlencode "UseCustomCollation=false" \
    --data-urlencode "Collation=" || echo 000)"

  # 400 means the antiforgery check rejected us, not that the install failed.
  if [ "$status" = "400" ]; then
    log "HTTP 400 - antiforgery rejected; re-scraping token"
    token="$(scrape_token)"
    sleep 5
    continue
  fi

  if [ "$status" != "200" ] || grep -qi 'validation-summary-errors\|Setup failed' "$BODY_FILE"; then
    log "install did not succeed (HTTP $status):"
    sed 's/</\n</g' "$BODY_FILE" \
      | grep -i -A3 'validation-summary-errors\|Setup failed' \
      | sed -E 's/<[^>]*>//g' \
      | grep -v '^[[:space:]]*$' \
      | head -10 || true
    # A failed install resets the saved DataConfig, so the token must be re-read.
    token="$(scrape_token)"
    sleep 20
    continue
  fi

  log "install completed"
  installed=true
  break
done

if [ "$installed" != true ]; then
  log "FATAL: install did not complete after 5 attempts"
  exit 1
fi

# --- 4. Restart so the app picks up the new DataConfig -------------------
# /install/restartapplication ends the process and relies on a process manager
# to bring it back. docker-compose.yml declares no restart policy, so callers
# running in a container pass NOP_RESTART_CMD instead.
if [ -n "${NOP_RESTART_CMD:-}" ]; then
  log "restarting via NOP_RESTART_CMD"
  bash -c "$NOP_RESTART_CMD"
else
  log "restarting via /install/restartapplication"
  curl -s -o /dev/null -b "$COOKIES" -m 60 "$BASE_URL/install/restartapplication" || true
fi

# --- 5. Confirm the storefront is live ----------------------------------
log "waiting for storefront"
for i in $(seq 1 90); do
  code="$(curl -s -o /dev/null -w '%{http_code}' -m 15 "$BASE_URL/" || true)"
  if [ "$code" = "200" ]; then
    log "storefront is live"
    seed_admin_preferences
    exit 0
  fi
  sleep 5
done

log "FATAL: storefront did not come up (last HTTP $code)"
exit 1
