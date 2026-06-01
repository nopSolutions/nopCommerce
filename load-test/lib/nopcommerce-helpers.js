export function extractAntiForgeryToken(response) {
    if (!response || !response.body) {
        console.error('extractAntiForgeryToken: Invalid response');
        return null;
    }

    try {
        const inputMatch = response.body.match(/<input[^>]*name="__RequestVerificationToken"[^>]*value="([^"]*)"/i);
        if (inputMatch && inputMatch[1]) {
            return inputMatch[1];
        }

        const altMatch = response.body.match(/value="([^"]*)"[^>]*name="__RequestVerificationToken"/i);
        if (altMatch && altMatch[1]) {
            return altMatch[1];
        }

        const metaMatch = response.body.match(/<meta[^>]*name="__RequestVerificationToken"[^>]*content="([^"]*)"/i);
        if (metaMatch && metaMatch[1]) {
            return metaMatch[1];
        }

        const scriptMatch = response.body.match(/__RequestVerificationToken['"]\s*:\s*['"]([^'"]+)['"]/);
        if (scriptMatch && scriptMatch[1]) {
            return scriptMatch[1];
        }

        console.warn('extractAntiForgeryToken: Token not found in response');
        return null;
    } catch (error) {
        console.error(`extractAntiForgeryToken: Parse error - ${error.message}`);
        return null;
    }
}

export function extractCookies(response) {
    const cookies = {};

    if (!response || !response.headers) {
        return cookies;
    }

    const setCookieHeaders = response.headers['Set-Cookie'] || response.headers['set-cookie'];
    if (!setCookieHeaders) {
        return cookies;
    }

    const cookieArray = Array.isArray(setCookieHeaders) ? setCookieHeaders : [setCookieHeaders];

    cookieArray.forEach((cookieStr) => {
        const parts = cookieStr.split(';')[0].split('=');
        if (parts.length >= 2) {
            const name = parts[0].trim();
            const value = parts.slice(1).join('=').trim();
            cookies[name] = value;
        }
    });

    return cookies;
}

export function mergeCookies(existingCookies, newCookies) {
    return Object.assign({}, existingCookies, newCookies);
}

export function buildCookieHeader(cookies) {
    return Object.entries(cookies)
        .map(([name, value]) => `${name}=${value}`)
        .join('; ');
}

export function buildAddToCartFormData(productId, quantity, token) {
    return {
        __RequestVerificationToken: token,
        product_id: productId.toString(),
        shoppingcarttype: '1',
        [`addtocart_${productId}.EnteredQuantity`]: quantity.toString(),
    };
}

export function buildBillingAddressFormData(token, vuNumber, iteration) {
    const timestamp = Date.now();
    const uniqueId = `${vuNumber}_${iteration}_${timestamp}`;

    return {
        __RequestVerificationToken: token,
        ShipToSameAddress: 'true',
        'BillingNewAddress.FirstName': 'LoadTest',
        'BillingNewAddress.LastName': `User${vuNumber}`,
        'BillingNewAddress.Email': `loadtest_${uniqueId}@test.local`,
        'BillingNewAddress.Company': 'Test Company',
        'BillingNewAddress.CountryId': '1',
        'BillingNewAddress.StateProvinceId': '',
        'BillingNewAddress.City': 'TestCity',
        'BillingNewAddress.Address1': `${vuNumber} Test Street`,
        'BillingNewAddress.Address2': '',
        'BillingNewAddress.ZipPostalCode': '12345',
        'BillingNewAddress.PhoneNumber': '555-0100',
        'BillingNewAddress.FaxNumber': '',
    };
}

export function buildShippingMethodFormData(token, shippingOption = 'Ground___Shipping.FixedByWeightByTotal') {
    return {
        __RequestVerificationToken: token,
        shippingoption: shippingOption,
    };
}

export function buildPaymentMethodFormData(token, paymentMethod = 'Payments.Manual') {
    return {
        __RequestVerificationToken: token,
        paymentmethod: paymentMethod,
    };
}

export function buildManualPaymentInfoFormData(token) {
    return {
        __RequestVerificationToken: token,
        CardholderName: 'Load Test User',
        CardNumber: '4111111111111111',
        ExpireMonth: '12',
        ExpireYear: '2028',
        CardCode: '123',
    };
}

export function buildCheckoutAttributeFormData(token, attributeId = 1, valueId = 1) {
    return {
        __RequestVerificationToken: token,
        [`checkout_attribute_${attributeId}`]: valueId.toString(),
    };
}

