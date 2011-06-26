using System;
using Nop.Core.Domain.Payments;

namespace Nop.Services.Payments
{
    public static class PaymentExtentions
    {
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
    }
}
