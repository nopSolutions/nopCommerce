#!/usr/bin/env python3
"""
Seed OpenBoxes with all nopCommerce sample products and initial stock.
Run this before load tests so no order fails due to out-of-stock.

Product list is read from:
  src/Presentation/Nop.Web/App_Data/Installation/SampleData.json

Auth and product-creation logic mirrors the VerdeMart.OpenBoxesBridge.

Usage (from repo root, while docker-compose stack is up):
    python3 scripts/init_openboxes_stock.py
    python3 scripts/init_openboxes_stock.py --url http://localhost:8080/openboxes --qty 50000
"""

import argparse
import datetime
import http.cookiejar
import json
import sys
import urllib.error
import urllib.parse
import urllib.request
from pathlib import Path

SAMPLE_DATA = (
    Path(__file__).resolve().parent.parent
    / "src/Presentation/Nop.Web/App_Data/Installation/SampleData.json"
)

DEFAULTS = dict(
    url="http://localhost:8080/openboxes/",
    username="admin",
    password="password",
    origin_location_id="1",
    category_name="DEFAULT_CATEGORY",
    product_type_id="DEFAULT",
    requested_by_id="1",
    quantity=10_000,
)


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> None:
    args = parse_args()
    base = args.url if args.url.endswith("/") else args.url + "/"

    products = load_nopcommerce_products()
    print(f"Loaded {len(products)} products from SampleData.json")

    client = OpenBoxesClient(
        base=base,
        username=args.username,
        password=args.password,
        origin_location_id=args.origin,
        category_name=args.category,
        product_type_id=DEFAULTS["product_type_id"],
        requested_by_id=DEFAULTS["requested_by_id"],
    )

    client.login()
    client.choose_location()
    category_id = client.ensure_category()

    ob_products: dict[str, str] = {}  # sku → openboxes id
    for p in products:
        sku: str = p.get("Sku", "").strip()
        name: str = p.get("Name", sku).strip()
        if not sku:
            print(f"  SKIP '{name}': no SKU")
            continue
        existing = client._find_product_by_code(sku)
        ob_id = existing or client._create_product(sku, name, category_id)
        ob_products[sku] = ob_id
        label = "found  " if existing else "created"
        print(f"  {label} {sku:20s} → {ob_id}  ({name})")

    if ob_products:
        client.seed_stock(ob_products, args.qty)

    print(f"\nDone — {len(ob_products)} products seeded with {args.qty:,} units each.")


def parse_args() -> argparse.Namespace:
    p = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    p.add_argument("--url",      default=DEFAULTS["url"],           help="OpenBoxes base URL")
    p.add_argument("--username", default=DEFAULTS["username"],      help="OpenBoxes username")
    p.add_argument("--password", default=DEFAULTS["password"],      help="OpenBoxes password")
    p.add_argument("--origin",   default=DEFAULTS["origin_location_id"], help="Warehouse location ID")
    p.add_argument("--category", default=DEFAULTS["category_name"], help="Category name for auto-created products")
    p.add_argument("--qty",      default=DEFAULTS["quantity"], type=int, help="Units to stock per product")
    return p.parse_args()


# ---------------------------------------------------------------------------
# nopCommerce product loader
# ---------------------------------------------------------------------------

def load_nopcommerce_products() -> list[dict]:
    if not SAMPLE_DATA.exists():
        sys.exit(f"SampleData.json not found at {SAMPLE_DATA}")
    data = json.loads(SAMPLE_DATA.read_text(encoding="utf-8-sig"))
    section = data.get("Products", {})
    if isinstance(section, dict):
        return section.get("Products", [])
    return section  # already a list


# ---------------------------------------------------------------------------
# OpenBoxes HTTP client
# ---------------------------------------------------------------------------