export function validateCheckoutStepResponse(response, stepName) {
    if (!response) {
        console.error(`${stepName}: No response received`);
        return false;
    }

    if (response.status !== 200 && response.status !== 302) {
        console.error(`${stepName}: HTTP ${response.status}`);
        return false;
    }

    const contentType = response.headers['Content-Type'] || response.headers['content-type'] || '';
    if (contentType.includes('application/json')) {
        try {
            const body = typeof response.body === 'string' ? JSON.parse(response.body) : response.body;

            if (body.message) {
                const message = Array.isArray(body.message) ? body.message.join(', ') : body.message;
                console.log(`${stepName}: Message - ${message}`);
            }

            if (body.error) {
                const errorDetail = body.message || body.error;
                console.error(`${stepName}: Error in response - ${errorDetail}`);
                return false;
            }

            if (body.success !== undefined && !body.success) {
                console.error(`${stepName}: Success=false in response`);
                return false;
            }

            if (body.Warnings && body.Warnings.length > 0) {
                console.error(`${stepName}: Warnings - ${body.Warnings.join(', ')}`);
                return false;
            }

            return true;
        } catch {
            return response.status === 200;
        }
    }

    if (response.body && typeof response.body === 'string') {
        const bodyLower = response.body.toLowerCase();
        if (bodyLower.includes('error') && bodyLower.includes('message-error')) {
            console.error(`${stepName}: Error message found in HTML`);
            return false;
        }

        if (bodyLower.includes('field-validation-error')) {
            console.error(`${stepName}: Validation error in form`);
            return false;
        }
    }

    return true;
}

export function extractOrderId(response) {
    if (!response || !response.body) {
        return null;
    }

    try {
        const contentType = response.headers['Content-Type'] || response.headers['content-type'] || '';
        if (contentType.includes('application/json')) {
            const body = typeof response.body === 'string' ? JSON.parse(response.body) : response.body;
            if (body.redirect && body.redirect.includes('orderId=')) {
                const match = body.redirect.match(/orderId=(\d+)/);
                if (match) {
                    return parseInt(match[1], 10);
                }
            }

            return null;
        }

        if (response.status === 302) {
            const location = response.headers.Location || response.headers.location;
            if (location && location.includes('orderId=')) {
                const match = location.match(/orderId=(\d+)/);
                if (match) {
                    return parseInt(match[1], 10);
                }
            }
        }

        const orderIdMatch = response.body.match(/orderId[=:](\d+)/i);
        if (orderIdMatch) {
            return parseInt(orderIdMatch[1], 10);
        }

        return null;
    } catch (error) {
        console.error(`extractOrderId: Error - ${error.message}`);
        return null;
    }
}

export function buildHeaders(cookies = {}, additionalHeaders = {}) {
    const headers = Object.assign({
        'User-Agent': 'k6-load-test/1.0',
        Accept: 'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',
        'Accept-Language': 'en-US,en;q=0.5',
        'Accept-Encoding': 'gzip, deflate',
        Connection: 'keep-alive',
        'Upgrade-Insecure-Requests': '1',
    }, additionalHeaders);

    if (Object.keys(cookies).length > 0) {
        headers.Cookie = buildCookieHeader(cookies);
    }

    return headers;
}

export function buildAjaxHeaders(cookies, token) {
    const headers = buildHeaders(cookies, {
        'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
        'X-Requested-With': 'XMLHttpRequest',
        Accept: 'application/json, text/javascript, */*; q=0.01',
    });

    if (token) {
        headers.RequestVerificationToken = token;
    }

    return headers;
}

export function extractCartItemIds(response) {
    if (!response || !response.body) {
        return [];
    }

    try {
        const body = typeof response.body === 'string' ? JSON.parse(response.body) : response.body;
        if (body.update_section && body.update_section.html) {
            const matches = body.update_section.html.matchAll(/itemquantity(\d+)/g);
            const itemIds = [];

            for (const match of matches) {
                if (match[1]) {
                    itemIds.push(match[1]);
                }
            }

            return itemIds;
        }

        return [];
    } catch (error) {
        console.error(`extractCartItemIds: Error - ${error.message}`);
        return [];
    }
}

export function logCheckoutProgress(vu, step, status, details = {}) {
    const timestamp = new Date().toISOString();
    const detailsStr = Object.keys(details).length > 0 ? ` | ${JSON.stringify(details)}` : '';
    console.log(`[${timestamp}] [VU ${vu}] ${step}: ${status}${detailsStr}`);
}

export function encodeFormData(formData) {
    return Object.entries(formData)
        .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
        .join('&');
}

export function humanDelay(baseSeconds, jitterPercent = 20) {
    const jitter = baseSeconds * (jitterPercent / 100);
    const min = Math.max(0, baseSeconds - jitter);
    const max = baseSeconds + jitter;
    return min + (Math.random() * (max - min));
}
