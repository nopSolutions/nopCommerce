document.addEventListener('DOMContentLoaded', () => {
    const root = document.getElementById('aliexpress-config-root');
    if (!root) {
        return;
    }

    const authCodeInput = document.getElementById('authCode');
    const finalPriceEl = document.getElementById('simFinalPrice');
    const profitEl = document.getElementById('simProfit');
    const roiEl = document.getElementById('simRoi');
    const healthEl = document.getElementById('simHealth');
    const exchangeAuthUrl = root.dataset.exchangeAuthUrl;
    const refreshTokenUrl = root.dataset.refreshTokenUrl;
    const antiForgeryField = document.querySelector('input[name="__RequestVerificationToken"]');
    const inputs = [
        'simProductPrice',
        'simFreight',
        'simShipping',
        'simCustoms',
        'simVat',
        'simMargin'
    ].map((id) => document.getElementById(id)).filter(Boolean);

    window.exchangeAuthCode = function () {
        const authCode = authCodeInput ? authCodeInput.value : '';
        if (!authCode) {
            alert('Please enter an authorization code');
            return;
        }
        if (!antiForgeryField || !exchangeAuthUrl) {
            return;
        }
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = exchangeAuthUrl;
        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'authCode';
        input.value = authCode;
        form.appendChild(input);
        const tokenClone = antiForgeryField.cloneNode();
        tokenClone.value = antiForgeryField.value;
        form.appendChild(tokenClone);
        document.body.appendChild(form);
        form.submit();
    };

    window.refreshToken = function () {
        if (!confirm('Are you sure you want to refresh the access token now?')) {
            return;
        }
        if (!antiForgeryField || !refreshTokenUrl) {
            return;
        }
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = refreshTokenUrl;
        const tokenClone = antiForgeryField.cloneNode();
        tokenClone.value = antiForgeryField.value;
        form.appendChild(tokenClone);
        document.body.appendChild(form);
        form.submit();
    };

    if (!window.Highcharts || !finalPriceEl || !profitEl || !roiEl || !healthEl) {
        return;
    }

    const chart = Highcharts.chart('priceSimulatorChart', {
        chart: { type: 'column' },
        title: { text: 'Unit Economics Snapshot', style: { fontSize: '14px' } },
        xAxis: { categories: ['Costs', 'Taxes', 'Profit'] },
        yAxis: { title: { text: 'Amount (R)' } },
        legend: { enabled: false },
        series: [{ name: 'Value', data: [0, 0, 0], colorByPoint: true }],
        colors: ['#94a3b8', '#38bdf8', '#22c55e'],
        credits: { enabled: false }
    });

    const formatCurrency = (value) => `R${value.toFixed(2)}`;
    const evaluateHealth = (roi) => {
        if (roi >= 0.5) return 'Excellent';
        if (roi >= 0.35) return 'Optimal';
        if (roi >= 0.2) return 'Monitor';
        return 'Needs Attention';
    };

    const recalc = () => {
        const productPrice = parseFloat(inputs[0]?.value) || 0;
        const freight = parseFloat(inputs[1]?.value) || 0;
        const shipping = parseFloat(inputs[2]?.value) || 0;
        const customsRate = (parseFloat(inputs[3]?.value) || 0) / 100;
        const vatRate = (parseFloat(inputs[4]?.value) || 0) / 100;
        const marginRate = (parseFloat(inputs[5]?.value) || 0) / 100;

        const landedCost = productPrice + freight + shipping;
        const customsDuty = landedCost * customsRate;
        const vat = (landedCost + customsDuty) * vatRate;
        const preMargin = landedCost + customsDuty + vat;
        const finalPrice = preMargin * (1 + marginRate);
        const profit = finalPrice - preMargin;
        const roi = landedCost > 0 ? profit / landedCost : 0;

        finalPriceEl.textContent = formatCurrency(finalPrice);
        profitEl.textContent = formatCurrency(profit);
        roiEl.textContent = `${(roi * 100).toFixed(1)}%`;
        healthEl.textContent = evaluateHealth(roi);

        chart.series[0].setData([
            Number(landedCost.toFixed(2)),
            Number((customsDuty + vat).toFixed(2)),
            Number(profit.toFixed(2))
        ]);
    };

    inputs.forEach((input) => input?.addEventListener('input', recalc));
    recalc();
});
