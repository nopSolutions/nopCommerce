using System.Runtime.Serialization;

namespace Nop.Plugin.Shipping.EasyPost.Domain.Shipment
{
    /// <summary>
    /// Represents delivery confirmation enumeration
    /// </summary>
    public enum DeliveryConfirmation
    {
        /// <summary>
        /// None
        /// </summary>
        [EnumMember(Value = "---")]
        None,

        /// <summary>
        /// Adult signature
        /// </summary>
        [EnumMember(Value = "ADULT_SIGNATURE")]
        AdultSignature,

        /// <summary>
        /// Signature
        /// </summary>
        [EnumMember(Value = "SIGNATURE")]
        Signature,

        /// <summary>
        /// No signature
        /// </summary>
        [EnumMember(Value = "NO_SIGNATURE")]
        NoSignature,

        /// <summary>
        /// Do not safe drop
        /// </summary>
        [EnumMember(Value = "DO_NOT_SAFE_DROP")]
        DoNotSafeDrop,

        /// <summary>
        /// Adult signature restricted
        /// </summary>
        [EnumMember(Value = "ADULT_SIGNATURE_RESTRICTED")]
        AdultSignatureRestricted,

        /// <summary>
        /// Signature restricted
        /// </summary>
        [EnumMember(Value = "SIGNATURE_RESTRICTED")]
        SignatureRestricted
    }

    /// <summary>
    /// Represents endorsement enumeration
    /// </summary>
    public enum Endorsement
    {
        /// <summary>
        /// None
        /// </summary>
        [EnumMember(Value = "---")]
        None,

        /// <summary>
        /// Address service requested
        /// </summary>
        [EnumMember(Value = "ADDRESS_SERVICE_REQUESTED")]
        AddressServiceRequested,

        /// <summary>
        /// Forwarding service requested
        /// </summary>
        [EnumMember(Value = "FORWARDING_SERVICE_REQUESTED")]
        ForwardingServiceRequested,

        /// <summary>
        /// Change service requested
        /// </summary>
        [EnumMember(Value = "CHANGE_SERVICE_REQUESTED")]
        ChangeServiceRequested,

        /// <summary>
        /// Return service requested
        /// </summary>
        [EnumMember(Value = "RETURN_SERVICE_REQUESTED")]
        ReturnServiceRequested,

        /// <summary>
        /// Leave if no response
        /// </summary>
        [EnumMember(Value = "LEAVE_IF_NO_RESPONSE")]
        LeaveIfNoResponse
    }

    /// <summary>
    /// Represents hazardous material type enumeration
    /// </summary>
    public enum HazmatType
    {
        /// <summary>
        /// None
        /// </summary>
        [EnumMember(Value = "---")]
        None,

        /// <summary>
        /// Primary contained
        /// </summary>
        [EnumMember(Value = "PRIMARY_CONTAINED")]
        PrimaryContained,

        /// <summary>
        /// Primary packed
        /// </summary>
        [EnumMember(Value = "PRIMARY_PACKED")]
        PrimaryPacked,

        /// <summary>
        /// Primary
        /// </summary>
        [EnumMember(Value = "PRIMARY")]
        Primary,

        /// <summary>
        /// Secondary contained
        /// </summary>
        [EnumMember(Value = "SECONDARY_CONTAINED")]
        SecondaryContained,

        /// <summary>
        /// Secondary packed
        /// </summary>
        [EnumMember(Value = "SECONDARY_PACKED")]
        SecondaryPacked,

        /// <summary>
        /// Secondary
        /// </summary>
        [EnumMember(Value = "SECONDARY")]
        Secondary,

        /// <summary>
        /// ORMD
        /// </summary>
        [EnumMember(Value = "ORMD")]
        Ormd,

        /// <summary>
        /// Limited quantity
        /// </summary>
        [EnumMember(Value = "LIMITED_QUANTITY")]
        LimitedQuantity,

        /// <summary>
        /// Lithium
        /// </summary>
        [EnumMember(Value = "LITHIUM")]
        Lithium
    }

