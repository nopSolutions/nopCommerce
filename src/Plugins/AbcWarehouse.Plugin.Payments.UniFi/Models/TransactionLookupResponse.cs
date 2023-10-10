using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace AbcWarehouse.Plugin.Payments.UniFi.Models
{
    public class TransactionLookupResponse
    {
        public string inquiryType { get; set; }
        public CreditAuthorizationInfo creditAuthorizationInfo { get; set; }
    }

    public class AccountInfo
    {
        [JsonProperty("cipher.accountNumber")]
        public string cipheraccountNumber { get; set; }
    }

    public class Address
    {
        [JsonProperty("cipher.addressLine1")]
        public string cipheraddressLine1 { get; set; }

        [JsonProperty("cipher.city")]
        public string ciphercity { get; set; }

        [JsonProperty("cipher.state")]
        public string cipherstate { get; set; }

        [JsonProperty("cipher.zipCode")]
        public string cipherzipCode { get; set; }
    }

    public class CreditAuthorizationInfo
    {
        public CustomerInfo customerInfo { get; set; }
        public AccountInfo accountInfo { get; set; }
        public string transactionCode { get; set; }
        public string transactionMessage { get; set; }
        public TransactionInfo transactionInfo { get; set; }
        public MerchantInfo merchantInfo { get; set; }
    }

    public class CustomerInfo
    {
        [JsonProperty("cipher.firstName")]
        public string cipherfirstName { get; set; }

        [JsonProperty("cipher.lastName")]
        public string cipherlastName { get; set; }
        public Address address { get; set; }
    }

    public class MerchantInfo
    {
        public string merchantNumber { get; set; }
        public string verificationId { get; set; }
        public string correlationId { get; set; }
    }

    public class Root
    {
        public string inquiryType { get; set; }
        public CreditAuthorizationInfo creditAuthorizationInfo { get; set; }
    }

    public class TransactionInfo
    {
        public string amount { get; set; }
        public string dateTime { get; set; }
        public string description { get; set; }
        public string authorizationCode { get; set; }
        public string promoCode { get; set; }
        public string promoType { get; set; }
    }
}
