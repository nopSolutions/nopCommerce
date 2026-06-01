import http from 'k6/http';
import { check } from 'k6';
import { Counter, Rate, Trend } from 'k6/metrics';
import {
    buildAddToCartFormData,
    buildAjaxHeaders,
    buildBillingAddressFormData,
    buildCheckoutAttributeFormData,
    buildHeaders,
    buildManualPaymentInfoFormData,
    buildPaymentMethodFormData,
    buildShippingMethodFormData,
    encodeFormData,
    extractAntiForgeryToken,
    extractCookies,
    extractOrderId,
    extractCartItemIds,
    logCheckoutProgress,
    mergeCookies,
    validateCheckoutStepResponse,
} from './lib/nopcommerce-helpers.js';

const orderSuccessRate = new Rate('order_success_rate');
const orderPlacementDuration = new Trend('order_placement_duration_ms');
const orderFailures = new Counter('order_failures');
const checkoutStepFailures = new Counter('checkout_step_failures');

const ORDER_TARGET = Number(__ENV.ORDER_TARGET || 0);
const REQUESTED_FIXED_VUS = Number(__ENV.FIXED_VUS || 5);
const FIXED_VUS = ORDER_TARGET > 0
    ? Math.max(1, Math.min(REQUESTED_FIXED_VUS, ORDER_TARGET))
    : REQUESTED_FIXED_VUS;

const scenario = ORDER_TARGET > 0
    ? {
        executor: 'shared-iterations',
        vus: FIXED_VUS,
        iterations: ORDER_TARGET,
        maxDuration: __ENV.MAX_DURATION || '30m',
    }
    : {
        executor: 'ramping-vus',
        startVUs: 0,
        stages: [
            { duration: __ENV.WARMUP_DURATION || '2m', target: 2 },
            { duration: __ENV.RAMP_DURATION || '3m', target: 5 },
            { duration: __ENV.SUSTAIN_DURATION || '50m', target: 5 },
            { duration: __ENV.COOLDOWN_DURATION || '5m', target: 0 },
        ],
        gracefulRampDown: '30s',
    };