class OpenBoxesClient:
    def __init__(
        self,
        base: str,
        username: str,
        password: str,
        origin_location_id: str,
        category_name: str,
        product_type_id: str,
        requested_by_id: str,
    ) -> None:
        self.base = base
        self.username = username
        self.password = password
        self.origin_location_id = origin_location_id
        self.category_name = category_name
        self.product_type_id = product_type_id
        self.requested_by_id = requested_by_id

        jar = http.cookiejar.CookieJar()
        self._opener = urllib.request.build_opener(
            urllib.request.HTTPCookieProcessor(jar),
            _NoRedirectHandler(),
        )
        self._opener.addheaders = [("User-Agent", "VerdeMart-InitScript/1.0")]
        self._category_id: str | None = None

    # ------------------------------------------------------------------
    # Session setup  (mirrors OpenBoxesClient.cs LoginAsync / ChooseLocationAsync)
    # ------------------------------------------------------------------

    def login(self) -> None:
        url = self.base + "auth/handleLogin"
        body = urllib.parse.urlencode({
            "username": self.username,
            "password": self.password,
            "targetUri": "",
        }).encode()
        req = urllib.request.Request(
            url, data=body,
            headers={"Content-Type": "application/x-www-form-urlencoded"},
        )
        try:
            resp = self._opener.open(req)
            raise RuntimeError(
                f"Login returned HTTP {resp.status} — credentials likely rejected "
                "(expected a redirect, got a page body)"
            )
        except urllib.error.HTTPError as e:
            location = e.headers.get("Location", "")
            bad = any(x in location for x in ("authfail", "/auth/login", "/auth/handleLogin"))
            if e.code in (302, 303) and not bad:
                print(f"Logged in as '{self.username}'")
                return
            raise RuntimeError(f"Login failed: HTTP {e.code} → {location!r}") from e

    def choose_location(self) -> None:
        url = self.base + f"dashboard/chooseLocation?id={self.origin_location_id}"
        try:
            self._opener.open(url)
        except urllib.error.HTTPError as e:
            if e.code in (200, 302, 303):
                print(f"Location {self.origin_location_id} chosen")
                return
            raise RuntimeError(f"chooseLocation failed: HTTP {e.code}") from e
        print(f"Location {self.origin_location_id} chosen")

    # ------------------------------------------------------------------
    # Category  (mirrors EnsureCategoryIdAsync)
    # ------------------------------------------------------------------

    def ensure_category(self) -> str:
        if self._category_id:
            return self._category_id

        status, body = self._get("api/categories")
        if status == 200:
            cid = _find_by_name(body, self.category_name)
            if cid:
                self._category_id = cid
                print(f"Category '{self.category_name}' found: {cid}")
                return cid

        status, body = self._post("api/categories", {"name": self.category_name, "isRoot": False})
        if status not in (200, 201):
            raise RuntimeError(f"Category create failed: HTTP {status} — {body[:200]}")

        cid = _extract_id(body, wrappers=("category", "data"))
        if not cid:
            raise RuntimeError(f"Category create returned no id. Body: {body[:200]}")

        self._category_id = cid
        print(f"Category '{self.category_name}' created: {cid}")
        return cid

    # ------------------------------------------------------------------
    # Products  (mirrors EnsureProductAsync / FindProductIdByCodeAsync / CreateProductAsync)
    # ------------------------------------------------------------------

    def ensure_product(self, sku: str, name: str, category_id: str) -> str:
        existing = self._find_product_by_code(sku)
        if existing:
            return existing
        return self._create_product(sku, name, category_id)

    def _find_product_by_code(self, sku: str) -> str | None:
        url = "api/products?productCode=" + urllib.parse.quote(sku)
        status, body = self._get(url)
        if status != 200:
            return None
        return _find_by_product_code(body, sku)

    def _create_product(self, sku: str, name: str, category_id: str) -> str:
        payload = {
            "productCode": sku,
            "name": name or sku,
            "productType": {"id": self.product_type_id},
            "category": {"id": category_id},
        }
        status, body = self._post("api/products", payload)

        if status == 409:
            existing = self._find_product_by_code(sku)
            if existing:
                return existing
            raise RuntimeError(f"Product {sku} conflict but could not re-resolve")

        if status not in (200, 201):
            raise RuntimeError(f"Product create failed for {sku}: HTTP {status} — {body[:200]}")

        pid = _extract_id(body, wrappers=("product", "data"))
        if not pid:
            raise RuntimeError(f"Product create returned no id for {sku}. Body: {body[:200]}")
        return pid

    # ------------------------------------------------------------------
    # Stock seeding via PRODUCT_INVENTORY transactions
    # ------------------------------------------------------------------

    def seed_stock(self, product_map: dict[str, str], quantity: int) -> None:
        """
        Set physical quantity-on-hand for every product by creating a single
        PRODUCT_INVENTORY (Inventory Baseline, type id=12) transaction at the
        warehouse. Each product gets one transactionEntry with the desired qty.

        This is the same mechanism as the OpenBoxes "Adjust Inventory" UI action
        and is the only API path that actually updates the Qty column.
        """
        print(f"\nSeeding stock ({quantity:,} units per product)…")

        # 1. Ensure every product has an inventoryItem record
        entries = []
        for sku, product_id in product_map.items():
            inv_item_id = self._ensure_inventory_item(product_id, sku)
            entries.append({
                "product":       {"id": product_id},
                "inventoryItem": {"id": inv_item_id},
                "quantity":      quantity,
            })

        # 2. Post a single PRODUCT_INVENTORY transaction covering all products
        now = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
        txn_payload = {
            "transactionDate":   now,
            "transactionType":   {"id": "12"},           # Inventory Baseline
            "inventory":         {"id": self.origin_location_id},
            "transactionEntries": entries,
        }
        status, body = self._post("api/generic/transaction", txn_payload)
        if status not in (200, 201):
            raise RuntimeError(
                f"PRODUCT_INVENTORY transaction failed: HTTP {status} — {body[:400]}"
            )

        doc = json.loads(body)
        txn = doc.get("data", doc)
        print(f"  Transaction created: {txn.get('id')}  ({len(entries)} products × {quantity:,} units)")

    def _ensure_inventory_item(self, product_id: str, sku: str) -> str:
        """
        Always POST a new inventoryItem for this product.

        The GET filter (product.id=) does not work in this OpenBoxes version and
        returns unrelated items. Creating a fresh one per product is safe: OpenBoxes
        allows multiple inventoryItems per product (lot tracking) and the new item
        is correctly associated with the right product id.
        """
        status, body = self._post(
            "api/generic/inventoryItem",
            {"product": {"id": product_id}},
        )
        if status not in (200, 201):
            raise RuntimeError(
                f"inventoryItem create failed for {sku}: HTTP {status} — {body[:200]}"
            )
        doc = json.loads(body)
        item = doc.get("data", doc)
        iid = item.get("id")
        if not iid:
            raise RuntimeError(f"inventoryItem create returned no id for {sku}. Body: {body[:200]}")
        return iid

    # ------------------------------------------------------------------
    # HTTP helpers
    # ------------------------------------------------------------------

    def _get(self, path: str) -> tuple[int, str]:
        url = self.base + path
        try:
            resp = self._opener.open(urllib.request.Request(url))
            return resp.status, resp.read().decode()
        except urllib.error.HTTPError as e:
            return e.code, e.read().decode()

    def _post(self, path: str, payload: object) -> tuple[int, str]:
        url = self.base + path
        data = json.dumps(payload).encode()
        req = urllib.request.Request(url, data=data, headers={"Content-Type": "application/json"})
        try:
            resp = self._opener.open(req)
            return resp.status, resp.read().decode()
        except urllib.error.HTTPError as e:
            return e.code, e.read().decode()


