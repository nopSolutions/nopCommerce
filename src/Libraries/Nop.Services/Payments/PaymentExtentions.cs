using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Orders;

namespace Nop.Services.Payments
{
    public static class PaymentExtentions
    {
        /// <summary>
        /// Is payment method active?
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="paymentSettings">Payment settings</param>
        /// <returns>Result</returns>
        public static bool IsPaymentMethodActive(this IPaymentMethod paymentMethod,
            PaymentSettings paymentSettings)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException("paymentMethod");

            if (paymentSettings == null)
                throw new ArgumentNullException("paymentSettings");

            if (paymentSettings.ActivePaymentMethodSystemNames == null)
                return false;
            foreach (string activeMethodSystemName in paymentSettings.ActivePaymentMethodSystemNames)
                if (paymentMethod.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        /// <summary>
        /// Calculate payment method fee
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="orderTotalCalculationService">Order total calculation service</param>
        /// <param name="cart">Shopping cart</param>
        /// <param name="fee">Fee value</param>
        /// <param name="usePercentage">Is fee amount specified as percentage or fixed value?</param>
        /// <returns>Result</returns>
        public static decimal CalculateAdditionalFee(this IPaymentMethod paymentMethod, 
            IOrderTotalCalculationService orderTotalCalculationService, IList<ShoppingCartItem> cart,
            decimal fee, bool usePercentage)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException("paymentMethod");
            if (fee <= 0)
                return fee;

            var result = decimal.Zero;
            if (usePercentage)
            {
                //percentage
                var orderTotalWithoutPaymentFee = orderTotalCalculationService.GetShoppingCartTotal(cart, usePaymentMethodAdditionalFee: false);
                result = (decimal)((((float)orderTotalWithoutPaymentFee) * ((float)fee)) / 100f);
            }
            else
            {
                //fixed value
                result = fee;
            }
            return result;
        }
    }
}