export const options = {
    scenarios: {
        automated_order_flow: scenario,
    },
    thresholds: {
        order_success_rate: ['rate>0.90'],
        order_placement_duration_ms: ['p(95)<15000'],
        http_req_duration: ['p(95)<10000'],
        http_req_failed: ['rate<0.15'],
        checks: ['rate>0.85'],
    },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:8080';

const PRODUCT_URLS = {
    3: '/lenovo-ideacentre',
    5: '/asus-laptop',
    6: '/samsung-premium-ultrabook',
    9: '/lenovo-thinkpad-carbon-laptop',
    22: '/samsung-galaxy-s24-256gb',
};

const PRODUCT_IDS = [3, 5, 6, 9, 22];

const PAYMENT_METHODS = [
    'Payments.CheckMoneyOrder',
    'Payments.Manual',
];

const SHIPPING_OPTIONS = [
    'Ground___Shipping.FixedByWeightByTotal',
    '2nd Day Air___Shipping.FixedByWeightByTotal',
    'Next Day Air___Shipping.FixedByWeightByTotal',
];

export default function () {
    const vu = __VU;
    const iter = __ITER;
    let cookies = {};
    const startTime = Date.now();

    const productId = PRODUCT_IDS[Math.floor(Math.random() * PRODUCT_IDS.length)];
    const paymentMethod = PAYMENT_METHODS[Math.floor(Math.random() * PAYMENT_METHODS.length)];
    const shippingOption = SHIPPING_OPTIONS[Math.floor(Math.random() * SHIPPING_OPTIONS.length)];

    logCheckoutProgress(vu, 'START', 'Initiating order flow', {
        productId,
        paymentMethod: paymentMethod.replace('Payments.', ''),
    });

    let response = http.get(`${BASE_URL}/`, {
        headers: buildHeaders(),
        tags: { name: 'GET /' },
    });

    const homeCheck = check(response, {
        'Homepage loaded': (r) => r.status === 200,
    });

    if (!homeCheck) {
        logCheckoutProgress(vu, 'Homepage', 'FAIL', { status: response.status });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    cookies = mergeCookies(cookies, extractCookies(response));
    logCheckoutProgress(vu, 'Homepage', 'OK', { cookies: Object.keys(cookies).length });

    response = http.get(`${BASE_URL}${PRODUCT_URLS[productId]}`, {
        headers: buildHeaders(cookies),
        tags: { name: `GET ${PRODUCT_URLS[productId]}` },
    });

    const productCheck = check(response, {
        'Product page loaded': (r) => r.status === 200,
    });

    if (!productCheck) {
        logCheckoutProgress(vu, 'Product Page', 'FAIL', { status: response.status });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    cookies = mergeCookies(cookies, extractCookies(response));
    let token = extractAntiForgeryToken(response);

    if (!token) {
        logCheckoutProgress(vu, 'Product Page', 'FAIL', { error: 'No anti-forgery token found' });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    logCheckoutProgress(vu, 'Product Page', 'OK', { tokenLength: token.length });

    const addToCartData = buildAddToCartFormData(productId, 1, token);

    response = http.post(
        `${BASE_URL}/addproducttocart/details/${productId}/1`,
        encodeFormData(addToCartData),
        {
            headers: buildAjaxHeaders(cookies, token),
            tags: { name: 'POST /addproducttocart' },
        }
    );

    const cartCheck = check(response, {
        'Product added to cart': (r) => {
            if (r.status !== 200) {
                return false;
            }

            try {
                const body = typeof r.body === 'string' ? JSON.parse(r.body) : r.body;
                return body.success === true || body.message;
            } catch {
                return r.status === 200;
            }
        },
    });

    if (!cartCheck) {
        logCheckoutProgress(vu, 'Add to Cart', 'FAIL', { status: response.status });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    cookies = mergeCookies(cookies, extractCookies(response));
    logCheckoutProgress(vu, 'Add to Cart', 'OK', { productId });

    const checkoutAttributeData = buildCheckoutAttributeFormData(token, 1, 1);

    response = http.post(
        `${BASE_URL}/shoppingcart/CheckoutAttributeChange?isEditable=true`,
        encodeFormData(checkoutAttributeData),
        {
            headers: buildAjaxHeaders(cookies, token),
            tags: { name: 'POST /shoppingcart/CheckoutAttributeChange' },
        }
    );

    const attributeCheck = check(response, {
        'Checkout attributes saved': (r) => r.status === 200,
    });

    if (!attributeCheck) {
        logCheckoutProgress(vu, 'Checkout Attributes', 'FAIL', { status: response.status });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    cookies = mergeCookies(cookies, extractCookies(response));
    logCheckoutProgress(vu, 'Checkout Attributes', 'OK', { giftWrapping: 'No' });

    response = http.get(`${BASE_URL}/onepagecheckout`, {
        headers: buildHeaders(cookies),
        tags: { name: 'GET /onepagecheckout' },
    });

    if (response.status === 302) {
        const location = response.headers.Location || response.headers.location || '';
        if (location.includes('login') || location.includes('customer/info')) {
            logCheckoutProgress(vu, 'Checkout Page', 'WARN', {
                error: 'Redirected to login - guest checkout may be disabled',
            });
            checkoutStepFailures.add(1);
            orderFailures.add(1);
            return;
        }
    }

    const checkoutCheck = check(response, {
        'Checkout page loaded': (r) => {
            if (r.status !== 200 && r.status !== 302) {
                return false;
            }

            const redirectedUrl = r.url || '';
            if (redirectedUrl.includes('/cart')) {
                return false;
            }

            return redirectedUrl.includes('/onepagecheckout')
                || (typeof r.body === 'string' && r.body.toLowerCase().includes('checkout-billing-load'));
        },
    });

    if (!checkoutCheck) {
        logCheckoutProgress(vu, 'Checkout Page', 'FAIL', {
            status: response.status,
            url: response.url || 'unknown',
        });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    cookies = mergeCookies(cookies, extractCookies(response));
    token = extractAntiForgeryToken(response) || token;
    logCheckoutProgress(vu, 'Checkout Page', 'OK');

    const billingData = buildBillingAddressFormData(token, vu, iter);

    response = http.post(
        `${BASE_URL}/checkout/OpcSaveBilling`,
        encodeFormData(billingData),
        {
            headers: buildAjaxHeaders(cookies, token),
            tags: { name: 'POST /checkout/OpcSaveBilling' },
        }
    );

    const billingCheck = validateCheckoutStepResponse(response, 'Billing Address');
    if (!billingCheck) {
        logCheckoutProgress(vu, 'Billing Address', 'FAIL', { status: response.status });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    cookies = mergeCookies(cookies, extractCookies(response));
    logCheckoutProgress(vu, 'Billing Address', 'OK', { shipToSameAddress: true });

    const shippingMethodData = buildShippingMethodFormData(token, shippingOption);

    response = http.post(
        `${BASE_URL}/checkout/OpcSaveShippingMethod`,
        encodeFormData(shippingMethodData),
        {
            headers: buildAjaxHeaders(cookies, token),
            tags: { name: 'POST /checkout/OpcSaveShippingMethod' },
        }
    );

    const shippingMethodCheck = validateCheckoutStepResponse(response, 'Shipping Method');
    if (!shippingMethodCheck) {
        logCheckoutProgress(vu, 'Shipping Method', 'FAIL', { status: response.status });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    cookies = mergeCookies(cookies, extractCookies(response));
    logCheckoutProgress(vu, 'Shipping Method', 'OK', { option: shippingOption.split('___')[1] });

    const paymentMethodData = buildPaymentMethodFormData(token, paymentMethod);

    response = http.post(
        `${BASE_URL}/checkout/OpcSavePaymentMethod`,
        encodeFormData(paymentMethodData),
        {
            headers: buildAjaxHeaders(cookies, token),
            tags: { name: 'POST /checkout/OpcSavePaymentMethod' },
        }
    );

    const paymentMethodCheck = validateCheckoutStepResponse(response, 'Payment Method');
    if (!paymentMethodCheck) {
        logCheckoutProgress(vu, 'Payment Method', 'FAIL', { status: response.status });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    cookies = mergeCookies(cookies, extractCookies(response));
    logCheckoutProgress(vu, 'Payment Method', 'OK', { method: paymentMethod.replace('Payments.', '') });

    const paymentInfoData = paymentMethod === 'Payments.Manual'
        ? buildManualPaymentInfoFormData(token)
        : { __RequestVerificationToken: token };

    response = http.post(
        `${BASE_URL}/checkout/OpcSavePaymentInfo`,
        encodeFormData(paymentInfoData),
        {
            headers: buildAjaxHeaders(cookies, token),
            tags: { name: 'POST /checkout/OpcSavePaymentInfo' },
        }
    );

    const paymentInfoCheck = validateCheckoutStepResponse(response, 'Payment Info');
    if (!paymentInfoCheck) {
        logCheckoutProgress(vu, 'Payment Info', 'FAIL', { status: response.status });
        checkoutStepFailures.add(1);
        orderFailures.add(1);
        return;
    }

    cookies = mergeCookies(cookies, extractCookies(response));
    const cartItemIds = extractCartItemIds(response);
    logCheckoutProgress(vu, 'Payment Info', 'OK', { cartItems: cartItemIds.length });

    const orderStartTime = Date.now();

    response = http.post(
        `${BASE_URL}/checkout/OpcConfirmOrder`,
        encodeFormData({
            __RequestVerificationToken: token,
            checkout_attribute_1: '1',
        }),
        {
            headers: buildAjaxHeaders(cookies, token),
            tags: {
                name: 'POST /checkout/OpcConfirmOrder',
                payment_method: paymentMethod,
                critical: 'true',
            },
            timeout: '30s',
        }
    );

    const orderDurationMs = Date.now() - orderStartTime;

    const orderCheck = check(response, {
        'Order API responded': (r) => r.status !== 0,
        'Order placement successful': (r) => {
            if (r.status !== 200) {
                return false;
            }

            try {
                const body = typeof r.body === 'string' ? JSON.parse(r.body) : r.body;
                return body.success === 1 || body.success === true;
            } catch {
                return false;
            }
        },
    });

    if (orderCheck) {
        const orderId = extractOrderId(response);
        const totalDurationMs = Date.now() - startTime;

        logCheckoutProgress(vu, 'ORDER PLACED', 'SUCCESS', {
            orderId: orderId || 'unknown',
            orderDuration: `${orderDurationMs}ms`,
            totalDuration: `${totalDurationMs}ms`,
            paymentMethod: paymentMethod.replace('Payments.', ''),
        });

        orderSuccessRate.add(true);
        orderPlacementDuration.add(totalDurationMs);
    } else {
        logCheckoutProgress(vu, 'ORDER PLACEMENT', 'FAIL', {
            status: response.status,
            orderDuration: `${orderDurationMs}ms`,
            body: response.body ? response.body.substring(0, 200) : 'empty',
        });

        orderSuccessRate.add(false);
        orderFailures.add(1);
    }
}

export function setup() {
    console.log('========================================');
    console.log(' AUTOMATED nopCommerce Order Placement  ');
    console.log('========================================');
    console.log(`Target: ${BASE_URL}`);
    console.log(`Products: ${PRODUCT_IDS.join(', ')}`);
    console.log(`Payment Methods: ${PAYMENT_METHODS.map((m) => m.replace('Payments.', '')).join(', ')}`);
    console.log('');
    if (ORDER_TARGET > 0) {
        console.log(`Fixed order target mode: ${ORDER_TARGET} iterations across ${FIXED_VUS} VUs`);
        console.log(`Max duration: ${__ENV.MAX_DURATION || '30m'}`);
    } else {
        console.log('Duration mode: ramping VUs over the configured stages');
    }
    console.log('');
    console.log('This script places real guest-checkout orders through nopCommerce.');
    console.log('If checkout is not configured, run ./verify-nopcommerce-config.sh first.');
    console.log('========================================');
    console.log('');
}

export function teardown() {
    console.log('');
    console.log('Load test complete');
}
