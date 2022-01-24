using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Models.Customer
{
    /// <summary>
    /// Represents a tax exemption model
    /// </summary>
    public record TaxExemptionModel : BaseNopModel
    {
        #region Ctor

        public TaxExemptionModel()
        {
            Certificates = new List<ExemptionCertificateModel>();
            AvailableExposureZones = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public string Token { get; set; }

        public string Link { get; set; }

        public int CustomerId { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Avalara.ExemptionCertificates.Add.ExposureZone")]
        public int ExposureZone { get; set; }

        public IList<ExemptionCertificateModel> Certificates { get; set; }

        public IList<SelectListItem> AvailableExposureZones { get; set; }

        #endregion
    }
}