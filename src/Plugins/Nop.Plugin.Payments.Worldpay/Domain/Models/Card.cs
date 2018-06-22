using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Enums.Converters;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a credit card specific data. 
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Gets o sets a data that has been read from the card's magnetic stripe.
        /// </summary>
        [JsonProperty("trackData")]
        public string TrackData { get; set; }

        /// <summary>
        /// Gets o sets a credit card account number.
        /// </summary>
        [JsonProperty("number")]
        public string CardNumber { get; set; }

        /// <summary>
        /// Gets o sets a card security code (CVV).
        /// </summary>
        [JsonProperty("cvv")]
        public string CardCvv { get; set; }

        /// <summary>
        /// Gets o sets an expiration date for the card. Format: MM/YYYY.
        /// </summary>
        [JsonProperty("expirationDate")]
        public string ExpirationDate { get; set; }

        /// <summary>
        /// Gets o sets a last 4 digits of card number.
        /// </summary>
        [JsonProperty("lastFourDigits")]
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets o sets a masked card number.
        /// </summary>
        [JsonProperty("maskedNumber")]
        public string MaskedNumber { get; set; }

        /// <summary>
        /// Gets o sets a credit card type.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("creditCardType")]
        public CreditCardType? CreditCardType { get; set; }

        /// <summary>
        /// Gets o sets an unaltered KSN Number from PIN pad. (Debit only.)
        /// </summary>
        [JsonProperty("ksn")]
        public string DebitKsnNumber { get; set; }

        /// <summary>
        /// Gets o sets a pinblock obtained from the PIN pad. (Debit only.)
        /// </summary>
        [JsonProperty("pinBlock")]
        public string DebitPinBlock { get; set; }

        /// <summary>
        /// Gets o sets a first name of the account holder.
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets o sets a last name of the account holder.
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets o sets a billing address of the account holder.
        /// </summary>
        [JsonProperty("address")]
        public Address BillingAddress { get; set; }

        /// <summary>
        /// Gets o sets an image of the signature of the cardholder as completed at the time of the transaction.
        /// </summary>
        [JsonProperty("signature")]
        public byte[] Signature { get; set; }

        /// <summary>
        /// Gets o sets an email address of the account holder.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets o sets a value indicating whether to enable or disable the functionality to send receipt via email.
        /// </summary>
        [JsonProperty("emailReceipt")]
        public bool EmailReceiptEnabled { get; set; }

        /// <summary>
        /// Gets o sets a value indicating the channel in which the card was input.
        /// </summary>
        [JsonProperty("contactlessIndicator")]
        public CardInputType? CardInputType { get; set; }

        /// <summary>
        /// Gets o sets a value indicating whether the card should be processed as a debit card.
        /// </summary>
        [JsonProperty("isDebit")]
        public CardType? CardType { get; set; }
    }
}