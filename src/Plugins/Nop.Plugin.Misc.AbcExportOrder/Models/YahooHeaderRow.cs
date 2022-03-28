using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.AbcExportOrder.Extensions;

namespace Nop.Plugin.Misc.AbcExportOrder.Models
{
    public class YahooHeaderRow
    {
        public string Id { get; protected set; }
        public string Datestamp { get; protected set; }
        public string FullName { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public string Address1 { get; protected set; }
        public string Address2 { get; protected set; }
        public string City { get; protected set; }
        public string State { get; protected set; }
        public string Zip { get; protected set; }
        public string Country { get; protected set; }
        public string Phone { get; protected set; }
        public string Email { get; protected set; }
        public string CardName { get; protected set; }
        public string CardNumber { get; protected set; }
        public string CardExpiry { get; protected set; }
        public string CardCvv2 { get; protected set; }
        public decimal TaxCharge { get; protected set; }
        public decimal ShippingCharge { get; protected set; }
        public decimal HomeDeliveryCharge { get; protected set; }
        public decimal Total { get; protected set; }
        public string Ip { get; protected set; }
        public string GiftCard { get; protected set; }
        public decimal GiftCardAmountUsed { get; protected set; }
        public string AuthCode { get; protected set; }
        public string CcRefNo { get; protected set; }

        public YahooHeaderRow(
            string prefix,
            Order order,
            Address billingAddress,
            string stateAbbreviation,
            string country,
            string decryptedCardName,
            string decryptedCardNumber,
            string decryptedExpirationMonth,
            string decryptedExpirationYear,
            string decryptedCvv2,
            decimal taxCharge,
            decimal total,
            string giftCard,
            decimal giftCardAmountUsed,
            string cardRefNo
        )
        {
            Id = $"{prefix}{order.Id}+p";
            FullName = $"{billingAddress.FirstName} {billingAddress.LastName}";
            FirstName = billingAddress.FirstName;
            LastName = billingAddress.LastName;
            Address1 = billingAddress.Address1;
            Address2 = billingAddress.Address2;
            City = billingAddress.City;
            State = stateAbbreviation;
            Zip = billingAddress.ZipPostalCode;
            Country = country;
            Phone = billingAddress.PhoneNumber;
            Email = billingAddress.Email;
            CardName = decryptedCardName;
            CardNumber = decryptedCardNumber != null ?
                new string(decryptedCardNumber.Where(c => char.IsDigit(c)).ToArray()) :
                null;
            CardExpiry = $"{decryptedExpirationMonth}/{decryptedExpirationYear}";
            CardCvv2 = decryptedCvv2;
            TaxCharge = taxCharge;
            Total = total;
            GiftCard = giftCard;
            GiftCardAmountUsed = giftCardAmountUsed;

            AuthCode = order.AuthorizationTransactionCode;
            CcRefNo = cardRefNo;
            Datestamp = order.CreatedOnUtc.ToString();
            Ip = order.CustomerIp;
            if (!string.IsNullOrWhiteSpace(Ip) && Ip.IndexOf(",") > 0)
            {
                var index = Ip.IndexOf(",");
                Ip = Ip.Substring(0, index - 1);
            }
        }

        public List<string> ToStringValues()
        {
            return new List<string>()
            {
                Id,
                Datestamp,
                FullName,
                FirstName,
                LastName,
                Address1,
                Address2,
                City,
                State,
                Zip,
                Country,
                Phone,
                Email,
                CardName,
                CardNumber,
                CardExpiry,
                TaxCharge.ToString(),
                ShippingCharge.ToString(),
                Total.ToString(),
                CardCvv2,
                Ip,
                GiftCard,
                GiftCardAmountUsed.ToString(),
                AuthCode,
                HomeDeliveryCharge.ToString(),
                CcRefNo
            };
        }
    }
}