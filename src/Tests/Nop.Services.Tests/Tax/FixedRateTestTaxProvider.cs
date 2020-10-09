using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Tax;

namespace Nop.Services.Tests.Tax
{
    public class FixedRateTestTaxProvider : BasePlugin, ITaxProvider
    {
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>Tax</returns>
        public TaxRateResult GetTaxRate(TaxRateRequest taxRateRequest)
        {
            return new TaxRateResult { TaxRate = 10 };
        }

        /// <summary>
        /// Gets tax total
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>Tax total</returns>
        public TaxTotalResult GetTaxTotal(TaxTotalRequest taxTotalRequest)
        {
            var taxRates = new SortedDictionary<decimal, decimal>();
            var cart = taxTotalRequest.ShoppingCart;
            var customer = taxTotalRequest.Customer;
            var storeId = taxTotalRequest.StoreId;
            var usePaymentMethodAdditionalFee = taxTotalRequest.UsePaymentMethodAdditionalFee;
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            var orderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
            var paymentService = EngineContext.Current.Resolve<IPaymentService>();
            var taxService = EngineContext.Current.Resolve<ITaxService>();
            var taxSettings = EngineContext.Current.Resolve<TaxSettings>();
            var paymentMethodSystemName = string.Empty;
            if (customer != null)
            {
                paymentMethodSystemName = genericAttributeService.GetAttribute<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, storeId);
            }

            //order sub total (items + checkout attributes)
            var subTotalTaxTotal = decimal.Zero;
            orderTotalCalculationService.GetShoppingCartSubTotal(cart, false, out _, out _, out _, out _, out var orderSubTotalTaxRates);
            foreach (var kvp in orderSubTotalTaxRates)
            {
                var taxRate = kvp.Key;
                var taxValue = kvp.Value;
                subTotalTaxTotal += taxValue;

                if (taxRate <= decimal.Zero || taxValue <= decimal.Zero)
                    continue;

                if (!taxRates.ContainsKey(taxRate))
                    taxRates.Add(taxRate, taxValue);
                else
                    taxRates[taxRate] = taxRates[taxRate] + taxValue;
            }

            //shipping
            var shippingTax = decimal.Zero;
            if (taxSettings.ShippingIsTaxable)
            {
                var shippingExclTax = orderTotalCalculationService.GetShoppingCartShippingTotal(cart, false, out var taxRate);
                var shippingInclTax = orderTotalCalculationService.GetShoppingCartShippingTotal(cart, true, out taxRate);
                if (shippingExclTax.HasValue && shippingInclTax.HasValue)
                {
                    shippingTax = shippingInclTax.Value - shippingExclTax.Value;
                    //ensure that tax is equal or greater than zero
                    if (shippingTax < decimal.Zero)
                        shippingTax = decimal.Zero;

                    //tax rates
                    if (taxRate > decimal.Zero && shippingTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(taxRate))
                            taxRates.Add(taxRate, shippingTax);
                        else
                            taxRates[taxRate] = taxRates[taxRate] + shippingTax;
                    }
                }
            }

            //payment method additional fee
            var paymentMethodAdditionalFeeTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                var paymentMethodAdditionalFee = paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                var paymentMethodAdditionalFeeExclTax = taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, false, customer, out var taxRate);
                var paymentMethodAdditionalFeeInclTax = taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, true, customer, out taxRate);

                paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax - paymentMethodAdditionalFeeExclTax;
                //ensure that tax is equal or greater than zero
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                //tax rates
                if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                        taxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                    else
                        taxRates[taxRate] = taxRates[taxRate] + paymentMethodAdditionalFeeTax;
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
}