using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Enums.Converters;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents an additional data to assist in reporting, ecommerce or moto transactions, and level 2 or level 3 processing.
    /// </summary>
    public class ExtendedInformation
    {
        /// <summary>
        /// Gets or sets a type of goods that are being purchased.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("typeOfGoods")]
        public GoodsType? GoodsType { get; set; }

        /// <summary>
        /// Gets or sets an information related to level two processing.
        /// </summary>
        [JsonProperty("levelTwoData")]
        public LevelTwoData LevelTwoData { get; set; }

        /// <summary>
        /// Gets or sets an information related to level three processing.
        /// </summary>
        [JsonProperty("levelThreeData")]
        public LevelThreeData LevelThreeData { get; set; }

        /// <summary>
        /// Gets or sets an entry source of the transaction.
        /// </summary>
        [JsonProperty("entrySource")]
        public string EntrySource { get; set; }

        /// <summary>
        /// Gets or sets an additional data for remote orders. 
        /// Required in the case of a mail, phone, or ecommerce transaction.
        /// </summary>
        [JsonProperty("mailOrTelephoneData")]
        public MailOrTelephoneData MailOrTelephoneData { get; set; }

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
        /// Gets or sets an invoice number, which is used for transaction reporting and settlement only.
        /// </summary>
        [JsonProperty("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets a description associated with the transaction, which is used for transaction reporting and settlement only.
        /// </summary>
        [JsonProperty("invoiceDescription")]
        public string InvoiceDescription { get; set; }

        /// <summary>
        /// Gets or sets a description in addition to the merchants DBA.
        /// Maximum length is 25 characters
        /// </summary>
        [JsonProperty("softDescriptor")]
        public string SoftDescriptor { get; set; }

        /// <summary>
        /// Gets or sets a 4 characters dynamic merchant category code.
        /// </summary>
        [JsonProperty("dynamicMCC")]
        public string DynamicMerchantCategory { get; set; }
    }
}