using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Enums.Converters;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a transaction details
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Gets or sets a merchant account identifier.
        /// </summary>
        [JsonProperty("secureNetId")]
        public int SecureNetId { get; set; }

        /// <summary>
        /// Gets or sets a transaction type.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("transactionType")]
        public TransactionType? TransactionType { get; set; }

        /// <summary>
        /// Gets or sets a transaction approval status.
        /// </summary>
        [JsonProperty("responseText")]
        public string ResponseText { get; set; }

        /// <summary>
        /// Gets or sets a client-generated unique ID for each transaction, used as a way to prevent the processing of duplicate transactions. 
        /// The orderId must be unique to the merchant's Worldpay ID; however, uniqueness is only evaluated for APPROVED transactions and only for the last 30 days. 
        /// If a transaction is declined, the corresponding orderId may be used again.
        /// The orderId is limited to 25 characters.
        /// </summary>
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// Gets or sets a transaction identifier.
        /// </summary>
        [JsonProperty("transactionId")]
        public int TransactionId { get; set; }

        /// <summary>
        /// Gets or sets an authorization approval code.
        /// </summary>
        [JsonProperty("authorizationCode")]
        public string AuthorizationCode { get; set; }

        /// <summary>
        /// Gets or sets an authorized amount. 
        /// </summary>
        [JsonProperty("authorizedAmount")]
        public decimal AuthorizedAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether partial payments are to be accepted.
        /// </summary>
        [JsonProperty("allowedPartialCharges")]
        public bool AllowedPartialCharges { get; set; }

        /// <summary>
        /// Gets or sets a credit card or check type.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("paymentTypeCode")]
        public PaymentMethodCode? PaymentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets a payment method type.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("paymentTypeResult")]
        public PaymentCode? PaymentCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the credit card is level 2 eligible.
        /// </summary>
        [JsonProperty("level2Valid")]
        public bool IsLevel2Valid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the credit card is level 3 eligible.
        /// </summary>
        [JsonProperty("level3Valid")]
        public bool IsLevel3Valid { get; set; }

        /// <summary>
        /// Gets or sets a date/time and amount of the authorized transaction.
        /// </summary>
        [JsonProperty("transactionData")]
        public AuthorizedTransactionData AuthorizedTransactionData { get; set; }

        /// <summary>
        /// Gets or sets a date/time, amount, and batchId of the settled transaction.
        /// </summary>
        [JsonProperty("settlementData")]
        public ChargedTransactionData ChargedTransactionData { get; set; }

        /// <summary>
        /// Gets or sets a customer data and Vault token.
        /// </summary>
        [JsonProperty("vaultData")]
        public VaultData VaultData { get; set; }

        /// <summary>
        /// Gets or sets a card type of the payment account.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("creditCardType")]
        public CreditCardType? CreditCardType { get; set; }

        /// <summary>
        /// Gets or sets a last 4 digits of the credit card number. Format: XXXXXXXXXXXX 3456.
        /// </summary>
        [JsonProperty("cardNumber")]
        public string MaskedCardNumber { get; set; }

        /// <summary>
        /// Gets or sets an address verification code. 
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("avsCode")]
        public AvsCode? AvsCode { get; set; }

        /// <summary>
        /// Gets or sets an address verification result.  
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("avsResult")]
        public AvsResultType? AvsResult { get; set; }

        /// <summary>
        /// Gets or sets a first name of the cardholder.
        /// </summary>
        [JsonProperty("cardHolder_FirstName")]
        public string CardHolderFirstName { get; set; }

        /// <summary>
        /// Gets or sets a last name of the cardholder.
        /// </summary>
        [JsonProperty("cardHolder_LastName")]
        public string CardHolderLastName { get; set; }

        /// <summary>
        /// Gets or sets a card expiration date. 
        /// </summary>
        [JsonProperty("expirationDate")]
        public string CardExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets a billing address of the customer.
        /// </summary>
        [JsonProperty("billAddress")]
        public Address BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets an email address of the customer.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a card security code result.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("cardCodeCode")]
        public CvvCode? CvvCode { get; set; }

        /// <summary>
        /// Gets or sets a card security code result.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("cardCodeResult")]
        public CvvResultType? CvvResult { get; set; }

        /// <summary>
        /// Gets or sets an account name of the bank account.
        /// </summary>
        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets an account type of the bank account. 
        /// </summary>
        [JsonProperty("accountType")]
        public string AccountType { get; set; }

        /// <summary>
        /// Gets or sets a last 4 digits of the bank account number. 
        /// </summary>
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Gets or sets an individual check number. 
        /// </summary>
        [JsonProperty("checkNumber")]
        public string CheckNumber { get; set; }

        /// <summary>
        /// Gets or sets a trace number of an ACH check transaction.
        /// </summary>
        [JsonProperty("traceNumber")]
        public string TraceNumber { get; set; }

        /// <summary>
        /// Gets or sets a surcharge amount to be added to the transaction.
        /// </summary>
        [JsonProperty("surchargeAmount")]
        public decimal SurchargeAmount { get; set; }

        /// <summary>
        /// Gets or sets a cash back amount given to a customer. Commonly used for debit transactions.
        /// </summary>
        [JsonProperty("cashbackAmount")]
        public decimal CashbackAmount { get; set; }

        /// <summary>
        /// Gets or sets a tip amount.
        /// </summary>
        [JsonProperty("gratuity")]
        public decimal Gratuity { get; set; }

        /// <summary>
        /// Gets or sets an industry-specific data for ecommerce and moto transactions.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("industrySpecificData")]
        public IndustrySpecificDataType? IndustrySpecificData { get; set; }

        /// <summary>
        /// Gets or sets an identifier for the network that returned the transaction response.
        /// </summary>
        [JsonProperty("networkCode")]
        public string NetworkCode { get; set; }

        /// <summary>
        /// Gets or sets a remaining balance on a stored value account.
        /// </summary>
        [JsonProperty("additionalAmount")]
        public decimal AdditionalAmount { get; set; }

        /// <summary>
        /// Gets or sets a transaction method.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("method")]
        public PaymentMethodType? PaymentMethodType { get; set; }

        /// <summary>
        /// Gets or sets a customer identifier.
        /// </summary>
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets o sets a value indicating whether to enable or disable the functionality to send reciept via email.
        /// </summary>
        [JsonProperty("emailReceipt")]
        public bool EmailReceiptEnabled { get; set; }

        /// <summary>
        /// Gets or sets an unique number assigned to a person that participates in the SNAP program.
        /// </summary>
        [JsonProperty("fnsNumber")]
        public string FnsNumber { get; set; }

        /// <summary>
        /// Gets or sets a 15 character number representing the EBT Voucher account.
        /// </summary>
        [JsonProperty("voucherNumber")]
        public string VoucherNumber { get; set; }

        /// <summary>
        /// For stored value cards this is the Previous Balance (15 characters long zero filled decimal number with explicit decimal point) followed by Cash Out Amount (15 characters long zero filled decimal number with explicit decimal point).
        /// </summary>
        [JsonProperty("additionalData1")]
        public string AdditionalData1 { get; set; }

        /// <summary>
        /// For EBT transactions, it will contain the FNS # echoed from the ADDITIONALINFO field of the MERCHANT_KEY object. 
        /// For stored value cards this is the virtual issued card number.
        /// </summary>
        [JsonProperty("additionalData2")]
        public string AdditionalData2 { get; set; }

        /// <summary>
        /// For EBT transactions, it will contain the Voucher # echoed from the ADDITIONALINFO field of the CARD object. 
        /// For stored value cards it is the issued card EXP date.
        /// </summary>
        [JsonProperty("additionalData3")]
        public string AdditionalData3 { get; set; }

        /// <summary>
        /// Gets or sets a virtual issued card pin number. 
        /// Used only for specific stored value card transactions.
        /// </summary>
        [JsonProperty("additionalData4")]
        public string AdditionalData4 { get; set; }

        /// <summary>
        /// Gets or sets a P2P Encryption Transaction Response-String (22 Characters): 7-digit Key ID + {1,2} + MMddyyyyHHmmss
        /// </summary>
        [JsonProperty("additionalData5")]
        public string AdditionalData5 { get; set; }

        /// <summary>
        /// Gets or sets a description in addition to the merchants DBA.
        /// </summary>
        [JsonProperty("softDescriptor")]
        public string SoftDescriptor { get; set; }

        /// <summary>
        /// Gets or sets a 4 characters dynamic merchant category code.
        /// </summary>
        [JsonProperty("dynamicMCC")]
        public string DynamicMerchantCategory { get; set; }

        /// <summary>
        /// Gets or sets custom user-defined fields tied to the transaction, which is used for transaction reporting and settlement only.
        /// </summary>
        [JsonProperty("userDefinedFields")]
        public IList<KeyValuePair<string, string>> UserDefinedFields { get; set; }

        /// <summary>
        /// Gets or sets notes associated with the transaction, which is used for transaction reporting and settlement only.
        /// </summary>
        [JsonProperty("notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets a description associated with the transaction, which is used for transaction reporting and settlement only.
        /// </summary>
        [JsonProperty("invoiceDescription")]
        public string InvoiceDescription { get; set; }

        /// <summary>
        /// Gets or sets a Visa Checkout transaction ID associated with a payment request.
        /// </summary>
        [JsonProperty("callId")]
        public string CallId { get; set; }
    }
}