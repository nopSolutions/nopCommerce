#!/bin/bash

set -e

BASE_URL="${BASE_URL:-http://localhost:8080}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo ""
echo "========================================"
echo "nopCommerce Configuration Checker"
echo "========================================"
echo ""
echo "Target: $BASE_URL"
echo ""

echo "Check 1: Verifying nopCommerce is accessible..."
if curl -s -o /dev/null -w "%{http_code}" "$BASE_URL" | grep -q "200"; then
    echo -e "${GREEN}OK${NC} nopCommerce is accessible at $BASE_URL"
else
    echo -e "${RED}ERROR${NC} nopCommerce is NOT accessible at $BASE_URL"
    exit 1
fi

echo ""
echo "Check 2: Checking if One-Page Checkout is enabled..."
CART_PAGE=$(curl -s "$BASE_URL/cart")

if echo "$CART_PAGE" | grep -qi "onepagecheckout"; then
    echo -e "${GREEN}OK${NC} One-Page Checkout appears to be enabled"
elif echo "$CART_PAGE" | grep -qi "checkout/billingaddress"; then
    echo -e "${YELLOW}WARNING${NC} Traditional multi-page checkout detected"
    echo "The automated script targets one-page checkout endpoints."
fi

echo ""
echo "Check 3: Checking if guest checkout is allowed..."
CHECKOUT_HEADERS=$(curl -s -D - -o /dev/null "$BASE_URL/onepagecheckout")
CHECKOUT_STATUS=$(printf "%s" "$CHECKOUT_HEADERS" | sed -n 's|^HTTP/[0-9.]* \([0-9][0-9][0-9]\).*$|\1|p' | tail -n1)
REDIRECT_LOCATION=$(printf "%s" "$CHECKOUT_HEADERS" | grep -i '^Location:' | tail -n1 | sed 's/^[Ll]ocation:[[:space:]]*//' | tr -d '\r')

if [ "$CHECKOUT_STATUS" = "200" ]; then
    echo -e "${GREEN}OK${NC} Guest checkout appears to be allowed"
elif [ "$CHECKOUT_STATUS" = "302" ] && echo "$REDIRECT_LOCATION" | grep -qi "^/cart"; then
    echo -e "${GREEN}OK${NC} One-page checkout redirects to cart when empty (expected before items are added)"
elif [ "$CHECKOUT_STATUS" = "302" ] && echo "$REDIRECT_LOCATION" | grep -qi "login"; then
    echo -e "${YELLOW}WARNING${NC} Guest checkout may be disabled"
    echo "Automated ordering will fail until anonymous checkout is enabled."
else
    echo -e "${YELLOW}WARNING${NC} Unable to confirm guest checkout status (HTTP $CHECKOUT_STATUS)"
fi

echo ""
echo "Check 4: Checking if sample products exist..."
PRODUCTS_FOUND=0

PRODUCT_PATHS=(
    "/build-your-own-computer"
    "/lenovo-ideacentre"
    "/apple-macbook-pro"
    "/asus-laptop"
    "/samsung-premium-ultrabook"
    "/lenovo-thinkpad-carbon-laptop"
    "/nikon-d5500-dslr"
    "/samsung-galaxy-s24-256gb"
)

for PRODUCT_PATH in "${PRODUCT_PATHS[@]}"; do
    PRODUCT_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL$PRODUCT_PATH")
    if [ "$PRODUCT_STATUS" = "200" ] || [ "$PRODUCT_STATUS" = "301" ] || [ "$PRODUCT_STATUS" = "302" ]; then
        PRODUCTS_FOUND=$((PRODUCTS_FOUND + 1))
    fi
done

if [ "$PRODUCTS_FOUND" -ge 3 ]; then
    echo -e "${GREEN}OK${NC} Found accessible sample product pages used by the load test"
elif [ "$PRODUCTS_FOUND" -gt 0 ]; then
    echo -e "${YELLOW}WARNING${NC} Only found $PRODUCTS_FOUND matching products"
else
    echo -e "${RED}ERROR${NC} No expected sample product pages found"
    exit 1
fi

echo ""
echo "Check 5: Testing add-to-cart endpoint..."
ADD_TO_CART_STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X POST "$BASE_URL/addproducttocart/catalog/1/1")

if [ "$ADD_TO_CART_STATUS" = "200" ] || [ "$ADD_TO_CART_STATUS" = "400" ]; then
    echo -e "${GREEN}OK${NC} Add-to-cart endpoint is reachable"
elif [ "$ADD_TO_CART_STATUS" = "404" ]; then
    echo -e "${RED}ERROR${NC} Add-to-cart endpoint not found"
    exit 1
else
    echo -e "${YELLOW}WARNING${NC} Add-to-cart endpoint returned HTTP $ADD_TO_CART_STATUS"
fi

echo ""
echo "Configuration check complete"
echo ""
echo "Run with:"
echo "  cd $SCRIPT_DIR"
echo "  ./run-load-test.sh automated"
