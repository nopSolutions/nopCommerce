using System.Collections.Generic;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Misc.AbcExportOrder.Models
{
    public class YahooHeaderRowShipping : YahooHeaderRow
    {
        public YahooHeaderRowShipping(
            string prefix,
            Order order,
            Address billingAddress,
            string stateAbbreviaton,
            string country,
            string decryptedCardName,
            string decryptedCardNumber,
            string decryptedExpirationMonth,
            string decryptedExpirationYear,
            string decryptedCvv2,
            decimal taxCharge,
            decimal shippingCharge,
            decimal homeDeliveryCharge,
            decimal total,
            string giftCard,
            decimal giftCardAmountUsed,
            string cardRefNo
        ) : base(
            prefix,
            order,
            billingAddress,
            stateAbbreviaton,
            country,
            decryptedCardName,
            decryptedCardNumber,
            decryptedExpirationMonth,
            decryptedExpirationYear,
            decryptedCvv2,
            taxCharge,
            total,
            giftCard,
            giftCardAmountUsed,
            cardRefNo
        )
        {
            Id = $"{prefix}{order.Id}+s";
            ShippingCharge = shippingCharge;
            HomeDeliveryCharge = homeDeliveryCharge;
        }
    }
}