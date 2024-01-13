using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Tax;

namespace Nop.Tests.Nop.Services.Tests.Tax;

public class FixedRateTestTaxProvider : BasePlugin, ITaxProvider
{
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly IPaymentService _paymentService;
    private readonly ITaxService _taxService;
    private readonly TaxSettings _taxSettings;

    public FixedRateTestTaxProvider(IGenericAttributeService genericAttributeService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentService paymentService,
        ITaxService taxService,
        TaxSettings taxSettings
    )
    {
        _genericAttributeService = genericAttributeService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _paymentService = paymentService;
        _taxService = taxService;
        _taxSettings = taxSettings;
    }

    /// <summary>
    /// Gets tax rate
    /// </summary>
    /// <param name="taxRateRequest">Tax rate request</param>
    /// <returns>Tax</returns>
    public Task<TaxRateResult> GetTaxRateAsync(TaxRateRequest taxRateRequest)
    {
        return Task.FromResult(new TaxRateResult { TaxRate = 10 });
    }

    /// <summary>
    /// Gets tax total
    /// </summary>
    /// <param name="taxTotalRequest">Tax total request</param>
    /// <returns>Tax total</returns>
    public async Task<TaxTotalResult> GetTaxTotalAsync(TaxTotalRequest taxTotalRequest)
    {
        var taxRates = new SortedDictionary<decimal, decimal>();
        var cart = taxTotalRequest.ShoppingCart;
        var customer = taxTotalRequest.Customer;
        var storeId = taxTotalRequest.StoreId;
        var usePaymentMethodAdditionalFee = taxTotalRequest.UsePaymentMethodAdditionalFee;

        var paymentMethodSystemName = string.Empty;
        if (customer != null)
            paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, storeId);

        //order sub total (items + checkout attributes)
        var subTotalTaxTotal = decimal.Zero;
        var (_, _, _, _, orderSubTotalTaxRates) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, false);
        foreach (var kvp in orderSubTotalTaxRates)
        {
            var taxRate = kvp.Key;
            var taxValue = kvp.Value;
            subTotalTaxTotal += taxValue;

            if (taxRate <= decimal.Zero || taxValue <= decimal.Zero)
                continue;

            if (!taxRates.TryGetValue(taxRate, out var value))
                taxRates.Add(taxRate, taxValue);
            else
                taxRates[taxRate] = value + taxValue;
        }

        //shipping
        var shippingTax = decimal.Zero;
        if (_taxSettings.ShippingIsTaxable)
        {
            var (shippingExclTax, _, _) = await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart, false);
            var (shippingInclTax, taxRate, _) = await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart, true);
            if (shippingExclTax.HasValue && shippingInclTax.HasValue)
            {
                shippingTax = shippingInclTax.Value - shippingExclTax.Value;
                //ensure that tax is equal or greater than zero
                if (shippingTax < decimal.Zero)
                    shippingTax = decimal.Zero;

                //tax rates
                if (taxRate > decimal.Zero && shippingTax > decimal.Zero)
                {
                    if (!taxRates.TryGetValue(taxRate, out var value))
                        taxRates.Add(taxRate, shippingTax);
                    else
                        taxRates[taxRate] = value + shippingTax;
                }
            }
        }

        //payment method additional fee
        var paymentMethodAdditionalFeeTax = decimal.Zero;
        if (usePaymentMethodAdditionalFee && _taxSettings.PaymentMethodAdditionalFeeIsTaxable)
        {
            var paymentMethodAdditionalFee = await _paymentService.GetAdditionalHandlingFeeAsync(cart, paymentMethodSystemName);
            var (paymentMethodAdditionalFeeExclTax, _) = await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, false, customer);
            var (paymentMethodAdditionalFeeInclTax, taxRate) = await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, true, customer);

            paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax - paymentMethodAdditionalFeeExclTax;
            //ensure that tax is equal or greater than zero
            if (paymentMethodAdditionalFeeTax < decimal.Zero)
                paymentMethodAdditionalFeeTax = decimal.Zero;

            //tax rates
            if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
            {
                if (!taxRates.TryGetValue(taxRate, out var value))
                    taxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                else
                    taxRates[taxRate] = value + paymentMethodAdditionalFeeTax;
            }
        }

        //add at least one tax rate (0%)
        if (!taxRates.Any())
            taxRates.Add(decimal.Zero, decimal.Zero);

        //summarize taxes
        var taxTotal = subTotalTaxTotal + shippingTax + paymentMethodAdditionalFeeTax;
        //ensure that tax is equal or greater than zero
        if (taxTotal < decimal.Zero)
            taxTotal = decimal.Zero;

        return new TaxTotalResult { TaxTotal = taxTotal, TaxRates = taxRates };
    }
}