namespace Nop.Plugin.Shipping.EasyPost.Domain.Shipment
{
    /// <summary>
    /// Represents the details to update the shipment
    /// </summary>
    public class UpdateShipmentRequest
    {
        /// <summary>
        /// Gets or sets the shipment options
        /// </summary>
        public Options OptionsDetails { get; set; }

        /// <summary>
        /// Gets or sets the customs info
        /// </summary>
        public CustomsInfo CustomsInfoDetails { get; set; }

        #region Nested classes

        public class Options
        {
            /// <summary>
            /// Gets or sets a value indicating whether to add an additional handling charge 
            /// </summary>
            public bool AdditionalHandling { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the shipment contains alcohol
            /// </summary>
            public bool Alcohol { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to prefer delivery by drone, if the carrier supports drone delivery.
            /// </summary>
            public bool ByDrone { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to add a charge to reduce carbon emissions
            /// </summary>
            public bool CarbonNeutral { get; set; }

            /// <summary>
            /// Gets or sets the delivery confirmation
            /// </summary>
            public string DeliveryConfirmation { get; set; }

            /// <summary>
            /// Gets or sets the endorsement type
            /// </summary>
            public string Endorsement { get; set; }

            /// <summary>
            /// Gets or sets the handling instructions
            /// </summary>
            public string HandlingInstructions { get; set; }

            /// <summary>
            /// Gets or sets the dangerous goods indicator
            /// </summary>
            public string Hazmat { get; set; }

            /// <summary>
            /// Gets or sets the invoice number
            /// </summary>
            public string InvoiceNumber { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not the parcel can be processed by the carriers equipment
            /// </summary>
            public bool Machinable { get; set; }

            /// <summary>
            /// Gets or sets the custom message
            /// </summary>
            public string PrintCustom1 { get; set; }

            /// <summary>
            /// Gets or sets the custom message code
            /// </summary>
            public string PrintCustomCode1 { get; set; }

            /// <summary>
            /// Gets or sets the custom message
            /// </summary>
            public string PrintCustom2 { get; set; }

            /// <summary>
            /// Gets or sets the custom message code
            /// </summary>
            public string PrintCustomCode2 { get; set; }

            /// <summary>
            /// Gets or sets the custom message
            /// </summary>
            public string PrintCustom3 { get; set; }

            /// <summary>
            /// Gets or sets the custom message code
            /// </summary>
            public string PrintCustomCode3 { get; set; }

            /// <summary>
            /// Gets or sets the restrictive rates from USPS
            /// </summary>
            public string SpecialRatesEligibility { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to use certified mail
            /// </summary>
            public bool CertifiedMail { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to use registered mail
            /// </summary>
            public bool RegisteredMail { get; set; }

            /// <summary>
            /// Gets or sets the value of the package contents
            /// </summary>
            public decimal RegisteredMailAmount { get; set; }

            /// <summary>
            /// Gets or sets the electronic return receipt
            /// </summary>
            public bool ReturnReceipt { get; set; }
        }

        public class CustomsInfo
        {
            /// <summary>
            /// Gets or sets the content type
            /// </summary>
            public string ContentsType { get; set; }

            /// <summary>
            /// Gets or sets the human readable description of content
            /// </summary>
            public string ContentsExplanation { get; set; }

            /// <summary>
            /// Gets or sets the restriction type
            /// </summary>
            public string RestrictionType { get; set; }

            /// <summary>
            /// Gets or sets the restriction comments
            /// </summary>
            public string RestrictionComments { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to electronically certify the information provided
            /// </summary>
            public bool CustomsCertify { get; set; }

            /// <summary>
            /// Gets or sets the signer
            /// </summary>
            public string CustomsSigner { get; set; }

            /// <summary>
            /// Gets or sets the non-delivery option
            /// </summary>
            public string NonDeliveryOption { get; set; }

            /// <summary>
            /// Gets or sets the "EEL" or "PFC"
            /// </summary>
            public string EelPfc { get; set; }
        }

        #endregion
    }
}