    /// <summary>
    /// Represents special rates enumeration
    /// </summary>
    public enum SpecialRate
    {
        /// <summary>
        /// None
        /// </summary>
        [EnumMember(Value = "---")]
        None,

        /// <summary>
        /// USPS MediaMail
        /// </summary>
        [EnumMember(Value = "USPS.MEDIAMAIL")]
        UspsMediamail,

        /// <summary>
        /// USPS LibraryMail
        /// </summary>
        [EnumMember(Value = "USPS.LIBRARYMAIL")]
        UspsLibrarymail
    }

    /// <summary>
    /// Represents custom code enumeration
    /// </summary>
    public enum CustomCode
    {
        /// <summary>
        /// None
        /// </summary>
        [EnumMember(Value = "---")]
        None,

        /// <summary>
        /// Accounts Receivable Customer Account
        /// </summary>
        [EnumMember(Value = "AJ")]
        AccountsReceivableCustomerAccount,

        /// <summary>
        /// Appropriation Number
        /// </summary>
        [EnumMember(Value = "AT")]
        AppropriationNumber,

        /// <summary>
        /// Bill of Lading Number
        /// </summary>
        [EnumMember(Value = "BM")]
        BillOfLadingNumber,

        /// <summary>
        /// Collect on Delivery (COD) Number
        /// </summary>
        [EnumMember(Value = "9V")]
        CollectOnDeliveryNumber,

        /// <summary>
        /// Dealer Order Number
        /// </summary>
        [EnumMember(Value = "ON")]
        DealerOrderNumber,

        /// <summary>
        /// Department Number
        /// </summary>
        [EnumMember(Value = "DP")]
        DepartmentNumber,

        /// <summary>
        /// Food and Drug Administration (FDA) Product Code
        /// </summary>
        [EnumMember(Value = "3Q")]
        FoodAndDrugAdministrationProductCode,

        /// <summary>
        /// Invoice Number
        /// </summary>
        [EnumMember(Value = "IK")]
        InvoiceNumber,

        /// <summary>
        /// Manifest Key Number
        /// </summary>
        [EnumMember(Value = "MK")]
        ManifestKeyNumber,

        /// <summary>
        /// Model Number
        /// </summary>
        [EnumMember(Value = "MJ")]
        ModelNumber,

        /// <summary>
        /// Part Number
        /// </summary>
        [EnumMember(Value = "PM")]
        PartNumber,

        /// <summary>
        /// Production Code
        /// </summary>
        [EnumMember(Value = "PC")]
        ProductionCode,

        /// <summary>
        /// Purchase Order Number
        /// </summary>
        [EnumMember(Value = "PO")]
        PurchaseOrderNumber,

        /// <summary>
        /// Purchase Request Number
        /// </summary>
        [EnumMember(Value = "RQ")]
        PurchaseRequestNumber,

        /// <summary>
        /// Return Authorization Number
        /// </summary>
        [EnumMember(Value = "RZ")]
        ReturnAuthorizationNumber,

        /// <summary>
        /// Salesperson Number
        /// </summary>
        [EnumMember(Value = "SA")]
        SalespersonNumber,

        /// <summary>
        /// Serial Number
        /// </summary>
        [EnumMember(Value = "SE")]
        SerialNumber,

        /// <summary>
        /// Store Number
        /// </summary>
        [EnumMember(Value = "ST")]
        StoreNumber,

        /// <summary>
        /// Transaction Reference Number
        /// </summary>
        [EnumMember(Value = "TN")]
        TransactionReferenceNumber,

        /// <summary>
        /// Federal Taxpayer ID No
        /// </summary>
        [EnumMember(Value = "TJ")]
        FederalTaxpayerId,

        /// <summary>
        /// Employer’s ID Number
        /// </summary>
        [EnumMember(Value = "EI")]
        EmployerNumber,

        /// <summary>
        /// Return Merchandise Authorization
        /// </summary>
        [EnumMember(Value = "RMA")]
        ReturnMerchandiseAuthorization
    }
}