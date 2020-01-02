using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request reason model
    /// </summary>
    public partial class ReturnRequestReasonModel : BaseNopEntityModel, ILocalizedModel<ReturnRequestReasonLocalizedModel>
    {
        #region Ctor

        public ReturnRequestReasonModel()
        {
            Locales = new List<ReturnRequestReasonLocalizedModel>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<ReturnRequestReasonLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial class ReturnRequestReasonLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name")]
        public string Name { get; set; }
    }
}