namespace Nop.Plugin.Payments.Synchrony.Models
{
    public class AuthenticationTokenResponse
    {
        public string MerchantId { get; set; }
        public string MerchantPassword { get; set; }
        public string postbackid { get; set; }
        public string clientToken { get; set; }
        public string clientTransId { get; set; }
        public bool Integration { get; set; }

        public string transactionId { get; set; }
        public string responseCode { get; set; }
        public string responseDesc { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string FirstName { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string LastName { get; set; }
        public string StatusCode { get; set; }
        public string AccountNumber { get; set; }
        public string StatusMessage { get; set; }

        public string ClientTransactionID { get; set; }
        // this should be a decimal, but is set to us in quotes.
        public string TransactionAmount { get; set; }
        public string TokenId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionDescription { get; set; }
        public string AuthCode { get; set; }
        public string accountNumber { get; set; }
        public string PromoCode { get; set; }
        public string PostbackId { get; set; }
    }
}
