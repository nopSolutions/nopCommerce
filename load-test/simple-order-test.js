import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const successRate = new Rate('success_rate');
const BASE_URL = __ENV.BASE_URL || 'http://localhost:8080';

export const options = {
    scenarios: {
        constant_load: {
            executor: 'constant-vus',
            vus: Number(__ENV.VUS || 10),
            duration: __ENV.DURATION || '30m',
        },
    },
};

export default function () {
    let response = http.get(`${BASE_URL}/`, {
        tags: { name: 'Homepage' },
    });
    successRate.add(response.status === 200);
    check(response, { 'Homepage responds': (r) => r.status === 200 });
    sleep(1);

    response = http.get(`${BASE_URL}/`, {
        tags: { name: 'Browse' },
    });
    successRate.add(response.status === 200);
    sleep(1);

    http.get(`${BASE_URL}/health`, {
        tags: { name: 'Health Check' },
    });
    sleep(2);
}

export function setup() {
    console.log('======================================');
    console.log('Simple Load Test for nopCommerce');
    console.log('======================================');
    console.log(`Hitting: ${BASE_URL}`);
    console.log('Generating HTTP traffic only');
    console.log('======================================');
    console.log('');
}

export function teardown() {
    console.log('');
    console.log('Test complete');
}
