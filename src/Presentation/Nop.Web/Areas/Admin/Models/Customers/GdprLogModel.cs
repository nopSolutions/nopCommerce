using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    /// <summary>
    /// Represents a GDPR log (request) model
    /// </summary>
    public partial class GdprLogModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Customers.GdprLog.Fields.CustomerInfo")]
        public string CustomerInfo { get; set; }

        [NopResourceDisplayName("Admin.Customers.GdprLog.Fields.RequestType")]
        public string RequestType { get; set; }

        [NopResourceDisplayName("Admin.Customers.GdprLog.Fields.RequestDetails")]
        public string RequestDetails { get; set; }

        [NopResourceDisplayName("Admin.Customers.GdprLog.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}