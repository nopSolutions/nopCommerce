using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Shipping.EasyPost.Models.Pickup;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.EasyPost.Models.Shipment
{
    /// <summary>
    /// Represents an additional shipment details model
    /// </summary>
    public record ShipmentDetailsModel : BaseNopEntityModel
    {
        #region Ctor

        public ShipmentDetailsModel()
        {
            AvailableRates = new List<SelectListItem>();
            AvailableDeliveryConfirmations = new List<SelectListItem>();
            AvailableEndorsements = new List<SelectListItem>();
            AvailableHazmatTypes = new List<SelectListItem>();
            AvailableCustomCodes = new List<SelectListItem>();
            AvailableSpecialRates = new List<SelectListItem>();
            PickupModel = new();
            SmartRates = new();
        }

        #endregion

        #region Properties

        #region General

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Fields.Id")]
        public string ShipmentId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Fields.Status")]
        public string Status { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Fields.RefundStatus")]
        public string RefundStatus { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Fields.Rate")]
        public string RateName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Fields.RateValue")]
        public string RateValue { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Fields.Insurance")]
        public string InsuranceValue { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Fields.PickupStatus")]
        public string PickupStatus { get; set; }

        #endregion

        #region Rates

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Rate")]
        public string RateId { get; set; }
        public IList<SelectListItem> AvailableRates { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Insurance")]
        public decimal Insurance { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Rate.SmartRate.Display")]
        public bool DisplaySmartRates { get; set; }

        public List<(string Name, int? Delivery_days, Dictionary<int, int?> TimeInTransit)> SmartRates { get; set; }

        #endregion

        #region Options

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.AdditionalHandling")]
        public bool AdditionalHandling { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.Alcohol")]
        public bool Alcohol { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.ByDrone")]
        public bool ByDrone { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.CarbonNeutral")]
        public bool CarbonNeutral { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.DeliveryConfirmation")]
        public int DeliveryConfirmation { get; set; }
        public IList<SelectListItem> AvailableDeliveryConfirmations { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.Endorsement")]
        public int Endorsement { get; set; }
        public IList<SelectListItem> AvailableEndorsements { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.HandlingInstructions")]
        public string HandlingInstructions { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.Hazmat")]
        public int Hazmat { get; set; }
        public IList<SelectListItem> AvailableHazmatTypes { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.Machinable")]
        public bool Machinable { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.PrintCustom")]
        public string PrintCustom1 { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.PrintCustomCode")]
        public int PrintCustomCode1 { get; set; }
        public IList<SelectListItem> AvailableCustomCodes { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.PrintCustom")]
        public string PrintCustom2 { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.PrintCustomCode")]
        public int PrintCustomCode2 { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.PrintCustom")]
        public string PrintCustom3 { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.PrintCustomCode")]
        public int PrintCustomCode3 { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.SpecialRatesEligibility")]
        public int SpecialRatesEligibility { get; set; }
        public IList<SelectListItem> AvailableSpecialRates { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.CertifiedMail")]
        public bool CertifiedMail { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.RegisteredMail")]
        public bool RegisteredMail { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.RegisteredMailAmount")]
        public decimal RegisteredMailAmount { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.Options.ReturnReceipt")]
        public bool ReturnReceipt { get; set; }

        #endregion

        #region Customs info

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.CustomsInfo.UseCustomsInfo")]
        public bool UseCustomsInfo { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.CustomsInfo.ContentsType")]
        public int ContentsType { get; set; }
        public SelectList AvailableContentsTypes { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.CustomsInfo.ContentsExplanation")]
        public string ContentsExplanation { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.CustomsInfo.RestrictionType")]
        public int RestrictionType { get; set; }
        public SelectList AvailableRestrictionTypes { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.CustomsInfo.RestrictionComments")]
        public string RestrictionComments { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.CustomsInfo.CustomsCertify")]
        public bool CustomsCertify { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.CustomsInfo.CustomsSigner")]
        public string CustomsSigner { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.CustomsInfo.NonDeliveryOption")]
        public int NonDeliveryOption { get; set; }
        public SelectList AvailableNonDeliveryOptions { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Shipment.CustomsInfo.EelPfc")]
        public string EelPfc { get; set; }

        #endregion

        #region Common

        public PickupModel PickupModel { get; set; }

        public bool InvoiceExists { get; set; }

        public string LabelFormat { get; set; }

        public string Error { get; set; }

        #endregion

        #endregion
    }
}