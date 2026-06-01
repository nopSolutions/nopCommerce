#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BASE_URL="${BASE_URL:-http://localhost:8080}"

RED='\033[0;31m'
GREEN='\033[0;32m'
NC='\033[0m'

echo "======================================"
echo "nopCommerce Load Test Runner"
echo "======================================"
echo ""

if ! command -v k6 >/dev/null 2>&1; then
    echo -e "${RED}ERROR k6 is not installed${NC}"
    exit 1
fi

echo -e "${GREEN}OK k6 is installed${NC} ($(k6 version))"

if curl -s -o /dev/null -w "%{http_code}" "$BASE_URL" | grep -q "200"; then
    echo -e "${GREEN}OK nopCommerce accessible${NC} ($BASE_URL)"
else
    echo -e "${RED}ERROR nopCommerce not accessible${NC} ($BASE_URL)"
    echo "Start the application yourself, then rerun this script."
    exit 1
fi

if [ -z "$1" ]; then
    echo ""
    echo "Usage: ./run-load-test.sh [automated|simple|help]"
    exit 0
fi

case "$1" in
    automated)
        TEST_SCRIPT="automated-order-placement.js"
        if [ -f "$SCRIPT_DIR/verify-nopcommerce-config.sh" ]; then
            bash "$SCRIPT_DIR/verify-nopcommerce-config.sh"
        fi
        ;;
    simple)
        TEST_SCRIPT="simple-order-test.js"
        ;;
    help|-h|--help)
        echo "Usage: ./run-load-test.sh [automated|simple|help]"
        exit 0
        ;;
    *)
        echo -e "${RED}ERROR unknown option '$1'${NC}"
        exit 1
        ;;
esac

echo ""
echo "Running $TEST_SCRIPT against $BASE_URL"
echo ""

cd "$SCRIPT_DIR"
k6 run "$TEST_SCRIPT"