# ---------------------------------------------------------------------------
# No-redirect handler (mirrors AllowAutoRedirect=false in the bridge)
# ---------------------------------------------------------------------------

class _NoRedirectHandler(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        return None


# ---------------------------------------------------------------------------
# JSON parsing helpers  (mirrors TryRead* methods in OpenBoxesClient.cs)
# ---------------------------------------------------------------------------

def _extract_id(body: str, wrappers: tuple[str, ...] = ()) -> str | None:
    try:
        doc = json.loads(body)
    except json.JSONDecodeError:
        return None
    for key in wrappers:
        if isinstance(doc, dict) and key in doc:
            doc = doc[key]
            break
    if isinstance(doc, dict):
        return doc.get("id")
    return None


def _extract_location_id(body: str) -> str | None:
    try:
        doc = json.loads(body)
    except json.JSONDecodeError:
        return None
    items = doc.get("data", doc) if isinstance(doc, dict) else doc
    if isinstance(items, list):
        return items[0].get("id") if items else None
    if isinstance(items, dict):
        return items.get("id")
    return None


def _find_by_name(body: str, name: str) -> str | None:
    try:
        doc = json.loads(body)
    except json.JSONDecodeError:
        return None
    items = doc.get("data", doc) if isinstance(doc, dict) else doc
    if not isinstance(items, list):
        return None
    for item in items:
        if item.get("name", "").strip().lower() == name.strip().lower():
            return item.get("id")
    return None


def _find_by_product_code(body: str, sku: str) -> str | None:
    try:
        doc = json.loads(body)
    except json.JSONDecodeError:
        return None
    root = doc.get("data", doc) if isinstance(doc, dict) else doc
    if isinstance(root, list):
        for item in root:
            if item.get("productCode", "").lower() == sku.lower():
                return item.get("id")
    elif isinstance(root, dict) and root.get("productCode", "").lower() == sku.lower():
        return root.get("id")
    return None


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        sys.exit(0)
    except Exception as exc:
        print(f"\nFatal: {exc}", file=sys.stderr)
        sys.exit(1)
